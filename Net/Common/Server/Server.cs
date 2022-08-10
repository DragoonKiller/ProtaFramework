using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Prota.Net
{
    public class ServerRoom
    {
        public readonly Dictionary<NetId, int> roomMap = new Dictionary<NetId, int>();
        
        public readonly Dictionary<int, HashSet<NetId>> usedRoom = new Dictionary<int, HashSet<NetId>>();
        
        public int roomCount => usedRoom.Count;
        
        public HashSet<NetId> this[int roomId] => usedRoom.TryGetValue(roomId, out var res) ? res : null;
        
        public bool TryGetRoom(NetId id, out int room) => roomMap.TryGetValue(id, out room);
        
        public bool TryEnterRoom(NetId id, int roomId)
        {
            if(roomMap.ContainsKey(id)) return false;
            usedRoom.GetOrCreate(roomId, out var playersInRoom);
            playersInRoom.Add(id);
            roomMap[id] = roomId;
            Console.WriteLine($"[Info] Prota server { id } Enter room { roomId }");
            return true;
        }
        
        public bool TryLeaveRoom(NetId id)
        {
            if(!roomMap.TryGetValue(id, out var roomId)) return false;
            roomMap.Remove(id);
            var playersInRoom = usedRoom[roomId];
            playersInRoom.Remove(id);
            if(playersInRoom.Count == 0) usedRoom.Remove(roomId);
            Console.WriteLine($"[Info] { id } Leave room { roomId }");
            return true;
        }
    }
        
        
        
    public class Server : IDisposable
    {
        readonly EventBasedNetListener listener;
        
        readonly NetManager mgr;
        
        readonly int maxConnection;
        
        readonly PairDictionary<NetId, NetPeer> peers = new PairDictionary<NetId, NetPeer>();
        
        readonly ServerRoom rooms = new ServerRoom();
        
        readonly NetIdPool idPool;
        
        public int latency { get; private set; }
        
        public string accpetKey = ClientConnection.defaultKey;
        
        [ThreadStatic]
        public static NetDataWriter _writer;
        static NetDataWriter writer => _writer == null ? _writer = new NetDataWriter() : _writer;
        
        public NetCallbackManager callbackList = new NetCallbackManager();
        
        public Server(int port, int maxConnection)
        {
            NetId.ValidRange(maxConnection).Assert();
            
            this.maxConnection = maxConnection;
            
            idPool = new NetIdPool(maxConnection);
            
            callbackList.AddProcessor<C2SReqEnterRoom>(PlayerReqEnterRoom);
            callbackList.AddProcessor<C2SReqExitRoom>(PlayerReqExitRoom);
            
            listener = new EventBasedNetListener();
            listener.ConnectionRequestEvent += ConnectionRequestEvent;
            listener.PeerConnectedEvent += PeerConnectedEvent;
            listener.PeerDisconnectedEvent += PeerDisconnectEvent;
            listener.NetworkReceiveUnconnectedEvent += ReceiveUnconnectedEvent;
            listener.NetworkLatencyUpdateEvent += UpdateLatency;
            listener.NetworkReceiveEvent += Receive;
            mgr = new NetManager(listener);
            mgr.UnsyncedEvents = true;
            bool success = mgr.Start(port);
            Console.WriteLine($"[Info] Prota server start [{ (success ? "success" : "fail") }] at port { mgr.LocalPort }");
        }
        
        
        
        void Receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            lock(this)
            {
                var header = reader.GetProtaSerialized<CommonHeader>();
                Console.WriteLine($"[Info] Prota server Receive from [{ peer.EndPoint }]{ header.src } protoId [{ header.protoId }] seq [{ header.seq }]");
                
                // 检查发送方是否还连着.
                if(!peers.TryGetValue(header.src, out var srcPeer))
                {
                    header.Error($"source invalid");
                    return;
                }
                
                
                // 检查发送方是否是真正的发送方.
                if(srcPeer != peer)
                {
                    header.Error($"source validation fail, actual source: { peers.GetKeyByValue(peer) }");
                    return;
                }
                
                // 服务器注册了专用数据结构的协议比较特殊, 优先处理.
                if(this.callbackList.IsListening(header.protoId))
                {
                    reader.SkipBytes(-(reader.Position - reader.UserDataOffset));       // 复位到 UserDataOffset
                    this.callbackList.Receive(peer, reader, deliveryMethod);
                }
                else
                {
                    if(header.dst.isNone) BroadcastMessage(header, reader, deliveryMethod);
                    else ResendMessage(header, reader, deliveryMethod);
                }
            }
        }
        
        // ====================================================================================================
        // 客户端请求数据转发给另一个客户端.
        // ====================================================================================================
        void ResendMessage(CommonHeader header, NetDataReader reader, DeliveryMethod deliveryMethod)
        {
            if(!peers.TryGetValue(header.dst, out var dstPeer))
            {
                header.Error($"destination not null but invalid");
                return;
            }
            
            dstPeer.Send(reader.RawData, reader.UserDataOffset, reader.UserDataSize, deliveryMethod);
            return;
        }
        
        // ====================================================================================================
        // 客户端想给房间内的人广播这条消息.
        // ====================================================================================================
        void BroadcastMessage(CommonHeader header, NetDataReader reader, DeliveryMethod deliveryMethod)
        {
            if(rooms.TryGetRoom(header.src, out var roomId))
            {
                header.Error($"doing boradcast needs to be in a room");
                return;
            }
            
            foreach(var playerId in rooms[roomId])
            {
                if(playerId == header.src) continue;        // 不会发送给自己.
                var dstPeer = peers[playerId];
                writer.Reset();
                writer.PutProtaSerialize(new CommonHeader(header.seq, header.src, peers.GetKeyByValue(dstPeer), header.protoId));
                writer.Put(reader.RawData, reader.UserDataOffset + CommonHeader.size, reader.UserDataSize - CommonHeader.size);
                dstPeer.Send(writer, deliveryMethod);
            }
        
            return;
        }
        
        // ====================================================================================================
        // 客户端请求加入房间.
        // ====================================================================================================
        void PlayerReqEnterRoom(CommonHeader header, C2SReqEnterRoom data)
        {
            if(!header.dst.isNone)
            {
                header.Error($"this protocol should have dst == null");
                return;
            }
            
            var peer = peers[header.src];
            
            if(!rooms.TryEnterRoom(header.src, data.roomId))
            {
                // 报告失败.
                SendDataWithWriter(header.seq.response, NetId.none, header.src, S2CRspEnterRoom.fail);
                return;
            }
            
            // 向其它客户端报告有人加入房间了. 注意客户端信息填在源id里, 只需要发送一个 header 就好了.
            foreach(var playerId in rooms[data.roomId])
            {
                if(playerId == header.src) continue;        // 不会发送给自己.
                SendDataWithWriter(NetSequenceId.notify, header.src, playerId, new S2CNtfOtherEnterExitRoom(true));
            }
            
            // 向请求加入房间的客户端报告成功. 另外要报告该房间内其他人.
            var value = new S2CRspEnterRoom(data.roomId, rooms[data.roomId].ToArray());
            SendDataWithWriter(header.seq.response, NetId.none, header.src, value);
            
            return;
        }
        
        
        // ====================================================================================================
        // 客户端请求退出房间.
        // ====================================================================================================
        void PlayerReqExitRoom(CommonHeader header, C2SReqExitRoom data)
        {
            var peer = peers[header.src];
            
            var protocolId = typeof(S2CNtfOtherEnterExitRoom).GetProtocolId();
            var deliveryMethod = typeof(S2CNtfOtherEnterExitRoom).GetProtocolMethod();
            
            if(!header.dst.isNone)
            {
                header.Error($"this protocol should have dst == null");
                SendDataWithWriter(header.seq.response, NetId.none, header.src, new S2CRspExitRoom(false));
                return;
            }
            
            
            if(!rooms.TryGetRoom(header.src, out int room))
            {
                header.Error($"client is not in a room.");
                SendDataWithWriter(header.seq.response, NetId.none, header.src, new S2CRspExitRoom(false));
                return;
            }
            
            if(!rooms.TryLeaveRoom(header.src))
            {
                header.Error("leaving room fail.");
                SendDataWithWriter(header.seq.response, NetId.none, header.src, new S2CRspExitRoom(false));
                return;
            }
            
            // 向其它客户端报告有人退出房间了. 注意客户端信息填在源id里, 只需要发送一个 header 就好了.
            var roomId = rooms.roomMap[header.src];
            foreach(var playerId in rooms[roomId])
            {
                if(playerId == header.src) continue;        // 不会发送给自己.
                SendDataWithWriter(NetSequenceId.notify, header.src, playerId, new S2CNtfOtherEnterExitRoom(false));
            }
            
            // 报告成功.
            SendDataWithWriter(header.seq.response, NetId.none, header.src, new S2CRspExitRoom(false));
            return;
        }
        
        
        void SendDataWithWriter<T>(NetSequenceId seq, NetId src, NetId dst, T data)
        {
            writer.Reset();
            writer.PutProtaSerialize(new CommonHeader(seq, src, dst, typeof(T).GetProtocolId()));
            writer.PutProtaSerialize(data);
            peers[dst].Send(writer, typeof(T).GetProtocolMethod());
        }
        
        void ConnectionRequestEvent(ConnectionRequest request)
        {
            lock(this)
            {
                Console.WriteLine($"[Info] Prota server get connection request from [{ request.RemoteEndPoint }]");
                if(idPool.usableCount <= 0)
                {
                    Console.WriteLine($"[Info] Prota server reject [{ request.RemoteEndPoint }] connection because there's no usable slot.");
                    request.Reject();
                }
                
                var peer = request.AcceptIfKey(accpetKey);
                if(peer == null)
                {
                    Console.WriteLine($"[Info] Prota server reject [{ request.RemoteEndPoint }] connection request because key is not correct.");
                }
                else
                {
                    Console.WriteLine($"[Info] Prota server accept [{ request.RemoteEndPoint }] connection request.");
                }
            }
        }

        void PeerConnectedEvent(NetPeer peer)
        {
            lock(this)
            {
                var id = idPool.Get();
                peers.Add(id, peer);
                SendDataWithWriter(NetSequenceId.notify, NetId.none, id, new S2CNtfClientId(id));
                Console.WriteLine($"[Info] Prota server connect to [{ peer.EndPoint }] established. Id { id }");
            }
        }

        void PeerDisconnectEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            lock(this)
            {
                var id = peers.GetKeyByValue(peer);
                if(rooms.roomMap.TryGetValue(id, out var roomId)) rooms.TryLeaveRoom(id);
                peers.Remove(id);
                idPool.Return(id);
                Console.WriteLine($"[Info] Prota server disconnect to [{ peer.EndPoint }].");
            }
        }

        void ReceiveUnconnectedEvent(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine($"[Error] cannot deal with unconnected message. [{ remoteEndPoint }]");
        }

        void UpdateLatency(NetPeer peer, int latency) => this.latency = latency;

        public void Dispose()
        {
            mgr.DisconnectAll();
        }
    }
}