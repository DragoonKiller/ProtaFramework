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
            Console.WriteLine($"[Info] { id } Enter room { roomId }");
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
        
        
        
    public class Server
    {
        readonly EventBasedNetListener listener;
        
        readonly NetManager mgr;
        
        readonly int maxConnection;
        
        readonly PairDictionary<NetId, NetPeer> peers = new PairDictionary<NetId, NetPeer>();
        
        readonly ServerRoom rooms = new ServerRoom();
        
        readonly NetIdPool idPool;
        
        public int latency { get; private set; }
        
        public string accpetKey = "ProtaClient";
        
        [ThreadStatic]
        public static NetDataWriter _writer;
        static NetDataWriter writer => _writer == null ? _writer = new NetDataWriter() : _writer;
        
        public Server(int port, int maxConnection)
        {
            NetId.ValidRange(maxConnection).Assert();
            
            this.maxConnection = maxConnection;
            
            idPool = new NetIdPool(maxConnection);
            
            listener = new EventBasedNetListener();
            mgr = new NetManager(listener);
            
            listener.ConnectionRequestEvent += ConnectionRequestEvent;
            listener.PeerConnectedEvent += PeerConnectedEvent;
            listener.PeerDisconnectedEvent += PeerDisconnectEvent;
            listener.NetworkReceiveUnconnectedEvent += ReceiveUnconnectedEvent;
            listener.NetworkLatencyUpdateEvent += UpdateLatency;
            listener.NetworkReceiveEvent += Receive;
            mgr.UnsyncedEvents = true;
            mgr.Start(port);
        }
        
        void Receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            lock(this)
            {
                var header = reader.GetCommonHeader();
                
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
                
                // ====================================================================================================
                // 客户端请求已连接客户端列表.
                // ====================================================================================================
                if(header.protoId == ProtoId.C2SReqClientList)
                {
                    writer.Reset();
                    var newHeader = new CommonHeader(header.seq.response, NetId.none, peers.GetKeyByValue(peer), ProtoId.S2CRspClientList);
                    writer.WriteHeader(newHeader);
                    writer.Put(peers.Count);
                    foreach(var (netId, p) in peers)
                    {
                        var roomValid = rooms.roomMap.TryGetValue(netId, out var roomId);
                        writer.Put(netId.id);
                        writer.Put(roomId);
                        writer.Put(roomValid);
                    }
                    peer.Send(writer, deliveryMethod);
                    return;
                }
                
                // ====================================================================================================
                // 客户端请求加入房间.
                // ====================================================================================================
                if(header.protoId == ProtoId.C2SReqEnterRoom)
                {
                    if(!header.dst.isNone)
                    {
                        header.Error($"this protocol should have dst == null");
                        return;
                    }
                    
                    var roomId = reader.GetInt();
                    
                    if(!rooms.TryEnterRoom(header.src, roomId))
                    {
                        // 报告失败.
                        writer.Reset();
                        writer.WriteHeader(new CommonHeader(header.seq.response, NetId.none, header.src, ProtoId.S2CRspExitRoom));
                        writer.Put(S2CRspEnterRoom.fail);
                        peer.Send(writer, deliveryMethod);
                    }
                    
                    // 向其它客户端报告有人加入房间了. 注意客户端信息填在源id里, 只需要发送一个 header 就好了.
                    foreach(var playerId in rooms[roomId])
                    {
                        if(playerId == header.src) continue;        // 不会发送给自己.
                        var dstPeer = peers[playerId];
                        writer.Reset();
                        writer.WriteHeader(new CommonHeader(NetSequenceId.notify, header.src, peers.GetKeyByValue(dstPeer), ProtoId.S2CNtfOtherEnterExitRoom));
                        writer.Put(new S2CNtfOtherEnterExitRoom(true));
                        dstPeer.Send(writer, deliveryMethod);
                    }
                    
                    // 向请求加入房间的客户端报告成功. 另外要报告该房间内其他人.
                    writer.Reset();
                    writer.WriteHeader(new CommonHeader(header.seq.response, NetId.none, header.src, ProtoId.S2CRspEnterRoom));
                    var room = rooms[roomId];
                    writer.Put(new S2CRspEnterRoom(roomId, room.ToArray()));
                    peer.Send(writer, deliveryMethod);
                    
                    return;
                }
                
                // ====================================================================================================
                // // 客户端请求退出房间.
                // ====================================================================================================
                if(header.protoId == ProtoId.C2SReqExitRoom)
                {
                    if(!header.dst.isNone)
                    {
                        header.Error($"this protocol should have dst == null");
                        // 报告失败.
                        writer.Reset();
                        writer.WriteHeader(new CommonHeader(header.seq.response, NetId.none, header.src, ProtoId.S2CRspExitRoom));
                        writer.Put(new S2CRspExitRoom(false));
                        peer.Send(writer, deliveryMethod);
                        return;
                    }
                    
                    if(!rooms.TryGetRoom(header.src, out int room))
                    {
                        header.Error($"client is not in a room.");
                        // 报告失败.
                        writer.Reset();
                        writer.WriteHeader(new CommonHeader(header.seq.response, NetId.none, header.src, ProtoId.S2CRspExitRoom));
                        writer.Put(new S2CRspExitRoom(false));
                        peer.Send(writer, deliveryMethod);
                        return;
                    }
                    
                    if(!rooms.TryLeaveRoom(header.src))     // 报告失败.
                    {
                        writer.Reset();
                        writer.WriteHeader(new CommonHeader(header.seq.response, NetId.none, header.src, ProtoId.S2CRspExitRoom));
                        writer.Put(new S2CRspExitRoom(false));
                        peer.Send(writer, deliveryMethod);
                        return;
                    }
                    
                    // 向其它客户端报告有人退出房间了. 注意客户端信息填在源id里, 只需要发送一个 header 就好了.
                    var roomId = rooms.roomMap[header.src];
                    foreach(var playerId in rooms[roomId])
                    {
                        if(playerId == header.src) continue;        // 不会发送给自己.
                        var dstPeer = peers[playerId];
                        writer.Reset();
                        writer.WriteHeader(new CommonHeader(NetSequenceId.notify, header.src, peers.GetKeyByValue(dstPeer), ProtoId.S2CNtfOtherEnterExitRoom));
                        writer.Put(new S2CNtfOtherEnterExitRoom(false));
                        dstPeer.Send(writer, deliveryMethod);
                    }
                    
                    // 报告成功.
                    writer.Reset();
                    writer.WriteHeader(new CommonHeader(header.seq.response, NetId.none, header.src, ProtoId.S2CRspExitRoom));
                    writer.Put(new S2CRspExitRoom(true));
                    peer.Send(writer, deliveryMethod);
                    return;
                }
                
                // ====================================================================================================
                // 客户端请求数据转发给另一个客户端.
                // ====================================================================================================
                if(!header.dst.isNone)
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
                if(header.dst.isNone)
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
                        writer.WriteHeader(new CommonHeader(header.seq, header.src, peers.GetKeyByValue(dstPeer), header.protoId));
                        writer.Put(reader.RawData, reader.UserDataOffset + CommonHeader.size, reader.UserDataSize - CommonHeader.size);
                        dstPeer.Send(writer, deliveryMethod);
                    }
                
                    return;    
                }
                
                
                header.Error($"unknown delivery pattern");
            }
        }
        
        void ConnectionRequestEvent(ConnectionRequest request)
        {
            lock(this)
            {
                if(idPool.usableCount <= 0) request.Reject();
                
                request.AcceptIfKey("accpetKey");
            }
        }

        void PeerConnectedEvent(NetPeer peer)
        {
            lock(this)
            {
                peers.Add(idPool.Get(), peer);
            }
        }

        void PeerDisconnectEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            lock(this)
            {
                var id = peers.GetKeyByValue(peer);
                peers.Remove(id);
                idPool.Return(id);
            }
        }

        void ReceiveUnconnectedEvent(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine($"[Error] cannot deal with unconnected message. [{ remoteEndPoint }]");
        }

        void UpdateLatency(NetPeer peer, int latency) => this.latency = latency;
        
    }
}