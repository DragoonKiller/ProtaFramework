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
    public class Client
    {
        delegate void OnSend(CommonHeader headear, NetDataWriter writer);
        delegate void OnRecieve(CommonHeader header, NetDataReader reader);
        
        
        public static Client local;
        
        public NetId id { get; private set; }
        
        EventBasedNetListener listener;
        
        NetManager mgr;
        
        public NetPeer peer = null;
        
        public HashSet<NetId> peers = new HashSet<NetId>();
        
        // 创建该对象的 context 是该对象的从属 context.
        // 所有异步方法会返回主线程执行.
        public SynchronizationContext context { get; private set; }
        
        public string connectionKey = "ProtaClient";
        
        public int latency { get; private set; }
        
        
        
        // 序列号的滑动区间. 序列号取值是 ushort, 范围是 1 ~ maxSeq. 序列号 0 表示没有序列号.
        CircleDualPointer pointers;
        Dictionary<NetSequenceId, OnRecieve> pendings = new Dictionary<NetSequenceId, OnRecieve>();
        
        
        
        public Client(bool setAsMain = true, int maxSeq = 100000)
        {
            if(setAsMain) local = this;
            this.pointers = new CircleDualPointer(maxSeq);
            
            listener = new EventBasedNetListener();
            mgr = new NetManager(listener);
            
            listener.PeerConnectedEvent += PeerConnectedEvent;
            listener.PeerDisconnectedEvent += PeerDisconnectEvent;
            listener.NetworkReceiveUnconnectedEvent += ReceiveUnconnectedEvent;
            listener.NetworkLatencyUpdateEvent += UpdateLatency;
            listener.NetworkReceiveEvent += Receive;
            
            context = SynchronizationContext.Current;
            
            mgr.Start();
        }
        
        public async void ConnectToServer(IPEndPoint endpoint)
        {
            mgr.Connect(endpoint, connectionKey);
            
            // 等待连接成功.
            await Task.Run(async () => {
                while(peer != null) await Task.Delay(50);
            });
            
            // 回主线程.
            await context;
        }
        
        void PeerConnectedEvent(NetPeer peer)
        {
            this.peer = peer;
        }
        
        void PeerDisconnectEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            this.peer = null;
        }
        
        void ReceiveUnconnectedEvent(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine($"[Error] cannot deal with unconnected message. [{ remoteEndPoint }]");
        }
        
        // 到服务器的延迟.
        void UpdateLatency(NetPeer peer, int latency)
        {
            this.latency = latency;
        }
        
        void Receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var header = reader.GetCommonHeader();
            var s = header.seq;
            
            if(s == 0)      // 这个消息是 notify.
            {
                
            }
            else
            {
                if(s < 0)           // 这个消息是 response.
                {
                    if(!pendings.TryGetValue(s, out var frsp))
                    {
                        header.Error($"{ this.id } a response with sequence[{ s }] received, but it is not listening.");
                        return;
                    }
                    
                    frsp(header, reader);
                    pendings.Remove(s);
                    for(int i = 0; i < pendings.Count; i++)
                    {
                        if(pointers.count == 0) break;
                        
                        // 没有这个元素说明已经执行过了, 可以增加计数.
                        if(!pendings.TryGetValue(new NetSequenceId(pointers[0]), out _))
                        {
                            pointers.FrontMoveNext();
                        }
                    }
                }
                else               // 这个消息是 request.
                {
                    
                }
                
            }
            
        }
        
        void Send(NetId target, int protoId, OnSend freq, OnRecieve frsp)
        {
            if(pendings.Count > 30000) Console.Write($"[Error] Too many pending requests!!!");
            
            var writer = new NetDataWriter();
            var seq = new NetSequenceId(pointers.BackMoveNext());
            var header = new CommonHeader(new NetSequenceId(seq), this.id, target, protoId);
            writer.WriteHeader(header);
            freq(header, writer);
            if(frsp == null)
            {
                pendings.Add(seq, frsp);
            }
        }
        
        void PollEvents() => mgr.PollEvents();
    }
}