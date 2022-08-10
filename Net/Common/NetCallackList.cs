using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Prota.Net
{
    public delegate void ProcessFunction<T>(CommonHeader header, T data);
    
    internal delegate void OnReceiveFunction(CommonHeader header, NetDataReader reader);
        
        
    public class NetCallbackManager
    {
        public struct CallbackHandle : IDisposable
        {
            public readonly NetCallbackManager callbackList;
            public readonly int protoId;
            public readonly bool removeAfterExecute;
            internal readonly OnReceiveFunction actualCallback;

            CallbackHandle(NetCallbackManager callbackList, int protoId, bool removeAfterExecute, OnReceiveFunction actualCallback)
            {
                this.callbackList = callbackList;
                this.protoId = protoId;
                this.removeAfterExecute = removeAfterExecute;
                this.actualCallback = actualCallback;
            }

            public static CallbackHandle Create<T>(NetCallbackManager callbackList, int protoId, bool removeAfterExecute, ProcessFunction<T> f)
            {
                var handle = new CallbackHandle(callbackList, protoId, removeAfterExecute, (header, reader) => {
                    f(header, reader.RawData.AsSegment(reader.Position).Deserialize<T>());
                });
                callbackList.processOnReceive.GetOrCreate(protoId, out var list);
                list.Add(handle);
                return handle;
            }
            
            public void RemoveFromClient()
            {
                callbackList.processOnReceive.GetOrCreate(protoId, out var list);
                list.Remove(this);
                if(list.Count == 0) callbackList.processOnReceive.Remove(protoId);
            }
            
            public void Dispose() => RemoveFromClient();
        }
        
        
        readonly Dictionary<int, List<CallbackHandle>> processOnReceive = new Dictionary<int, List<CallbackHandle>>();
        
        public NetPeer currentProcessingPeer { get; private set; }
        
        public CallbackHandle AddProcessor<T>(ProcessFunction<T> f, bool removeAfterExecute = false)
        {
            var protoId = ProtocolInfoCollector.GetId(typeof(T));
            return CallbackHandle.Create(this, protoId, removeAfterExecute, f);
        } 
        
        public void RemoveProcessor(CallbackHandle handle) => handle.Dispose();
        
        
        public bool IsListening(int protoId) => processOnReceive.ContainsKey(protoId);
        
        public void Receive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var header = reader.GetCommonHeader();
            var s = header.seq;
            
            if(!processOnReceive.TryGetValue(header.protoId, out var list))
            {
                header.Error($"receiving packet of type [{ header.protoId }] not listening.");
                return;
            }
            
            currentProcessingPeer = peer;
            try
            {
                int freePointer = 0;
                for(int i = 0; i < list.Count; i++)
                {
                    var c = list[i];
                    c.actualCallback(header, reader);
                    if(!c.removeAfterExecute)
                    {
                        if(freePointer != i) list[freePointer] = list[i];
                        freePointer += 1;
                    }
                }
                
                for(int i = 0, n = list.Count - freePointer; i < n; i++) list.RemoveLast();
            }
            finally
            {
                currentProcessingPeer = null;
            }
        }
        
    }
}