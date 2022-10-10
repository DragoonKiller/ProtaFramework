using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Prota.Timer
{

    public class Timer
    {
        public string mname = null;
        public TimeKey key { get; private set; }
        public readonly Action callback;
        public bool repeat;
        public float duration;
        public string name => mname ?? key.id.ToString();
        
        internal Timer(string name, float curTime, float duration, bool repeat, Action callback)
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
        
        
        
        public static Timer New(float time, bool repeat, bool realtime, Action callback)
        {
            (time >= 0f).Assert();
            TimerManager.EnsureExists();
            if(realtime) return realtimeTimer.New(time, repeat, callback);
            return normalTimer.New(time, repeat, callback);
        }
        public static Timer New(float time, bool repeat, Action callback) => New(time, repeat, false, callback);
        public static Timer New(float time, Action callback) => New(time, false, false, callback);
        public static Timer New(float time) => New(time, false, false, null);
        
        
        public static Timer New(string name, float time, bool repeat, bool realtime, Action callback)
        {
            TimerManager.EnsureExists();
            if(realtime) return realtimeTimer.New(name, time, repeat, callback);
            return normalTimer.New(name, time, repeat, callback);
        }
        public static Timer New(string name, float time, bool repeat, Action callback) => New(name, time, repeat, false, callback);
        public static Timer New(string name, float time, Action callback) => New(name, time, false, false, callback);
        
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
    }
}