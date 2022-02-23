using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using XLua;
using System.Net;

using Prota.Lua;
using System.Collections.Generic;

using Prota.Unity;

using NetCallback = System.Action<LiteNetLib.NetPeer, LiteNetLib.Utils.NetDataReader, LiteNetLib.DeliveryMethod>;

namespace Prota.Net
{
    public static class BuiltinMsgId
    {
        // 告知服务器内容.
        public const int S2CNotifyInfo = -1;
        
        // 告知有客户端连接/断开服务器.
        public const int S2CAddOrRemoveClient = -2;
        
        // 告知服务器客户端的一些基础信息.
        public const int C2SReportInfo = -10;
        
        // P2P 连接 request.
        public const int C2SRequestP2PConnection = -101;
        public const int S2CResponseP2PConnection = -101;
        
        
        // 建立 P2P 连接后, 客户端向主机发送逻辑接入请求.
        public const int C2CRequestClientConnection = -102;
        // 主机受到客户端的请求, 回复是否接收成功.
        public const int C2CResponseClientConnection = -103;
        
        
        // 向服务器声明自己是/不是主机. 服务器通知所有连接.
        public const int C2SRequestHost = -110;
        public const int S2CNotifyHost = -110;
        
        // 数据传输.
        public const int C2CAddRecord = -201;
        public const int C2CRemoveRecord = -202;
        public const int C2CModifyData = -203;
        
    }
    
    public struct NetId
    {
        public readonly long id;
        
        public NetId(long id)
        {
            this.id = id;
        }
        
        public static NetId None => new NetId(0);
        
        public bool valid => id != 0;

        public override bool Equals(object obj) => obj is NetId id && this.id == id.id;

        public override int GetHashCode() => id.GetHashCode();

        public override string ToString() => $"NetId[{ id }]";

        public static bool operator==(NetId a, NetId b) => a.id == b.id;
        public static bool operator!=(NetId a, NetId b) => a.id != b.id;
        
        public static implicit operator long(NetId id) => id.id;
        public static implicit operator NetId(long id) => new NetId(id);
    }
    
    // 一个能够和服务器建立连接, 且能相互之间建立连接的对象.
    public class ClientBase : MonoBehaviour
    {
        [Serializable]
        public class ClientInfo
        {
            public NetId id;
            public string ip;
            public int port;
            public IPEndPoint endpoint;
            public bool isHost;
            
            public ClientInfo(NetId id, string ip, int port)
            {
                this.id = id;
                this.ip = ip;
                this.port = port;
                this.endpoint = LiteNetLib.NetUtils.MakeEndPoint(ip, port);
            }
        }
        
        readonly static IPEndPoint gateEndpoint = new IPEndPoint(IPAddress.Parse("106.54.195.7"), 33200);
        
        [NonSerialized]
        public NetManager client;
        
        public NetPeer peerToServer;
        
        protected EventBasedNetListener listener { get; private set; }
        
        protected EventBasedNatPunchListener NATPunchListener { get; private set; }
        
        protected virtual string authorizedKey => "ProtaClient";
        
        public NetId myId;
        
        public IPEndPoint myEndpoint;
        
        public Dictionary<int, List<NetCallback>> callbacks = new Dictionary<int, List<NetCallback>>();
        
        [NonSerialized]
        public List<NetPeer> connectedPeers = new List<NetPeer>();
        
        [NonSerialized]
        public List<ClientInfo> serverConnections = new List<ClientInfo>();
        
        public event Action<NetPeer> onConnect;
        
        public event Action<NetPeer> onDisconnect;
        
