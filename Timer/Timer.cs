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
        
        public Timer(string name, float curTime, float duration, bool repeat, Action callback)
        {
            this.mname = name;
            this.key = new TimeKey(curTime + duration);
            this.duration = duration;
            this.repeat = repeat;
            this.callback = callback;
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
            if(realtime) return realtimeTimer.New(time, repeat, callback);
            return normalTimer.New(time, repeat, callback);
        }
        public static Timer New(float time, bool repeat, Action callback) => New(time, repeat, false, callback);
        public static Timer New(float time, Action callback) => New(time, false, false, callback);
        
        
        public static Timer New(string name, float time, bool repeat, bool realtime, Action callback)
        {
            if(realtime) return realtimeTimer.New(name, time, repeat, callback);
            return normalTimer.New(name, time, repeat, callback);
        }
        public static Timer New(string name, float time, bool repeat, Action callback) => New(name, time, repeat, false, callback);
        public static Timer New(string name, float time, Action callback) => New(name, time, false, false, callback);
        
    } 
}