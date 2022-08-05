using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Prota
{
    public class SystemTimer
    {
        readonly Timer timer;
        
        public event Action callback;
        
        public bool triggered;
        
        // duration in seconds.
        public SystemTimer(double duration)
        {
            var t = (int)Math.Ceiling(duration * 1000);
            this.timer = new Timer(state =>
            {
                var callback = this.callback;
                triggered = true;
                this.callback = null;
                callback?.Invoke();
            }, null, t, 1);
        }
        
        // duration in seconds.
        public SystemTimer(float duration) : this((double)duration) { }
        
        
        public Awaiter GetAwaiter() => new Awaiter();
        
        public struct Awaiter : INotifyCompletion
        {
            SystemTimer timer;
            
            public Awaiter(SystemTimer timer)
            {
                this.timer = timer;
            }
            
            public void GetResult() { }
            
            public bool IsCompleted => timer.triggered;
            
            public void OnCompleted(Action continuation)
            {
                if(null != continuation) Task.Run(continuation);
            }
        }
    }
}
