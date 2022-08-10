using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LiteNetLib.Utils;

namespace Prota.Net
{
    using InternalType = System.UInt16;
    
    [StructLayout(LayoutKind.Sequential)]
    [ProtaSerialize]
    public struct NetId : IEquatable<NetId>, IComparable<NetId>, INetSerializable
    {
        public const int internalSize = sizeof(InternalType);
        
        public InternalType id { get; private set; }
        
        public static NetId none => new NetId(0);
        public bool isNone => id == 0;
        public static bool ValidRange(int i) => InternalType.MinValue <= i && i <= InternalType.MaxValue;
        
        public NetId(InternalType id) => this.id = id;
        public int CompareTo(NetId other) => id.CompareTo(other.id);
        public bool Equals(NetId other) => id == other.id;
        public override bool Equals(object obj) => obj is NetId nid && nid.id == this.id;
        public override int GetHashCode() => id.GetHashCode();
        public override string ToString() => $"NetId[{ id }]";

        public void Serialize(NetDataWriter writer) => writer.Put(id);
        public void Deserialize(NetDataReader reader) => id = reader.GetUShort();

        public static bool operator <(NetId left, NetId right) => left.CompareTo(right) < 0;
        public static bool operator >(NetId left, NetId right) => left.CompareTo(right) > 0;
        public static bool operator <=(NetId left, NetId right) => left.CompareTo(right) <= 0;
        public static bool operator >=(NetId left, NetId right) => left.CompareTo(right) >= 0;
        public static bool operator ==(NetId left, NetId right) => left.CompareTo(right) == 0;
        public static bool operator !=(NetId left, NetId right) => left.CompareTo(right) != 0;
    }
    
    
    
    public class NetIdPool
    {
        readonly HashSet<NetId> unused = new HashSet<NetId>();
        
        readonly HashSet<NetId> used = new HashSet<NetId>();
        
        
        public int usableCount => unused.Count;
        
        public int usedCount => used.Count;
        
        public int totalCount => usableCount + usedCount;
        
        public NetIdPool(int max)
        {
            NetId.ValidRange(max).Assert();
            for(int i = 1; i <= max; i++)
            {
                unused.Add(new NetId((InternalType)i));
            }
        }
        
        public NetId Get()
        {
            var e = unused.FirstElement();
            unused.Remove(e);
            used.Add(e);
            return e;
        }
        
        public NetIdPool Return(NetId id)
        {
            if(!used.Contains(id)) return this;
            unused.Add(id);
            used.Remove(id);
            return this;
        }
    }
}