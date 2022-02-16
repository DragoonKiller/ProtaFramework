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
    public struct NetPeerKey
    {
        public readonly IPEndPoint endpoint;

        public NetPeerKey(IPEndPoint endpoint)
        {
            this.endpoint = endpoint;
        }
    }
    
    public class ClientBase : MonoBehaviour
    {
        [Serializable]
        public struct ClientInfo
        {
            public long id;
            public string ip;
            public int port;
        }
        
        [Serializable]
        public class Info
        {
            public string local;
            public string server;
            
            public string myEndpoint;
            
            public float pingToServer;
        
        }
        
        readonly static IPEndPoint gateEndpoint = new IPEndPoint(IPAddress.Parse("106.54.195.7"), 33200);
        
        protected NetManager client;
        
        protected NetPeer peerToServer;
        
        protected EventBasedNetListener listener { get; private set; }
        
        protected virtual string authorizedKey => "ProtaClient";
        
        [Header("=== Base ===")]
        [SerializeField]
        public Info info;
        
        public long id;
        
        public IPEndPoint myEndpoint;
        
        public Dictionary<int, List<NetCallback>> callbacks = new Dictionary<int, List<NetCallback>>();
        
        [SerializeField]
        public List<string> connectedPeers = new List<string>();
        
        [SerializeField]
        public List<ClientInfo> serverConnections = new List<ClientInfo>();
        
        protected virtual void Start()
        {
            listener = new EventBasedNetListener();
            client = new NetManager(listener);
            client.Start();
            ConnectToServer();
            
            info.local = ":" + client.LocalPort;
            
            listener.ConnectionRequestEvent += req => {
                Log.Info($"{ this } 接收到了建立连接请求 { req.RemoteEndPoint } 数据: [{ req.Data.GetString() }]");
                // 客户端可以无脑接收.
                req.Accept();
            };
            
            listener.PeerConnectedEvent += peer => {
                if(peer.EndPoint.Equals(gateEndpoint))
                {
                    Log.Info($"{ this } 连接服务器成功 { peer.EndPoint }");
                    info.server = peer.EndPoint.ToString();
                    peerToServer = peer;
                    SendToServer(w => {
                        w.Put(-10);
                        w.Put(this.GetInstanceID());
                        w.Put(this.ToString());
                    });
                }
                
                connectedPeers.Add(peer.EndPoint.ToString());
            };
            
            listener.PeerDisconnectedEvent += (peer, info) => {
                if(!peer.EndPoint.Equals(gateEndpoint)) return;
                Log.Info($"{ this } 断开与服务器的连接: { peer.EndPoint }");
                this.info.server = "";
                connectedPeers.Remove(peer.EndPoint.ToString());
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
            info.pingToServer = peerToServer?.Ping ?? -1;
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
        
        public NetPeerKey ConnectTo(IPEndPoint endpoint)
        {
            client.Connect(endpoint, authorizedKey);
            return new NetPeerKey(endpoint);
        }
        
        public NetPeerKey ConnectTo(string endpoint)
        {
            var parts = endpoint.Split(":");
            if(parts.Length != 2) throw new Exception("Invalid endpoint " + endpoint);
            var ipEndpoint = LiteNetLib.NetUtils.MakeEndPoint(parts[0], int.Parse(parts[1]));
            return ConnectTo(ipEndpoint);
        } 
        
        void DisconnectTo(NetPeer peer)
        {
            peer.Disconnect();
        }
        
        public void DisconnectTo(NetPeerKey key)
        {
            var target = client.ConnectedPeerList.Find(x => x.EndPoint.Equals(key.endpoint));
            target?.Disconnect();
        }
        
        public void DisconnectTo(IPEndPoint endpoint)
        {
            var target = client.ConnectedPeerList.Find(x => x.EndPoint.Equals(endpoint));
            target?.Disconnect();
        }
        
        // 通过服务器, 建立 p2p 连接.
        public void ConnectClientByServer(int otherClientId)
        {
            var clientInfo = serverConnections.Find(x => x.id == otherClientId);
            if(string.IsNullOrEmpty(clientInfo.ip)) throw new Exception("serverId not found!");
            var ipAddress = LiteNetLib.NetUtils.MakeEndPoint(clientInfo.ip, clientInfo.port);
            SendToServer(w => {
                w.Put(-101);
                w.Put(id);
                w.Put(otherClientId);
            });
        }
        
        // ============================================================================================================
        // Send
        // ============================================================================================================
        
        public void SendToServerOrdered(Action<NetDataWriter> f)
        {
            SendToPeerOrdered(peerToServer, f);
        }
        
        public void SendToServer(Action<NetDataWriter> f, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableUnordered)
        {
            SendToPeer(peerToServer, f, deliveryMethod);
        }
        
        public NetDataWriter writer = new NetDataWriter();
        public void SendToPeer(NetPeer peer, Action<NetDataWriter> f, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableUnordered)
        {
            lock(writer)
            {
                writer.Reset();
                f(writer);
                peerToServer.Send(writer, deliveryMethod);
            }
        }
        
        public void SendToPeerOrdered(NetPeer peer, Action<NetDataWriter> f)
        {
            SendToPeer(peer, f, DeliveryMethod.ReliableOrdered);
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
            AddCallback(-1, (peer, reader, method) => {
                id = reader.GetLong();
                myEndpoint = reader.GetNetEndPoint();
                info.myEndpoint = myEndpoint.ToString();
            });
            
            // 服务器告知有新的连接加入/退出.
            AddCallback(-2, (peer, reader, method) => {
                var op = reader.GetInt();
                var id = reader.GetLong();           // 服务器一侧保存的 id.
                var endpoint = reader.GetNetEndPoint();
                if(op == 1)     // 加入.
                {
                    serverConnections.Add(new ClientInfo() {
                        id = id,
                        ip = endpoint.Address.ToString(),
                        port = endpoint.Port,
                    });
                }
                else            // 退出.
                {
                    serverConnections.RemoveAll(x => x.id == id);
                }
            });
            
            // 建立 p2p 连接. 第三步; 连接到目标端点.
            AddCallback(-101, (peer, reader, method) => {
                var otherEndpoint = reader.GetNetEndPoint();
                client.Connect(otherEndpoint, "ProtaClient");
            });
            
            
        }
        
        
    }
}