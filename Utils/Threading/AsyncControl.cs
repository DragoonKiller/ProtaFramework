using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Prota
{

    // 异步流程控制器, 用于在给定的时间手动调起某些异步流程.
    // 用法:
    // var control = new UniversalAsyncControl();
    // 在某个异步方法中:
    // await control;
    // 在控制器中调用:
    // control.Step();
    // 即可从控制器调用到异步方法的执行过程.
    // 如果控制器调用 Step() 时在主线程, 那么异步方法的执行也会回到主线程.
    public class AsyncControl
    {
        public long stepId { get; private set; } = 0;
        
        public readonly List<Action> callbacks = new List<Action>();
        
        public Awaiter GetAwaiter() => new Awaiter(stepId, this);
        
        public struct Awaiter : INotifyCompletion
        {
            public readonly long stepId;
            
            public readonly AsyncControl control;

            public bool valid => control != null;
            
            public Awaiter(long stepId, AsyncControl control)
            {
                this.stepId = stepId;
                this.control = control;
            }

            public bool IsCompleted => stepId < control.stepId;
            
            public void OnCompleted(Action continuation)
            {
                control.callbacks.Add(continuation);
            }
            
            public void GetResult()
            {
                
            }
        }
        
        public AsyncControl Step()
        {
            using var _ = TempList.Get<Action>(out var callbacks);
            callbacks.AddRange(this.callbacks);
            this.callbacks.Clear();
            foreach(var callback in callbacks) callback();
            stepId += 1;
            return this;
        }
        
        public AsyncControl StepUntilClear(int max = 10000)
        {
            for(int i = 0; i <= max; i++)
            {
                if(callbacks.Count == 0) break;
                Step();
                if(i == max) throw new Exception("AsyncControl.StepUntilClear() reached max. There could be a dead loop.");
            }
            return this;
        }
        
        public void CancelAll()
        {
            callbacks.Clear();
        }
        
        // 一个由 UniversalAsyncControl 控制的主动更新式异步 timer.
        // 主要用在一些可以根据逻辑*暂停*的异步计时.
        public async Task Wait(float time, Func<float> deltaTime, TaskCanceller.Token? token = null)
        {
            var t = 0f;
            while((token == null || !token.Value.cancelled) && t < time)
            {
                await this;
                t += deltaTime();
            }
        }
        
    }
}
