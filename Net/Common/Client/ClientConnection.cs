using System;
using System.Net;
using System.Threading.Tasks;
using LiteNetLib;

namespace Prota.Net
{
    // ====================================================================================================
    // 管理与服务器建立的连接.
    // ====================================================================================================
    public class ClientConnection
    {
        public const string defaultKey = "ProtaClient";
        
        public readonly string connectionKey;
        
        public NetId id { get; private set; }
        
        public NetPeer peer { get; private set; }       // 到服务器的连接.
        
        public int latency { get; private set; }
        
        public readonly EventBasedNetListener listener;
        
        public readonly NetManager mgr;
        
        public Action onDisconnect;
        
        public ClientConnection() => mgr = new NetManager(this.listener = new EventBasedNetListener());
        
        public void RegisterCallbacks(EventBasedNetListener.OnNetworkReceive onReceive)
        {
            listener.PeerConnectedEvent += PeerConnectedEvent;
            listener.PeerDisconnectedEvent += PeerDisconnectEvent;
            listener.NetworkLatencyUpdateEvent += UpdateLatency;
            listener.NetworkReceiveUnconnectedEvent += ReceiveUnconnectedEvent;
            listener.NetworkReceiveEvent += onReceive;
        }
        
        public void Start() => mgr.Start();
        
        public async Task ConnectToServer(IPEndPoint endpoint, string connectionKey = defaultKey)
        {
            mgr.Connect(endpoint, connectionKey);
            
            // 等待连接成功.
            await Task.Run(async () => {
                while(peer != null) await Task.Delay(Client.threadCheckDelay);
            });
        }
        
        public void UpdateLatency(NetPeer peer, int latency) => this.latency = latency;
        
        void PeerConnectedEvent(NetPeer peer) => this.peer = peer;
        
        void PeerDisconnectEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            this.peer = null;
            onDisconnect?.Invoke();
        }
        
        void ReceiveUnconnectedEvent(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine($"[Error] cannot deal with unconnected message. [{ remoteEndPoint }]");
        }
    }
}