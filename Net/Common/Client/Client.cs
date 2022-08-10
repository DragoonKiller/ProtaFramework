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
    public class Client : IDisposable
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
    
        public const int threadCheckDelay = 50;
        
        public static Client local;
                
        // 自身的 NetId.
        public NetId id => connection?.id ?? NetId.none;
        
        public Room room { get; private set; }
        
        public ClientConnection connection { get; private set; }
        
        public NetCallbackManager callbackList { get; private set; } = new NetCallbackManager();
        
        readonly Dictionary<NetSequenceId, OnReceiveFunction> pendings = new Dictionary<NetSequenceId, OnReceiveFunction>();

        // 序列号的滑动区间. 序列号取值是 ushort, 范围是 1 ~ maxSeq. 序列号 0 表示没有序列号.
        readonly CircleDualPointer pointers;
        
        readonly CancellationTokenSource cancelSource = new CancellationTokenSource();
        
        public Client(bool setAsMain = true, int maxSeq = 1000000)
        {
            if(setAsMain) local = this;
            this.pointers = new CircleDualPointer(maxSeq);
            connection = new ClientConnection();
            connection.RegisterCallbacks(callbackList.Receive);
            connection.Start();
            connection.onDisconnect += () => room = null;
            AddCallback<S2CNtfOtherEnterExitRoom>(PlayerEnterExitRoom);
        }
        
        public void PollEvents() => connection?.mgr?.PollEvents();
        
        public async Task ConnectToServer(IPEndPoint endpoint, string key = ClientConnection.defaultKey) => await connection.ConnectToServer(endpoint, key);
        
        // ====================================================================================================
        // 发送消息通用逻辑
        // ====================================================================================================
        
        NetDataWriter NetWriterWithHeader(NetSequenceId seq, NetId target, int protoId)
        {
            var writer = new NetDataWriter();
            var header = new CommonHeader(NetSequenceId.notify, this.id, target, protoId);
            writer.Put(header);
            return writer;
        }
        
        // target 填对应 NetId 则发送给客户端; target 填 NetId.none 广播.
        // 一些服务器会特护处理的内置协议类型除外.
        public void SendNotify<T>(NetId target, T data)
        {
            if(connection?.peer == null)  throw new InvalidOperationException("Connect not initialized!");
            
            var writer = NetWriterWithHeader(NetSequenceId.notify, target, typeof(T).GetProtocolId());
            writer.PutProtaSerialize(data);
            
            connection.peer.Send(writer, typeof(T).GetProtocolMethod());
        }
        
        void SendRequest<T>(NetId target, T data, OnReceiveFunction frsp)
        {
            var seq = new NetSequenceId(pointers.BackMoveNext() + 1);
            var writer = NetWriterWithHeader(seq, target, typeof(T).GetProtocolId());
            writer.PutProtaSerialize(data);
            connection.peer.Send(writer, typeof(T).GetProtocolMethod());
            pendings.Add(seq, frsp);
        }
        
        public RawRequestResult Request<T>(NetId target, T data)
        {
            var res = new RawRequestResult() { cancellationToken = cancelSource.Token };
            SendRequest(target, data, res.OnResponse);
            return res;
        }
        
        // ====================================================================================================
        // 房间相关
        // ====================================================================================================
        
        public async Task<bool> EnterRoom(int roomId)
        {
            var res = await Request(NetId.none, new C2SReqEnterRoom(roomId)).ExpectResult<S2CRspEnterRoom>();
            if(!res.success) return false;
            lock(this) room = new Room(res);
            return true;
        }
        
        public async Task<bool> LeaveRoom()
        {
            var res = await Request(NetId.none, new C2SReqExitRoom()).ExpectResult<S2CRspExitRoom>();
            if(!res.success) return false;
            lock(this) room = null;
            return true;
        }
        
        void PlayerEnterExitRoom(CommonHeader header, S2CNtfOtherEnterExitRoom info)
        {
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
        
        public NetCallbackManager.CallbackHandle AddCallback<T>(ProcessFunction<T> f) => callbackList.AddProcessor<T>(f);
        
        void RemoveCallback(NetCallbackManager.CallbackHandle handle) => handle.Dispose();
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public void Dispose()
        {
            cancelSource.Cancel();
            room = null;
            callbackList = null;
            connection.Dispose();
            connection = null;
        }
    }
}