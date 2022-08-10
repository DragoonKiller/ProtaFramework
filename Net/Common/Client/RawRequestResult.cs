using System;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLib.Utils;

namespace Prota.Net
{
    public class RawRequestResult
    {
        public CommonHeader header;
        
        public NetDataReader reader;
        
        public CancellationToken cancellationToken;
        
        int _completed;
        public bool completed => _completed != 0;
        
        public bool valid = true;
        
        public void OnResponse(CommonHeader header, NetDataReader reader)
        {
            this.header = header;
            this.reader = reader;
            Interlocked.Increment(ref _completed);
        }
        
        public Task<T> ExpectResult<T>()
        {
            var expectingProtoId = ProtocolInfoCollector.GetId(typeof(T));
            return Task.Run(async () => {
                
                int sleepCount = 0;
                while(!completed && !this.cancellationToken.IsCancellationRequested)
                {
                    sleepCount++;
                    // 最多等待 0.1s.
                    if(sleepCount > 100) await Task.Delay(Math.Min(100, sleepCount / 100), this.cancellationToken);
                    else Thread.Yield();
                }
                
                if(header.protoId != expectingProtoId)
                    throw new ArgumentException($"Expecting message type [{ typeof(T).Name }] with ProtocolId [{ expectingProtoId }] but receive [{ header.protoId }]");
                
                var segment = reader.RawData.AsSegment(reader.Position);
                return segment.Deserialize<T>();
                    
            }, cancellationToken);
        }
    }
}