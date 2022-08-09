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
        public readonly InternalType seq;
        public bool isNotify => seq == 0;
        public bool isRequest => seq > 0;
        public bool isResponse => seq < 0;
        public NetSequenceId response => new NetSequenceId(-Math.Abs(seq));
        public NetSequenceId request => new NetSequenceId(Math.Abs(seq));
        
        public static NetSequenceId notify => new NetSequenceId(0);
        
        public NetSequenceId(InternalType seq) => this.seq = seq;
        public int CompareTo(NetSequenceId other) => seq.CompareTo(other.seq);
        public bool Equals(NetSequenceId other) => seq == other.seq;
        public override bool Equals(object obj) => obj is NetSequenceId nid && nid.seq == this.seq;
        public override int GetHashCode() => seq.GetHashCode();
        public override string ToString() => $"NetSequence[{ seq }]";
        public static bool operator <(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) < 0;
        public static bool operator >(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) > 0;
        public static bool operator <=(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) <= 0;
        public static bool operator >=(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) >= 0;
        public static bool operator ==(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) == 0;
        public static bool operator !=(NetSequenceId left, NetSequenceId right) => left.CompareTo(right) != 0;
        
        public static implicit operator int(NetSequenceId seq) => seq.seq;
    }
}