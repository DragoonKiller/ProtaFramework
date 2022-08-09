using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Prota.Net
{
    using InternalType = System.Int32;
    
    [StructLayout(LayoutKind.Sequential)]
    public struct NetSequenceId : IEquatable<NetSequenceId>, IComparable<NetSequenceId>
    {
        public const int internalSize = sizeof(InternalType);
        public readonly InternalType v;
        public bool isNotify => v == 0;
        public bool isRequest => v > 0;
        public bool isResponse => v < 0;
        public NetSequenceId response => new NetSequenceId(-Math.Abs(v));
        public NetSequenceId request => new NetSequenceId(Math.Abs(v));
        
        public static NetSequenceId notify => new NetSequenceId(0);
        
        public NetSequenceId(InternalType v) => this.v = v;
        public int CompareTo(NetSequenceId other) => v.CompareTo(other.v);
        public bool Equals(NetSequenceId other) => v == other.v;
        public override bool Equals(object obj) => obj is NetSequenceId nid && nid.v == this.v;
        public override int GetHashCode() => v.GetHashCode();
        public override string ToString() => $"NetSequence[{ v }]";
        public static bool operator <(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) < 0;
        public static bool operator >(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) > 0;
        public static bool operator <=(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) <= 0;
        public static bool operator >=(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) >= 0;
        public static bool operator ==(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) == 0;
        public static bool operator !=(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) != 0;
        
        public static implicit operator int(NetSequenceId seq) => seq.v;
    }
}