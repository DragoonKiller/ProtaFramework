using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using XLua;
using System.Net;

using Prota.Lua;

namespace Prota.Net
{
    public class ClientBase : MonoBehaviour
    {
        [Serializable]
        public class Info
        {
            public string local;
            public string server;
            
            public string endpoint;
            
        }
        
        readonly static IPEndPoint gateEndpoint = new IPEndPoint(IPAddress.Parse("106.54.195.7"), 33200);
        
        protected NetManager client;
        
        protected NetPeer peerToServer;
        
        protected EventBasedNetListener listener { get; private set; }
        
        protected virtual string authorizedKey => "ProtaClient";
        
        [SerializeField]
        public Info info;
        
        public long id;
        
        public IPEndPoint myEndpoint;
            
        public NetDataWriter writer = new NetDataWriter();
        
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
                if(!peer.EndPoint.Equals(gateEndpoint)) return;
                Log.Info($"{ this } 连接服务器成功 { peer.EndPoint }");
                info.server = peer.EndPoint.ToString();
                peerToServer = peer;
                SendToServer(w => {
                    w.Put(100);
                    w.Put(this.GetInstanceID());
                    w.Put(this.ToString());
                });
            };
            
            listener.PeerDisconnectedEvent += (peer, info) => {
                if(!peer.EndPoint.Equals(gateEndpoint)) return;
                Log.Info($"{ this } 断开与服务器的连接: { peer.EndPoint }");
                this.info.server = "";
                return;
            };
            
            listener.NetworkReceiveEvent += (peer, reader, method) => {
                var type = reader.GetInt();
                switch(type)
                {
                    case -1:
                    id = reader.GetLong();
                    myEndpoint = reader.GetNetEndPoint();
                    info.endpoint = myEndpoint.ToString();
                    break;
                    
                    default:
                    OnReceiveData(type, reader);
                    break;
                }
            };
        }
        
        protected virtual void OnReceiveData(int type, NetDataReader reader)
        {
            
        }
        
        protected virtual void Update()
        {
            client.PollEvents();
        }
        
        protected virtual void OnDestroy()
        {
            client.Stop();
        }
        
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
        
        public void SendToServerOrdered(Action<NetDataWriter> f)
        {
            SendToServer(f, DeliveryMethod.ReliableOrdered);
        }
        
        public void SendToServer(Action<NetDataWriter> f, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableUnordered)
        {
            lock(writer)
            {
                writer.Reset();
                f(writer);
                peerToServer.Send(writer, deliveryMethod);
            }
        }
    }
}