using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Prota.Net
{
    public delegate void ProcessFunction<T>(CommonHeader header, T data);
    
    internal delegate void OnReceiveFunction(CommonHeader header, NetDataReader reader);
    
    public class NetCallbackManager
    {
        public readonly struct CallbackHandle : IDisposable
        {
            public readonly NetSequenceId seq;
            public readonly NetCallbackManager callbackList;
            public readonly int protoId;
            internal readonly OnReceiveFunction actualCallback;
            
            CallbackHandle(NetSequenceId seq, NetCallbackManager callbackList, int protoId, OnReceiveFunction actualCallback)
            {
                this.seq = seq;
                this.callbackList = callbackList;
                this.protoId = protoId;
                this.actualCallback = actualCallback;
            }
            
            // 如果注册的是 response (seq < 0), 则填入 processOnResponse 数组, 并记录 seq id.
            // 如果注册的是 notify 或 request (seq >= 0), 则填入 processOnReceive.
            public static CallbackHandle Create<T>(NetSequenceId seq, NetCallbackManager callbackList, int protoId, ProcessFunction<T> f)
            {
                var handle = new CallbackHandle(seq, callbackList, protoId, (header, reader) => {
                    f(header, reader.RawData.AsSegment(reader.Position).Deserialize<T>());
                });
                AddToList(handle);
                return handle;
            }
            
            static void AddToList(CallbackHandle handle)
            {
                lock(handle.callbackList.lockobj)
                {
                    if(handle.seq.isResponse)
                    {
                        handle.callbackList.processOnResponse.GetOrCreate(handle.protoId, out var dict);
                        dict.Add(handle.seq.response, handle);
                    }
                    else
                    {
                        handle.callbackList.processOnReceive.GetOrCreate(handle.protoId, out var list);
                        list.Add(handle);
                    }
                }
            }
            
            public void Dispose()
            {
                lock(callbackList.lockobj)
                {
                    if(seq.isResponse)
                    {
                        callbackList.processOnResponse.GetOrCreate(protoId, out var dict);
                        dict.Remove(this.seq.response);
                        if(dict.Count == 0) callbackList.processOnResponse.Remove(protoId);
                    }
                    else
                    {
                        callbackList.processOnReceive.GetOrCreate(protoId, out var list);
                        list.Remove(this);
                        if(list.Count == 0) callbackList.processOnReceive.Remove(protoId);
                    }
                }
            }
        }
        
        
        readonly Dictionary<int, List<CallbackHandle>> processOnReceive = new Dictionary<int, List<CallbackHandle>>();
        
        readonly Dictionary<int, Dictionary<NetSequenceId, CallbackHandle>> processOnResponse = new Dictionary<int, Dictionary<NetSequenceId, CallbackHandle>>();
        
        readonly object lockobj = new object();
        
        // 默认是 notify. 接收 Request 时也可以使用该接口注册函数.
        public CallbackHandle AddProcessor<T>(ProcessFunction<T> f)
        {
            lock(lockobj)
            {
                return CallbackHandle.Create(NetSequenceId.notify, this, ProtocolInfoCollector.GetId(typeof(T)), f);
            }
        }
        
        // 接收 Response 时使用该接口.
        public CallbackHandle AddProcessor<T>(NetSequenceId seq, ProcessFunction<T> f)
        {
            lock(lockobj)
            {
                return CallbackHandle.Create(seq, this, ProtocolInfoCollector.GetId(typeof(T)), f);
            }
        }
        
        public bool IsListening(int protoId)
        {
            lock(lockobj)
            {
                return processOnReceive.ContainsKey(protoId);
            }
        }
        
        public void Receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var header = reader.GetProtaSerialized<CommonHeader>();
            var s = header.seq;
            
            if(s.isNotify || s.isRequest)
            {
                using (var tempList = TempList<CallbackHandle>.Get())
                {
                    lock(lockobj)
                    {
                        if(!processOnReceive.TryGetValue(header.protoId, out var list))
                        {
                            header.Error($"receiving packet of type [{ header.protoId }] not listening.");
                            return;
                        }
                        
                        foreach(var handle in list) tempList.value.Add(handle);
                    }
                    
                    foreach(var handle in tempList.value) handle.actualCallback(header, reader);
                }
            }
            else
            {
                CallbackHandle handle = default;
                OnReceiveFunction callback = null;
                
                lock(lockobj)
                {
                    if(!processOnResponse.TryGetValue(header.protoId, out var dict))
                    {
                        header.Error($"receiving packet of type [{ header.protoId }] not listening.");
                        return;
                    }
                    if(!dict.TryGetValue(s, out handle))
                    {
                        header.Error($"receiving packet with [{ s }] but it's not listening.");
                        return;
                    }
                    
                    dict.Remove(handle.seq);
                    callback = handle.actualCallback;
                }
                
                handle.Dispose();
                callback(header, reader);
            }
        }
        
    }
}