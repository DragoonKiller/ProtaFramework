using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using XLua;
using System.Net;

using Prota.Lua;
using System.Collections.Generic;

using Prota.Unity;

using NetCallback = System.Action<Prota.Net.NetId, LiteNetLib.Utils.NetDataReader, LiteNetLib.DeliveryMethod>;

namespace Prota.Net
{
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
        
        readonly static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("106.54.195.7"), 33200);
        
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
        
        public event Action<NetId> onConnect;
        
        public event Action<NetId> onDisconnect;
        
        protected virtual void Start()
        {
            listener = new EventBasedNetListener();
            NATPunchListener = new EventBasedNatPunchListener();
            client = new NetManager(listener);
            
            client.UpdateTime = 10;                     // 收发包等待间隔.
            client.DisconnectTimeout = 60000;           // 一分钟超时断开连接.
            client.BroadcastReceiveEnabled = true;      // 打开广播接收.
            client.ReconnectDelay = 2000;               // 这么多时间没有回复后重新发送连接协议.
            client.MaxConnectAttempts = 100000;         // 连不上时的重连尝试次数. 相当于一直尝试连接直到连上为止.
            client.PingInterval = 3000;                 // ping 检测 & 心跳包时间间隔.
            client.UnsyncedDeliveryEvent = true;        // 调用发送时直接发送, 不等 PollEvent.
             
            client.Start();
            ConnectToServer();
            
            listener.ConnectionRequestEvent += req => {
                Log.Info($"{ this } 接收到了建立连接请求 { req.RemoteEndPoint } 数据: [{ req.Data.GetString() }]");
                // 必须在 serverConnections 列表里才接收.
                if(!NeedClientInfo(req.RemoteEndPoint, out var _))
                {
                    req.Reject();
                    return;
                }
                req.Accept();
            };
            
            listener.PeerConnectedEvent += peer => {
                if(peer.EndPoint.Equals(serverEndpoint))
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
                var id = NetId.None;
                if(peer != peerToServer && !NeedPeerId(peer, out id))
                {
                    Log.Warning($"{ this } 连接不在服务器下发的客户端列表里, 断开连接 { peer.EndPoint }");
                    peer.Disconnect();
                    return;
                }
                
                onConnect?.Invoke(id);
            };
            
            listener.PeerDisconnectedEvent += (peer, info) => {
                connectedPeers.Remove(peer);
                NeedPeerId(peer, out var id);
                
                if(peer.EndPoint.Equals(serverEndpoint))
                {
                    Log.Info($"{ this } 断开与服务器的连接: { peer.EndPoint } id: { id }");
                }
                else
                {
                    Log.Info($"{ this } 断开与客户端的连接: { peer.EndPoint } id: { id }");
                }
                
                if(id != NetId.None) onDisconnect?.Invoke(id);
            };
            
            listener.NetworkReceiveEvent += (peer, reader, method) => {
                
                
                var type = reader.GetInt();
                do 
                {
                    if(!NeedCallbacks(type, out var list)) break;
                    var id = NetId.None;
                    if(peer != peerToServer && !NeedId(peer, out id)) break;
                    foreach(var callback in list)
                    {
                        var tempReader = new NetDataReader(reader.RawData, reader.UserDataOffset + 4, reader.UserDataSize - 4);  // 去掉开头的 type 字段.
                        callback(id, tempReader, method);
                    }
                } while(false);
                reader.Recycle();
            };
            
            listener.NetworkReceiveUnconnectedEvent += (endpoint, reader, method) => {
                Log.Info($"从 { endpoint } 收到了未知的消息.");
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
        
        
        bool NeedCallbacks(int type, out List<NetCallback> list)
        {
            callbacks.GetOrCreate(type, out list);
            
            if(list.Count == 0)
            {
                Log.Warning("收到未处理的数据包类型 " + type);
                return false;
            }
            return true;
        }
        
        bool NeedId(NetPeer peer, out NetId id)
        {
            id = NetId.None;
            var sentBy = serverConnections.Find(x => peer.EndPoint.Equals(x.endpoint));
            if(sentBy == null)
            {
                Log.Warning($"收到了来自 { peer.EndPoint } 的数据包, 该数据包不在服务器的连接列表里, 但是在本机连接列表里.");
                return false;
            }
            id = sentBy.id;
            return true;
        }
        
        // ============================================================================================================
        // Operation
        // ============================================================================================================
        
        public void ConnectToServer()
        {
            DisconnectToServer();
            peerToServer = client.Connect(serverEndpoint, authorizedKey);
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
        void SendToPeer(NetPeer peer, Action<NetDataWriter> f, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            lock(writer)
            {
                writer.Reset();
                f(writer);
                peer.Send(writer, deliveryMethod);
            }
        }
        
        public void SendToClient(NetId key, Action<NetDataWriter> f, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            // 找到已有的连接.
            var clientInfo = serverConnections.Find(x => x.id == key);
            if(clientInfo == null)
            {
                Log.Error($"找不到编号为 { key.id } 的客户端, 跳过数据发送.");
            }
            
            var peer = connectedPeers.Find(x => x.EndPoint.Equals(clientInfo.endpoint));
            if(peer != null)
            {
                SendToPeer(peer, f, deliveryMethod);
                return;
            }
            
            SendToServer(w => {
                // 告诉服务器需要转发.
                w.Put(BuiltinMsgId.C2CSendByServer);
                // 告诉服务器转发到哪里.
                w.Put(key.id);
                // 普通数据.
                f(w);
            }, deliveryMethod);
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
            AddCallback(BuiltinMsgId.S2CNotifyInfo, (_, reader, method) => {
                myId = new NetId(reader.GetLong());
                myEndpoint = reader.GetNetEndPoint();
            });
            
            // 服务器告知有新的连接加入/退出.
            AddCallback(BuiltinMsgId.S2CAddOrRemoveClient, (_, reader, method) => {
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
            AddCallback(BuiltinMsgId.S2CResponseP2PConnection, (_, reader, method) => {
                var otherEndpoint = reader.GetNetEndPoint();
                Log.Info($"[内网穿透]服务器告知需要连接 [{ otherEndpoint }]");
                client.Connect(otherEndpoint, "ProtaClient");
            });
            
            AddCallback(BuiltinMsgId.S2CNotifyHost, (_, reader, method) => {
                var id = reader.GetLong();
                var clientInfo = serverConnections.Find(x => x.id == id);
                if(clientInfo == null)
                {
                    Log.Info($"从服务器收到了客户端[{ id }]变更为主机通知, 但是找不到这个客户端.");
                    return;
                }
                
                clientInfo.isHost = reader.GetBool();
            });
            
            AddCallback(BuiltinMsgId.C2CSendByServer, (id, reader, method) => {
                var fromId = new NetId(reader.GetLong());
                var type = reader.GetInt();     // 实际类型.
                if(!NeedCallbacks(type, out var list)) return;
                foreach(var callback in list)
                {
                    // 去掉 id 和 type 字段.
                    var tempReader = new NetDataReader(reader.RawData, reader.UserDataOffset + 12, reader.RawDataSize);
                    callback(fromId, tempReader, method);
                }
            });
        }
        
        // ============================================================================================================
        // 通用
        // ============================================================================================================
        
        bool NeedClientInfo(IPEndPoint endpoint, out ClientInfo info)
        {
            info = serverConnections.Find(x => x.endpoint.Equals(endpoint));
            return info == null; 
        }
        
        bool NeedPeerId(NetPeer peer, out NetId id)
        {
            if(!PeerToId(peer, out id))
            {
                Log.Warning($"连接节点 { peer.EndPoint } 没有对应的 id.");
                return false;
            }
            return true;
        }
        
        bool PeerToId(NetPeer peer, out NetId id)
        {
            var clientInfo = serverConnections.Find(x => x.endpoint.Equals(peer.EndPoint));
            id = clientInfo?.id ?? NetId.None;
            return clientInfo != null;
        }
        
        
        bool EndpointToId(IPEndPoint endpoint, out NetId id)
        {
            var clientInfo = serverConnections.Find(x => x.endpoint.Equals(endpoint));
            id = clientInfo.id;
            return clientInfo != null;
        }
        
    }
}