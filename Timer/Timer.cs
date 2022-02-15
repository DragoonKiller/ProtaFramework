using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using XLua;

namespace Prota.Timer
{
    [LuaCallCSharp]
    public class Timer
    {
        public TimeKey key { get; private set; }
        public readonly Action callback;
        
        Timer(TimeKey key, bool repeat, Action callback)
        {
            this.key = key;
            this.callback = callback;
        }
        
        
        
        public struct TimeKey
        {
            public readonly uint id;
            public readonly float time;

            public TimeKey(float time)
            {
                this.id = unchecked(++gid);
                this.time = time;
            }
            
            public TimeKey(TimeKey x)
            {
                this.id = x.id;
                this.time = x.time;
            }
        }
        
        struct TimeKeyComparer : IComparer<TimeKey>
        {
            public int Compare(TimeKey a, TimeKey b)
            {
                return a.time != b.time ? a.time.CompareTo(b.time)
                    : a.id.CompareTo(b.id);
            }
        }
        
        static uint gid = 0;
        
        static SortedDictionary<TimeKey, Timer> timers = new SortedDictionary<TimeKey, Timer>(new TimeKeyComparer());
        
        static SortedDictionary<TimeKey, Timer> realtimeTimers = new SortedDictionary<TimeKey, Timer>(new TimeKeyComparer());
        
        public static int timersPerFrame = 5000;
        
        public static void Start()
        {
            gid = 0;
        }
        
        public static void Update()
        {
            var curTime = Time.time;
            int i = 0;
            for(i = 0; i < timersPerFrame; i++)
            {
                if(timers.Count == 0) break;
                var (timeKey, timer) = timers.First();
                if(timeKey.time <= curTime) break;
                timers.Remove(timeKey);
                timer.callback();
            }
            if(i == timersPerFrame) Log.Warning($"达到{ timersPerFrame }/帧计时器处理上限");
            
            curTime = Time.realtimeSinceStartup;
            for(i = 0; i < timersPerFrame; i++)
            {
                if(realtimeTimers.Count == 0) break;
                var (timeKey, timer) = realtimeTimers.First();
                if(timeKey.time <= curTime) break;
                realtimeTimers.Remove(timeKey);
                timer.callback();
            }
            
            if(i == timersPerFrame) Log.Warning($"达到{ timersPerFrame }/帧计时器处理上限");
        }
        
        
        public void Destroy()
        {
            timers.Remove(key);
            realtimeTimers.Remove(key);
        }
        
        public static Timer New(float time, Action callback)
        {
            var timer = new Timer(new TimeKey(Time.time + time), false, callback);
            timers.Add(timer.key, timer);
            return timer;
        }
        
        public static Timer NewRepeated(float time, Action callback)
        {
            var key = new TimeKey(Time.time + time);
            Timer timer = null;
            timer = new Timer(key, false, () => {
                callback();
                timer.key = new TimeKey(Time.time + time);    // 重新创建一个 key, 复用 timer.
                timers.Add(timer.key, timer);
            });
            return timer;
        }
        
        public static Timer NewRealtime(float time, Action callback)
        {
            var timer = new Timer(new TimeKey(Time.time + time), false, callback);
            realtimeTimers.Add(timer.key, timer);
            return timer;
        }
        
        public static Timer NewRealtimeRepeated(float time, Action callback)
        {
            var key = new TimeKey(Time.time + time);
            Timer timer = null;
            timer = new Timer(key, false, () => {
                timer.key = new TimeKey(Time.time + time);    // 重新创建一个 key, 复用 timer.
                realtimeTimers.Add(timer.key, timer);
            });
            return timer;
        }
        
    } 
}