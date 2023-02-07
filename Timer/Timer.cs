using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

using Prota.Unity;

namespace Prota.Timer
{

    public class Timer
    {
        public string mname = null;
        public TimeKey key { get; private set; }
        public readonly Action callback;
        public bool repeat;
        public float duration;
        public readonly LifeSpan guard;
        public bool isAlive => guard == null || guard.alive;
        public string name => mname ?? key.id.ToString();
        
        internal Timer(string name, float curTime, float duration, bool repeat, LifeSpan guard, Action callback)
        {
            this.mname = name;
            this.key = new TimeKey(curTime + duration);
            this.duration = duration;
            this.repeat = repeat;
            this.callback = callback;
        }
        
        internal bool NextRepeat()
        {
            if(!repeat) return false;
            if(guard != null && !guard.alive) return false;
            key = new TimeKey(key, duration);
            return true;
        }
        
        public static TimerQueue normalTimer = new TimerQueue(() => Time.time);
        
        public static TimerQueue realtimeTimer = new TimerQueue(() => Time.realtimeSinceStartup);
        
        public static void Start()
        {
            
        }
        
        public static void Update()
        {
            normalTimer.Update();
            realtimeTimer.Update();
        }
        
        
        
        public static Timer New(float time, bool repeat, bool realtime, LifeSpan guard, Action callback)
        {
            (time >= 0f).Assert();
            TimerManager.EnsureExists();
            if(realtime) return realtimeTimer.New(time, repeat, guard, callback);
            return normalTimer.New(time, repeat, guard, callback);
        }
        public static Timer New(float time, bool repeat, LifeSpan guard, Action callback) => New(time, repeat, false, guard, callback);
        public static Timer New(float time, LifeSpan guard, Action callback) => New(time, false, false, guard, callback);
        public static Timer New(float time, bool repeat, Action callback) => New(time, repeat, false, null, callback);
        public static Timer New(float time, Action callback) => New(time, false, false, null, callback);
        
        public static Timer New(string name, float time, bool repeat, bool realtime, LifeSpan guard, Action callback)
        {
            TimerManager.EnsureExists();
            if(realtime) return realtimeTimer.New(name, time, repeat, guard, callback);
            return normalTimer.New(name, time, repeat, guard, callback);
        }
        public static Timer New(string name, float time, bool repeat, LifeSpan guard, Action callback) => New(name, time, repeat, false, guard, callback);
        public static Timer New(string name, float time, LifeSpan guard, Action callback) => New(name, time, false, false, guard, callback);
        public static Timer New(string name, float time, bool repeat, Action callback) => New(name, time, repeat, false, null, callback);
        public static Timer New(string name, float time, Action callback) => New(name, time, false, false, null, callback);
        
        // 支持 Async-Await.
        // await 后的流程会从主线程(timer的调度线程)调起来.
        // 由于创建 timer 也需要在主线程创建, 所以这个函数需要在主线程调用.
        public static TimerWait Wait(float time)
        {
            var res = new TimerWait();
            res.timer = New(time, false, false, null, res.OnTimesUp);
            return res;
        }
    }
    
    public static class TimerExt
    {
        public static bool IsValid(this Timer timer)
        {
            if(Timer.normalTimer.IsTimerValid(timer)) return true;
            if(Timer.realtimeTimer.IsTimerValid(timer)) return true;
            return false;
        }
        
        public static bool Remove(this Timer timer)
        {
            if(Timer.normalTimer.TryRemove(timer)) return true;
            if(Timer.realtimeTimer.TryRemove(timer)) return true;
            return false;
        }
        
        public static Timer NewTimer(this GameObject x, float time, Action callback)
            => Timer.New(time, false, false, x.LifeSpan(), callback);
        public static Timer NewTimer(this GameObject x, float time, bool repeat, bool realtime, Action callback)
            => Timer.New(time, repeat, realtime, x.LifeSpan(), callback);
        public static Timer NewTimer(this Component x, float time, Action callback)
            => Timer.New(time, false, false, x.LifeSpan(), callback);
        public static Timer NewTimer(this Component x, float time, bool repeat, bool realtime, Action callback)
            => Timer.New(time, repeat, realtime, x.LifeSpan(), callback);
    }
}