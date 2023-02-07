using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Timer
{
    // 存储时间和计时器id, 用来给计时器排序.
    public struct TimeKey : IComparable<TimeKey>, IEquatable<TimeKey>
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
        
        public override string ToString() => $"TimeKey[{ id }:{ time }]";
        public int CompareTo(TimeKey other) => id == other.id ? time.CompareTo(other.time) : id.CompareTo(other.id);
        public bool Equals(TimeKey other) => this.CompareTo(other) == 0;
        public override int GetHashCode() => HashCode.Combine(id, time);
        public override bool Equals(object obj) => obj is TimeKey tk && tk.Equals(this);
        public static bool operator==(TimeKey left, TimeKey right) => left.CompareTo(right) == 0;
        public static bool operator!=(TimeKey left, TimeKey right) => left.CompareTo(right) != 0;
        public static bool operator<(TimeKey left, TimeKey right) => left.CompareTo(right) < 0;
        public static bool operator <=(TimeKey left, TimeKey right) => left.CompareTo(right) <= 0;
        public static bool operator >(TimeKey left, TimeKey right) => left.CompareTo(right) > 0;
        public static bool operator >=(TimeKey left, TimeKey right) => left.CompareTo(right) >= 0;
    }
        
}