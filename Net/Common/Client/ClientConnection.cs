using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;

namespace Prota.Net
{
    // ====================================================================================================
    // 管理与服务器建立的连接.
    // ====================================================================================================
    public class ClientConnection : IDisposable
    {
        public const string defaultKey = "ProtaClient";
        
        public readonly string connectionKey;
        
        public NetId id { get; private set; }
        
        public NetPeer peer { get; private set; }       // 到服务器的连接.
        
        public int latency { get; private set; }
        
        public readonly EventBasedNetListener listener;
        
        public readonly NetManager mgr;
        
        public Action onDisconnect;
        
        readonly CancellationTokenSource cancelSource = new CancellationTokenSource();
        
        readonly EventBasedNetListener.OnNetworkReceive onReceive;
        
        public ClientConnection(EventBasedNetListener.OnNetworkReceive onReceive)
        {
            this.listener = new EventBasedNetListener();
            listener.PeerConnectedEvent += PeerConnectedEvent;
            listener.PeerDisconnectedEvent += PeerDisconnectEvent;
            listener.NetworkLatencyUpdateEvent += UpdateLatency;
            listener.NetworkReceiveUnconnectedEvent += ReceiveUnconnectedEvent;

            // 临时的 receive event. 当收到 id 协议后会改换成参数中的 onReceive.
            listener.NetworkReceiveEvent += OnIdReceive;
            this.onReceive = onReceive;
            
            mgr = new NetManager(listener);
        }
        
        public void Start()
        {
            bool success = mgr.Start();
            Console.WriteLine($"[Info] Prota client start [{ (success ? "success" : "fail") }] at port { mgr.LocalPort }");
        }
        
        public Task ConnectToServer(IPEndPoint endpoint, string connectionKey = defaultKey)
        {
            mgr.Connect(endpoint, connectionKey);
            
            // 等待连接成功.
            return Task.Run(async () => {
                while(peer == null || this.id == NetId.none)
                {
                    // Console.WriteLine($"waiting... { peer != null }");
                    await Task.Delay(Client.threadCheckDelay, cancelSource.Token);
                }
            }, cancelSource.Token);
        }
        
        public void UpdateLatency(NetPeer peer, int latency) => this.latency = latency;
        
        public void OnIdReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod method)
        {
            var header = reader.GetProtaSerialized<CommonHeader>();
            if(header.protoId == typeof(S2CNtfClientId).GetProtocolId())
            {
                this.id = header.dst;
                listener.NetworkReceiveEvent -= OnIdReceive;
                listener.NetworkReceiveEvent += onReceive;
                return;
            }
            Console.WriteLine($"[Warn] Prota client receive packege [{ header.protoId }] before id is set is not allowed.");
        }
        
        void PeerConnectedEvent(NetPeer peer)
        {
            this.peer = peer;
            Console.WriteLine($"[Info] Prota client connected to server [{ peer.EndPoint }]");
        }
        
        void PeerDisconnectEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            this.peer = null;
            onDisconnect?.Invoke();
            Console.WriteLine($"[Info] Prota client disconnect to server [{ peer.EndPoint }]");
        }
        
        void ReceiveUnconnectedEvent(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine($"[Error] cannot deal with unconnected message. [{ remoteEndPoint }]");
        }

        public void Dispose()
        {
            cancelSource.Cancel();
            mgr.DisconnectAll();
            
        }
    }
}