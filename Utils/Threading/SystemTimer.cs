using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Prota
{
    public class SystemTimer
    {
        readonly Timer timer;
        
        Action _callbacks;
        
        public event Action callbacks
        {
            add
            {
                if(triggered) value();
                else _callbacks += value;
            }
            
            remove
            {
                _callbacks -= value;
            }
        }
        
        public bool triggered { get; private set; }
        
        // duration in seconds.
        public SystemTimer(double duration)
        {
            var t = (int)Math.Ceiling(duration * 1000);
            this.timer = new Timer(state => {
                if(triggered) return;
                triggered = true;
                var cc = this._callbacks;
                this._callbacks = null;
                this.timer.Dispose();
                cc?.Invoke();
            }, null, t, 1);
        }
        
        // duration in seconds.
        public SystemTimer(float duration) : this((double)duration) { }
        
        
        public Awaiter GetAwaiter() => new Awaiter(this);
        
        public struct Awaiter : INotifyCompletion
        {
            readonly SystemTimer timer;
            public Awaiter(SystemTimer timer) => this.timer = timer;
            public void GetResult() { }
            public bool IsCompleted => timer.triggered;
            public void OnCompleted(Action continuation)
            {
                if(continuation != null) timer.callbacks += continuation;
            }
        }
    }
}
