using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Timer
{
    // 存储时间和计时器id, 用来给计时器排序.
    public struct TimeKey
    {
        static ulong gid = 0;
        public readonly ulong id;
        public readonly float time;
        
        public TimeKey(TimeKey original, float appendTime)
        {
            id = original.id;
            this.time = original.time + appendTime;
        }
        
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
        
}