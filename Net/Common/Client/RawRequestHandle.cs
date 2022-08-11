using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Prota.Net
{
    public partial class Client
    {
        public class RawRequestHandle<T> : INotifyCompletion
        {
            public readonly Client client;
            
            public NetSequenceId requestSeq { get; private set; }
            
            public NetSequenceId responseSeq => requestSeq.response;
            
            public T data { get; private set; }
            
            public CommonHeader header { get; private set; }
            
            public bool success => IsCompleted;
            
            public bool IsCompleted { get; private set; }
            
            public DateTime? requestTime { get; private set; }
            
            public DateTime? responseTime { get; private set; }
            
            public float roundTripTime => requestTime == null || responseTime == null ? 0 : (float)(requestTime.Value - responseTime.Value).TotalMilliseconds / 1000;
            
            public RawRequestHandle<T> Request<G>(NetId target, G data)
            {
                var res = new RawRequestResult() { cancellationToken = client.cancelSource.Token };
                requestSeq = new NetSequenceId(client.seqPool.Get());
                var writer = client.NetWriterWithHeader(requestSeq, target, typeof(G).GetProtocolId());
                writer.PutProtaSerialize(data);
                this.requestTime = DateTime.Now;
                client.connection.peer.Send(writer, typeof(G).GetProtocolMethod());
                return this;
            }
            
            public RawRequestHandle(Client client) => this.client = client;

            public RawRequestHandle<T> GetAwaiter() => this;
            
            public RawRequestHandle<T> GetResult() => this;
            
            public void OnCompleted(Action f)
            {
                (!responseSeq.isNotify && !responseSeq.isRequest).Assert();
                client.callbackList.AddProcessor<T>(responseSeq, (header, data) => {
                    client.seqPool.Return(responseSeq.request);
                    this.responseTime = DateTime.Now;
                    this.header = header;
                    this.data = data;
                    this.IsCompleted = true;
                    f();
                });
            }
        }
    }
}