        protected virtual void Start()
        {
            listener = new EventBasedNetListener();
            NATPunchListener = new EventBasedNatPunchListener();
            client = new NetManager(listener);
            client.NatPunchEnabled = true;
            client.NatPunchModule.Init(NATPunchListener);
            client.Start();
            ConnectToServer();
            
            listener.ConnectionRequestEvent += req => {
                Log.Info($"{ this } 接收到了建立连接请求 { req.RemoteEndPoint } 数据: [{ req.Data.GetString() }]");
                // 客户端可以无脑接收.
                req.Accept();
            };
            
            listener.PeerConnectedEvent += peer => {
                if(peer.EndPoint.Equals(gateEndpoint))
                {
                    Log.Info($"{ this } 连接服务器成功 { peer.EndPoint }");
                    peerToServer = peer;
                    SendToServer(w => {
                        w.Put(BuiltinMsgId.C2SReportInfo);
                        w.Put(this.GetInstanceID());
                        w.Put(this.ToString());
                    });
                }
                else
                {
                    Log.Info($"{ this } 建立 p2p 连接 { peer.EndPoint }");
                }
                
                connectedPeers.Add(peer);
                
                onConnect?.Invoke(peer);
            };
            
            listener.PeerDisconnectedEvent += (peer, info) => {
                onDisconnect?.Invoke(peer);
                connectedPeers.Remove(peer);
                if(peer.EndPoint.Equals(gateEndpoint))
                {
                    Log.Info($"{ this } 断开与服务器的连接: { peer.EndPoint }");
                }
                else
                {
                    Log.Info($"{ this } 断开与客户端的连接: { peer.EndPoint }");
                }
                return;
            };
            
            listener.NetworkReceiveEvent += (peer, reader, method) => {
                var type = reader.GetInt();
                callbacks.GetOrCreate(type, out var list);
                if(list.Count == 0)
                {
                    Log.Warning("收到未处理的数据包类型 " + type);
                }
                
                foreach(var callback in list)
                {
                    var tempReader = new NetDataReader(reader.RawData, reader.UserDataOffset + 4, reader.UserDataSize - 4);  // 去掉开头的 type 字段.
                    callback(peer, tempReader, method);
                }
                
                reader.Recycle();
            };
            
            RegisterInternalCallbacks();
        }
        
        protected virtual void Update()
        {
            client.PollEvents();
        }
        
        protected virtual void OnDestroy()
        {
            client.Stop();
        }
        
        // ============================================================================================================
        // Operation
        // ============================================================================================================
        
        public void ConnectToServer()
        {
            DisconnectToServer();
            peerToServer = client.Connect(gateEndpoint, authorizedKey);
        }
        
        public void DisconnectToServer()
        {
            if(peerToServer == null) return;
            peerToServer.Disconnect();
            peerToServer = null;
        }
        
        public void DisconnectAllClient()
        {
            foreach(var peer in connectedPeers)
            {
                if(peer != peerToServer) peer.Disconnect();
            }
        }
        
        protected void ConnectTo(IPEndPoint endpoint)
        {
            client.Connect(endpoint, authorizedKey);
        }
        
        public void ConnectTo(string endpoint)
        {
            var parts = endpoint.Split(":");
            if(parts.Length != 2) throw new Exception("Invalid endpoint " + endpoint);
            var ipEndpoint = LiteNetLib.NetUtils.MakeEndPoint(parts[0], int.Parse(parts[1]));
            ConnectTo(ipEndpoint);
        } 
        
        void DisconnectTo(NetPeer peer)
        {
            peer.Disconnect();
        }
        
        public void DisconnectTo(NetId key)
        {
            if(!RequireClinetInfo(key, out var clientInfo)) return;
            if(!RequirePeer(clientInfo.endpoint, out var target)) return;
            if(!key.valid)
            {
                Log.Error("找到的 key 是 0?");
                return;
            }
            target.Disconnect();
        }
        
        protected void DisconnectTo(IPEndPoint endpoint)
        {
            var target = client.ConnectedPeerList.Find(x => x.EndPoint.Equals(endpoint));
            target?.Disconnect();
        }
        
        // 通过服务器, 建立 p2p 连接.
        public void ConnectClientByServer(NetId key)
        {
            if(!RequireClinetInfo(key, out var clientInfo)) return;
            Log.Info($"[内网穿透]告知服务器想要连接 [{ clientInfo.endpoint }]");
            SendToServer(w => {
                w.Put(BuiltinMsgId.C2SRequestP2PConnection);
                w.Put(myId.id);
                w.Put(key.id);
            });
        }
        
