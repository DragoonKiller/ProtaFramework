using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Prota;
using Prota.Unity;

namespace Prota.Timer
{

    public class TimerWait
    {
        public bool timesUp = false;
        
        public Timer timer;
        
        public readonly List<Action> callbacks = new List<Action>();
        
        public void OnTimesUp()
        {
            timesUp = true;
            callbacks.InvokeAll();
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
                if(continuation != null) wait.callbacks.Add(continuation);
            }
        }
        
        
        
        
        
        public static class UnitTest
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
            
            public static void Run()
            {
                DoFunc();
            }
        }
        
    }
}