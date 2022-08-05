using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Prota
{
    public static class SynchronizationContextAwaitExt
    {
        public static SynchronizationContextAwaiter GetAwaiter(this SynchronizationContext context)
        {
            return new SynchronizationContextAwaiter(context);
        }
    }
    
    public struct SynchronizationContextAwaiter : INotifyCompletion
    {
        readonly SynchronizationContext context;
        public bool IsCompleted { get; private set; }
        internal SynchronizationContextAwaiter(SynchronizationContext context)
        {
            this.context = context;
            this.IsCompleted = false;
        }
        
        public void GetResult() { }
        public void OnCompleted(Action continuation)
        {
            IsCompleted = true;
            if(continuation == null) return;
            context.Post(o => continuation(), null);
        }
    }
}
