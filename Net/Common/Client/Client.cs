using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Prota.Net
{
    // 管理房间.
    public class Room
    {
        public readonly int roomId;
        
        public readonly HashSet<NetId> players = new HashSet<NetId>();
        
        public Room(S2CRspEnterRoom room)
        {
            this.roomId = room.id;
            foreach(var s in room.players) players.Add(s);
        }
    }
    
    // 管理与服务器建立的连接.
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
    
    
    public class Client
    {
        public const int threadCheckDelay = 50;
        
        public delegate void OnSendFunction(CommonHeader headear, NetDataWriter writer);
        
        public delegate void OnReceiveFunction(CommonHeader header, NetDataReader reader);
        
        public static Client local;
        
        public Room room { get; private set; }
        
        public ClientConnection connection { get; private set; }
        
        // 自身的 NetId.
        public NetId id => connection?.id ?? NetId.none;
        
        // 序列号的滑动区间. 序列号取值是 ushort, 范围是 1 ~ maxSeq. 序列号 0 表示没有序列号.
        readonly CircleDualPointer pointers;
        
        readonly Dictionary<NetSequenceId, OnReceiveFunction> pendings = new Dictionary<NetSequenceId, OnReceiveFunction>();
        
        readonly Dictionary<int, List<OnReceiveFunction>> processOnReceive = new Dictionary<int, List<OnReceiveFunction>>();
        
        public Client(bool setAsMain = true, int maxSeq = 1000000)
        {
            if(setAsMain) local = this;
            this.pointers = new CircleDualPointer(maxSeq);
            connection = new ClientConnection();
            connection.RegisterCallbacks(Receive);
            connection.Start();
            connection.onDisconnect += () => room = null;
            
            AddCallback(ProtoId.S2CNtfOtherEnterExitRoom, PlayerEnterExitRoom);
        }
        
        public void PollEvents() => connection?.mgr?.PollEvents();
        
        public async Task ConnectToServer(IPEndPoint endpoint, string key = ClientConnection.defaultKey) => await connection.ConnectToServer(endpoint, key);
        
        // ====================================================================================================
        // 发送消息通用逻辑
        // ====================================================================================================
        
        // target 填对应 NetId 则发送给客户端; target 填 NetId.none 广播.
        // 一些服务器会特护处理的内置协议类型除外.
        public void SendNotify(NetId target, int protoId, OnSendFunction freq)
        {
            var writer = new NetDataWriter();
            var seq = new NetSequenceId(pointers.BackMoveNext() + 1);
            var header = new CommonHeader(NetSequenceId.notify, this.id, target, protoId);
            writer.WriteHeader(header);
            freq(header, writer);
        }
        
        // target 填对应 NetId 则发送给客户端; target 填 NetId.none 则广播.
        // 一些服务器会特护处理的内置协议类型除外, target 填 NetId.none 会由服务器处理.
        public void SendRequest(NetId target, int protoId, OnSendFunction freq, OnReceiveFunction frsp)
        {
            var writer = new NetDataWriter();
            var seq = new NetSequenceId(pointers.BackMoveNext() + 1);
            var header = new CommonHeader(new NetSequenceId(seq), this.id, target, protoId);
            writer.WriteHeader(header);
            freq(header, writer);
            pendings.Add(seq, frsp);
        }
        
        public async void SendRequest(NetId target, int protoId, OnSendFunction freq)
        {
            CommonHeader header = new CommonHeader();
            NetDataReader reader = null;
            int completed = 0;
            SendRequest(target, protoId, freq, (h, r) => {
                header = h;
                reader = r;
                Interlocked.Increment(ref completed);
            });
            while(completed == 0) await Task.Delay(threadCheckDelay);
        }
        
        // ====================================================================================================
        // 房间相关
        // ====================================================================================================
        
        public async Task<bool> EnterRoom(int roomId)
        {
            int completed = 0;
            int success = 0;
            SendRequest(NetId.none, ProtoId.C2SReqEnterRoom, (header, writer) => {
                writer.Put(roomId);
            }, (header, reader) => {
                Interlocked.Increment(ref completed);
                if(!reader.GetBool()) return;
                Interlocked.Increment(ref success);
                var roomInfo = reader.Get<S2CRspEnterRoom>();
                room = new Room(roomInfo);
            });
            while(completed == 0) await Task.Delay(threadCheckDelay);
            return success != 0;
        }
        
        void PlayerEnterExitRoom(CommonHeader header, NetDataReader reader)
        {
            var info = reader.Get<S2CNtfOtherEnterExitRoom>();
            if(info.isEnter)
            {
                room.AssertNotNull();
                room.players.Add(header.src);
            }
            else
            {
                // 自己已经退出了.
                if(room == null) return;
                room.players.Remove(header.src);
            }
        }
        
        
        
        // ====================================================================================================
        // 接收消息通用逻辑
        // ====================================================================================================
        
        public void AddCallback(int protoId, OnReceiveFunction f)
        {
            processOnReceive.GetOrCreate(protoId, out var list);
            list.Add(f);
        }
        
        void Receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            
            var header = reader.GetCommonHeader();
            var s = header.seq;
            
            // 这个消息是 notify.
            if(s == 0)
            {
                ExecuteCallback(header, reader);
                return;
            }
            
            // 这个消息是 response.
            if(s < 0)
            {
                var reqId = new NetSequenceId(-s);
                
                if(!pendings.TryGetValue(reqId, out var frsp))
                {
                    header.Error($"{ this.id } a response with sequence[{ s }] received, but it is not listening.");
                    return;
                }
                
                // 全局注册函数和 Response 注册函数都要执行.
                ExecuteCallback(header, reader);
                frsp(header, reader);
                
                pendings.Remove(reqId); // 注册时的时候是按照 request 序号注册.
                
                for(int i = 0; i < pendings.Count && pointers.count != 0; i++)
                {
                    // 没有这个元素说明已经执行过了, 可以增加计数.
                    if(!pendings.ContainsKey(new NetSequenceId(pointers[0]))) pointers.FrontMoveNext();
                }
                return;
            }
            
            // 这个消息是 request.
            ExecuteCallback(header, reader);
        }
        
        void ExecuteCallback(in CommonHeader header, in NetDataReader reader)
        {
            if(!processOnReceive.TryGetValue(header.protoId, out var list))
            {
                header.Error($"receiving packet of type [{ header.protoId }] not listening.");
                return;
            }
            
            foreach(var c in list) c(header, reader);
        }
    }
}