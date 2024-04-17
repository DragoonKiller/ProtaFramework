using System;
using System.Collections.Generic;

namespace Prota
{
    public struct BuiltinType : IEquatable<BuiltinType>, IComparable<BuiltinType>
    {
        public byte v { get; private set; }
        
        public static BuiltinType none => new BuiltinType { v = 0 };
        public static BuiltinType i8 => new BuiltinType { v = 1 };
        public static BuiltinType u8 => new BuiltinType { v = 2 };
        public static BuiltinType i16 => new BuiltinType { v = 3 };
        public static BuiltinType u16 => new BuiltinType { v = 4 };
        public static BuiltinType i32 => new BuiltinType { v = 5 };
        public static BuiltinType u32 => new BuiltinType { v = 6 };
        public static BuiltinType i64 => new BuiltinType { v = 7 };
        public static BuiltinType u64 => new BuiltinType { v = 8 };
        public static BuiltinType f32 => new BuiltinType { v = 9 };
        public static BuiltinType f64 => new BuiltinType { v = 10 };
        

        #if UNITY_2017_1_OR_NEWER
        
        public static BuiltinType vec2 => new BuiltinType { v = 20 };
        public static BuiltinType vec3 => new BuiltinType { v = 21 };
        public static BuiltinType vec4 => new BuiltinType { v = 22 };
        public static BuiltinType quat => new BuiltinType { v = 23 };
        public static BuiltinType color => new BuiltinType { v = 24 };
        public static BuiltinType color32 => new BuiltinType { v = 25 };
        
        #endif
    
    
        static int[] _size;
        static string[] _name;
        
        static BuiltinType()
        {
            _size = new int[64];
            _size[none.v] = 0;
            _size[i8.v] = _size[u8.v] = 1;
            _size[i16.v] = _size[u16.v] = 2;
            _size[i32.v] = _size[u32.v] = _size[f32.v] = 4;
            _size[i64.v] = _size[u64.v] = 8;
            
            _name = new string[64];
            _name[none.v] = "Unknown";
            _name[i8.v] = "i8";
            _name[u8.v] = "u8";
            _name[i16.v] = "i16";
            _name[u16.v] = "u16";
            _name[i32.v] = "i32";
            _name[u32.v] = "u32";
            _name[i64.v] = "i64";
            _name[u64.v] = "u64";
            _name[f32.v] = "f32";
            _name[f64.v] = "f64";
            
            #if UNITY_2017_1_OR_NEWER
            
            _size[vec2.v] = 8;
            _size[vec3.v] = 12;
            _size[vec4.v] = 16;
            _size[color.v] = 16;
            _size[color32.v] = 4;
            
            _name[vec2.v] = "vec2";
            _name[vec3.v] = "vec3";
            _name[vec4.v] = "vec4";
            _name[color.v] = "color";
            _name[color32.v] = "color32";
            
            #endif
        }
        
        public BuiltinType(byte v) => this.v = v;
        
        public int size => _size[this.v];
        
        public static implicit operator BuiltinType(byte v) => new BuiltinType { v = v };
        public static implicit operator byte(BuiltinType v) => v.v;
        public static implicit operator BuiltinType(sbyte v) => new BuiltinType { v = (byte)v };
        public static implicit operator sbyte(BuiltinType v) => (sbyte)v.v;
        public static implicit operator BuiltinType(int v) => new BuiltinType { v = (byte)v };
        public static implicit operator int(BuiltinType v) => v.v;
        public static implicit operator BuiltinType(uint v) => new BuiltinType { v = (byte)v };
        public static implicit operator uint(BuiltinType v) => v.v;
        public static implicit operator BuiltinType(long v) => new BuiltinType { v = (byte)v };
        public static implicit operator long(BuiltinType v) => v.v;
        public static implicit operator BuiltinType(ulong v) => new BuiltinType { v = (byte)v };
        public static implicit operator ulong(BuiltinType v) => v.v;
        
        
        public int CompareTo(BuiltinType other) => this.v - other.v;

        public override int GetHashCode() => this.v.GetHashCode();
        
        public bool Equals(BuiltinType other) => this.v == other.v;
        
        public override bool Equals(System.Object other) => other is BuiltinType x ? this.Equals(x) : false;
        
        

        public override string ToString() => _name[v];
        
        
        public static bool operator==(BuiltinType a, BuiltinType b) => a.v == b.v;
        public static bool operator!=(BuiltinType a, BuiltinType b) => a.v != b.v;
        public static bool operator>(BuiltinType a, BuiltinType b) => a.v > b.v;
        public static bool operator<(BuiltinType a, BuiltinType b) => a.v < b.v;
        public static bool operator>=(BuiltinType a, BuiltinType b) => a.v >= b.v;
        public static bool operator<=(BuiltinType a, BuiltinType b) => a.v <= b.v;
    }
    
}
