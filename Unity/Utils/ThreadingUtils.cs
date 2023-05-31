using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public struct SwitchToMainThread
    {
        static SynchronizationContext _context;
        public static SynchronizationContext context
        {
            get
            {
                if(_context != null) return _context;
                _context = SynchronizationContext.Current;
                
                // this should not happen since the most early execution is [InitializeOnLoadMethod]
                // so either SwitchToMainThread.Init is excuted normally
                // or this getter is executed in some [InitializeOnLoadMethod]
                if(_context == null) throw new Exception("context is used too early, try delay it.");
                
                return _context;
            }
        }
        
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            if(_context == null) _context = SynchronizationContext.Current;
            // Debug.Log($"Get SyncContext: { context }");
        }
        
        public SynchronizationContextAwaiter GetAwaiter()
        {
            if(_context == null) throw new Exception("BackToMainThread should not use in Awake.");
            return context.GetAwaiter();
        }
    }
    
    public struct WaitForNextFrame
    {
        public SynchronizationContextAwaiter GetAwaiter()
        {
            if(SwitchToMainThread.context == null) throw new Exception("BackToMainThread should not use in Awake.");
            return SwitchToMainThread.context.GetAwaiter();
        }
    }
    
    public struct SwitchToWorkerThread
    {   
        public SynchronizationContextAwaiter GetAwaiter()
        {
            return (null as SynchronizationContext).GetAwaiter();
        }
    }
    
    
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
            using var _ = TempList<Action>.Get(out var callbacks);
            callbacks.AddRange(this.callbacks);
            this.callbacks.Clear();
            foreach(var callback in callbacks) callback();
            stepId += 1;
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
