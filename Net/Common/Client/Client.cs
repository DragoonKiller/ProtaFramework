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
    public partial class Client : IDisposable
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
        
        internal NetCallbackManager callbackList { get; private set; } = new NetCallbackManager();
        
        ClientConnection connection { get; set; }
        
        // 序列号的滑动区间. 序列号取值是 ushort, 范围是 1 ~ maxSeq. 序列号 0 表示没有序列号.
        readonly CircleDualPointer pointers;
        
        readonly CancellationTokenSource cancelSource = new CancellationTokenSource();
        
        readonly object lockobj = new object();
        
        public Client(bool setAsMain = true, int maxSeq = 1000000)
        {
            if(setAsMain) local = this;
            this.pointers = new CircleDualPointer(maxSeq);
            connection = new ClientConnection(callbackList.Receive);
            connection.Start();
            AddCallback<S2CNtfOtherEnterExitRoom>(OnPlayerEnterExitRoom);
            AddCallback<C2AReqPing>(OnPing);
        }
        
        public void PollEvents()
        {
            lock(lockobj) connection?.mgr?.PollEvents();
        }
        
        // ====================================================================================================
        // 连接到服务器
        // ====================================================================================================
        
        public Task ConnectToServer(IPEndPoint endpoint, string key = ClientConnection.defaultKey) => connection.ConnectToServer(endpoint, key);
        
        // ====================================================================================================
        // 发送消息通用逻辑
        // ====================================================================================================
        
        NetDataWriter NetWriterWithHeader(NetSequenceId seq, NetId target, int protoId)
        {
            var writer = new NetDataWriter();
            writer.PutProtaSerialize(new CommonHeader(seq, this.id, target, protoId));
            return writer;
        }
        
        // target 填对应 NetId 则发送给客户端; target 填 NetId.none 广播.
        // 一些服务器会特护处理的内置协议类型除外.
        public void Send<T>(NetId target, T data)
        {
            if(connection?.peer == null)  throw new InvalidOperationException("Connect not initialized!");
            
            var writer = NetWriterWithHeader(NetSequenceId.notify, target, typeof(T).GetProtocolId());
            writer.PutProtaSerialize(data);
            
            lock(lockobj) connection.peer.Send(writer, typeof(T).GetProtocolMethod());
        }
        
        public RawRequestHandle<T> ExpectResult<T>() => new RawRequestHandle<T>(this);
        
        // ====================================================================================================
        // 房间相关
        // ====================================================================================================
        
        public async Task<bool> EnterRoom(int roomId)
        {
            var res = await ExpectResult<S2CRspEnterRoom>().Request(NetId.none, new C2SReqEnterRoom(roomId));
            if(!res.success) return false;
            lock(lockobj) room = new Room(res.data);
            return true;
        }
        
        public async Task<bool> LeaveRoom()
        {
            var res = await ExpectResult<S2CRspExitRoom>().Request(NetId.none, new C2SReqExitRoom());
            if(!res.success) return false;
            lock(lockobj) room = null;
            return true;
        }
        
        void OnPlayerEnterExitRoom(CommonHeader header, S2CNtfOtherEnterExitRoom info)
        {
            lock(lockobj)
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
        }
        
        // ====================================================================================================
        // 时间相关.
        // ====================================================================================================
        
        
        public async Task<string> Ping(NetId? target = null, string info = null)
        {
            target = target ?? NetId.none;
            var res = await ExpectResult<A2CRspPing>().Request(target.Value, new C2AReqPing() { info = info });
            return res.data.info;
        }
        
        
        void OnPing(CommonHeader header, C2AReqPing data)
        {
            Send(header.src, new A2CRspPing() { info = data.info });
        }
        
        // Ping operation with info == null.
        public async Task<float> GetRoundTripTime(NetId? target = null)
        {
            target = target ?? NetId.none;
            var res = await ExpectResult<A2CRspPing>().Request(target.Value, new C2AReqPing() { });
            return res.roundTripTime;
        }
        
        
        // ====================================================================================================
        // 接收消息通用逻辑
        // ====================================================================================================
        
        public NetCallbackManager.CallbackHandle AddCallback<T>(ProcessFunction<T> f)
        {
            lock(lockobj)
            {
                return callbackList.AddProcessor<T>(f);
            }
        }
        
        void RemoveCallback(NetCallbackManager.CallbackHandle handle)
        {
            lock(lockobj)
            {
                handle.Dispose();
            }
        }
        
        public async Task<ResultType> Request<ParamType, ResultType>(NetId? id = null, ParamType val = default)
        {
            id = id ?? NetId.none;
            return (await ExpectResult<ResultType>().Request(id.Value, val)).data;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public void Dispose()
        {
            lock(lockobj)
            {
                cancelSource.Cancel();
                room = null;
                callbackList = null;
                connection.Dispose();
                connection = null;
            }
        }
    }
}