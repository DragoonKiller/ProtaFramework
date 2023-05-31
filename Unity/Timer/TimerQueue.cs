using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
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
        public readonly SortedDictionary<TimeKey, Timer> timers = new SortedDictionary<TimeKey, Timer>(new TimeKeyComparer());
        
        public TimerQueue(Func<float> GetTime) => this.GetTime = GetTime;
        
        public void Update()
        {
            var curTime = GetTime();
            int i = 0, n = timersPerUpdate.Min(timers.Count);
            for(i = 0; i < n; i++)
            {
                var (timeKey, timer) = timers.First();
                if(curTime < timeKey.time) break;
                
                // 先删除, 如果有需要再添加回去.
                timers.Remove(timeKey);
                
                if(timer.isAlive)
                {
                    var callback = timer.callback;
                    callback?.Invoke();
                    if(timer.NextRepeat()) timers[timer.key] = timer;
                }
            }
            if(i == timersPerUpdate) UnityEngine.Debug.LogWarning($"达到{ timersPerUpdate }/帧计时器处理上限");
        }
        
        public Timer New(float duration, bool repeat, LifeSpan guard, Action callback)
            => New(null, duration, repeat, guard, callback);
        
        public Timer New(string name, float duration, bool repeat, LifeSpan guard, Action callback)
        {
            var timer = new Timer(name, GetTime(), duration, repeat, guard, callback);
            timers.Add(timer.key, timer);
            return timer;
        }
        
        public bool TryRemove(Timer timer) => timers.Remove(timer.key);
        
        public bool IsTimerValid(Timer timer) => timers.ContainsKey(timer.key);
        
        public void Clear() => timers.Clear();
        
    }
}
