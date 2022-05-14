using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Timer
{
    public class TimerQueue
    {
        struct TimeKeyComparer : IComparer<TimeKey>
        {
            public int Compare(TimeKey a, TimeKey b)
            {
                return a.time != b.time ? a.time.CompareTo(b.time)
                    : a.id.CompareTo(b.id);
            }
        }
        
        const int timersPerUpdate = 5000;
        
        Func<float> GetTime;
        public SortedDictionary<TimeKey, Timer> timers = new SortedDictionary<TimeKey, Timer>(new TimeKeyComparer());
        
        public TimerQueue(Func<float> GetTime) => this.GetTime = GetTime;
        
        public void Update()
        {
            var curTime = GetTime();
            int i = 0;
            for(i = 0; i < timersPerUpdate; i++)
            {
                if(timers.Count == 0) break;
                var (timeKey, timer) = timers.First();
                if(timeKey.time <= curTime) break;
                timers.Remove(timeKey);
                timer.callback();
                if(timer.repeat) timers[new TimeKey(timer.duration)] = timer;
            }
            if(i == timersPerUpdate) UnityEngine.Debug.LogWarning($"达到{ timersPerUpdate }/帧计时器处理上限");
        }
        
        public Timer New(float duration, bool repeat, Action callback)
        {
            var timer = new Timer(null, GetTime(), duration, repeat, callback);
            return timer;
        }
        
        public Timer New(string name, float duration, bool repeat, Action callback)
        {
            var timer = new Timer(name, GetTime(), duration, repeat, callback);
            return timer;
        }
        
        public void Clear() => timers.Clear();
        
    }
}