        public bool TryGetIdByEndpoint(IPEndPoint endpoint, out NetId id)
        {
            var clientInfo = serverConnections.Find(x => x.endpoint.Equals(endpoint));
            id = clientInfo.id;
            return clientInfo != null;
        }
        
        bool RequireClinetInfo(NetId key, out ClientInfo clientInfo)
        {
            clientInfo = serverConnections.Find(x => x.id == key);
            if(clientInfo == null)
            {
                Log.Info("找不到 ClientInfo: " + key);
                return false;
            }
            return true;
        }
        
        bool RequirePeer(IPEndPoint endpoint, out NetPeer target)
        {
            target = client.ConnectedPeerList.Find(x => x.EndPoint.Equals(endpoint));
            if(target == null)
            {
                Log.Info("找不到 target: " + endpoint);
                return false;
            }
            return true;
        }
        
        
        // ============================================================================================================
        // Send
        // ============================================================================================================
        
        public void SendToServer(Action<NetDataWriter> f, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            SendToPeer(peerToServer, f, deliveryMethod);
        }
        
        public NetDataWriter writer = new NetDataWriter();
        public void SendToPeer(NetPeer peer, Action<NetDataWriter> f, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            lock(writer)
            {
                writer.Reset();
                f(writer);
                peer.Send(writer, deliveryMethod);
            }
        }
        
        // ============================================================================================================
        // Receive
        // ============================================================================================================
        
        public void AddCallback(int type, NetCallback callback)
        {
            callbacks.GetOrCreate(type, out var list);
            if(list.Contains(callback)) return;
            list.Add(callback);
        }
        
        public bool RemoveCallback(int type, NetCallback callback)
        {
            if(callbacks.TryGetValue(type, out var list))
            {
                return list.Remove(callback);
            }
            return false;
        }
        
        public bool ClearCallback(int type)
        {
            if(callbacks.TryGetValue(type, out var list))
            {
                list.Clear();
                return true;
            }
            return false;
        }
        
        public bool ClearAllCallbacks()
        {
            callbacks.Clear();
            return true;
        }
        
        
        void RegisterInternalCallbacks()
        {
            // 从服务器获取一些连接的基础信息.
            AddCallback(BuiltinMsgId.S2CNotifyInfo, (peer, reader, method) => {
                myId = new NetId(reader.GetLong());
                myEndpoint = reader.GetNetEndPoint();
            });
            
            // 服务器告知有新的连接加入/退出.
            AddCallback(BuiltinMsgId.S2CAddOrRemoveClient, (peer, reader, method) => {
                var op = reader.GetInt();
                var id = reader.GetLong();           // 服务器一侧保存的 id.
                var endpoint = reader.GetNetEndPoint();
                if(op == 1)     // 加入.
                {
                    serverConnections.Add(new ClientInfo(new NetId(id), endpoint.Address.ToString(), endpoint.Port));
                }
                else            // 退出.
                {
                    serverConnections.RemoveAll(x => x.id == new NetId(id));
                }
            });
            
            // 建立 p2p 连接. 第三步; 连接到目标端点.
            AddCallback(BuiltinMsgId.S2CResponseP2PConnection, (peer, reader, method) => {
                var otherEndpoint = reader.GetNetEndPoint();
                Log.Info($"[内网穿透]服务器告知需要连接 [{ otherEndpoint }]");
                client.Connect(otherEndpoint, "ProtaClient");
            });
            
            AddCallback(BuiltinMsgId.S2CNotifyHost, (peer, reader, method) => {
                var id = reader.GetLong();
                var clientInfo = serverConnections.Find(x => x.id == id);
                if(clientInfo == null)
                {
                    Log.Info($"从服务器收到了客户端[{ id }]变更为主机通知, 但是找不到这个客户端.");
                    return;
                }
                
                clientInfo.isHost = reader.GetBool();
            });
        }
        
        
    }
}