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
            lock(this)
            {
                this.header = header;
                this.reader = reader;
                Interlocked.Increment(ref _completed);
            }
        }
        
        public Task<T> ExpectResult<T>()
        {
            var expectingProtoId = ProtocolInfoCollector.GetId(typeof(T));
            return Task.Run(() => {
                
                int sleepCount = 0;
                while(!completed && !this.cancellationToken.IsCancellationRequested)
                {
                    sleepCount++;
                    if(sleepCount > 10) Thread.Sleep(1);
                    else Thread.Yield();
                }
                
                if(header.protoId != expectingProtoId)
                    throw new ArgumentException($"Expecting message type [{ typeof(T).Name }] with ProtocolId [{ expectingProtoId }] but receive [{ header.protoId }]");
                
                lock(this)
                {
                    var segment = reader.RawData.AsSegment(reader.Position);
                    return segment.Deserialize<T>();
                }
            });
        }
    }
}