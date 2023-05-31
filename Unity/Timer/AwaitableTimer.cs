using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.CompilerServices;
using Prota;
using System.Threading.Tasks;

namespace Prota.Unity
{

    public class TimerWait
    {
        public bool timesUp = false;
        
        public Timer timer;
        
        public TaskCanceller.Token cancellationToken;
        
        // add some plain callbacks so that we don't allocate a List.
        Action callback;
        Action callback2;
        Action callback3;
        List<Action> callbacks;
        
        public void OnTimesUp()
        {
            timesUp = true; 
            if(cancellationToken.cancelled) return;    // do nothing if canclled.
            callback?.Invoke();
            callback2?.Invoke();
            callback3?.Invoke();
            callbacks?.InvokeAll();
            callback = callback2 = callback3 = null;
            callbacks = null;
        }
        
        // Async-Await support.
        public Awaiter GetAwaiter() => new Awaiter(this);
        
        public struct Awaiter : INotifyCompletion
        {
            readonly TimerWait wait;
            public Awaiter(TimerWait wait) => this.wait = wait;
            public void GetResult() { }
            public bool IsCompleted => wait.timesUp;
            public void OnCompleted(Action continuation)
            {
                if(continuation == null) return;
                
                if(IsCompleted)
                {
                    continuation();
                    return;
                }
                
                if(wait.callback == null)
                {
                    wait.callback = continuation;
                }
                else if(wait.callback2 == null)
                {
                    wait.callback2 = continuation;
                }
                else if(wait.callback3 == null)
                {
                    wait.callback3 = continuation;
                }
                else
                {
                    wait.callbacks = wait.callbacks ?? new List<Action>();
                    wait.callbacks.Add(continuation);
                }
            }
        }
        
        
        
        
        public static partial class UnitTest
        {
            
                public static void UnitTestAwaitableTimer()
                {
                    static async void DoFunc()
                    {
                        await new SwitchToMainThread();
                        $"stage 1 { Time.time }".Log();
                        await Timer.Wait(1);
                        $"stage 2 { Time.time }".Log();
                        await Timer.Wait(2);
                        $"stage 3 { Time.time }".Log();
                        await Timer.Wait(3);
                        $"stage 4 { Time.time }".Log();
                    }
                
                    DoFunc();
                }
        }
        
    }
}
