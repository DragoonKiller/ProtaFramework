using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Prota.Data
{
    
    
    public enum DataType : byte
    {
        None = 0,
        Int32 = 1,
        Int64 = 2,
        Float32 = 3,
        Float64 = 4,
        String = 5,
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public struct RawDataValue
    {
        [FieldOffset(0)]
        public readonly int i32;
        
        [FieldOffset(0)]
        public readonly long i64;
        
        [FieldOffset(0)]
        public readonly float f32;
        
        [FieldOffset(0)]
        public readonly double f64;
        
        [FieldOffset(8)]
        public readonly string str;
        
        public RawDataValue(RawDataValue value) : this()
        {
            this.i64 = value.i64;
            this.str = value.str;
        }
        
        public RawDataValue(int v) : this() => i32 = v;
        public RawDataValue(long v) : this() => i64 = v;
        public RawDataValue(float v) : this() => f32 = v;
        public RawDataValue(double v) : this() => f64 = v;
        public RawDataValue(string v) : this() => str = v;

        public static bool operator==(RawDataValue a, RawDataValue b) => a.i64 == b.i64 && a.str == b.str;
        public static bool operator!=(RawDataValue a, RawDataValue b) => a.i64 != b.i64 && a.str == b.str;
        
        public override bool Equals(object obj) => obj is RawDataValue value && i64 == value.i64 && str == value.str;
        
        public override int GetHashCode()
        {
            var ret = new HashCode();
            ret.Add(i64);
            ret.Add(str);
            return ret.ToHashCode();
        }
        
        public override string ToString() => $"RawData[{i64};{str}]";
    }
    
    
    [StructLayout(LayoutKind.Sequential)]
    public struct DataValue
    {
        public readonly RawDataValue rawValue;
        
        public readonly DataType type;
        
        public int i32 => rawValue.i32;
        public long i64 => rawValue.i64;
        public float f32 => rawValue.f32;
        public double f64 => rawValue.f64;
        public string str => rawValue.str;
        

        public string stringPresentation
        {
            get
            {
                switch(type)
                {
                    case DataType.Int32: return i32.ToString();
                    case DataType.Int64: return i64.ToString();
                    case DataType.Float32: return f32.ToString();
                    case DataType.Float64: return f64.ToString();
                    case DataType.String: return str.ToString();
                    default: return "UnknownType";
                }
            }
        }
        
        public DataValue(int v)
        {
            type = DataType.Int32;
            rawValue = new RawDataValue(v);
        }
        
        public DataValue(long v)
        {
            type = DataType.Int64;
            rawValue = new RawDataValue(v);
        }
        public DataValue(float v)
        {
            type = DataType.Float32;
            rawValue = new RawDataValue(v);
        }
        public DataValue(double v)
        {
            type = DataType.Float64;
            rawValue = new RawDataValue(v);
        }
        public DataValue(string v)
        {
            type = DataType.String;
            rawValue = new RawDataValue(v);
        }
        
        public DataValue(DataType type, RawDataValue value)
        {
            rawValue = new RawDataValue(value);
            this.type = type;
        }
        
        public DataValue(DataType type, long value)
        {
            if(type == DataType.Int32)
            {
                rawValue = new RawDataValue((int)value);
                this.type = type;
            }
            else if(type == DataType.Int64)
            {
                rawValue = new RawDataValue(value);
                this.type = type;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        
        public DataValue(DataType type, double value)
        {
            if(type == DataType.Float32)
            {
                rawValue = new RawDataValue((float)value);
                this.type = type;
            }
            else if(type == DataType.Float64)
            {
                rawValue = new RawDataValue(value);
                this.type = type;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
        
        public DataValue(DataType type, string value)
        {
            Debug.Assert(type == DataType.String);
            rawValue = new RawDataValue(value);
            this.type = type;
        }
        
        public static implicit operator DataValue(int v) => new DataValue(v);
        public static implicit operator DataValue(long v) => new DataValue(v);
        public static implicit operator DataValue(float v) => new DataValue(v);
        public static implicit operator DataValue(double v) => new DataValue(v);
        public static implicit operator DataValue(string v) => new DataValue(v);
        
        
        public static bool operator==(DataValue a, DataValue b)
        {
            if(a.type != b.type) return false;
            switch(a.type)
            {
                case DataType.Int32: return a.i32 == b.i32;
                case DataType.Int64: return a.i64 == b.i64;
                case DataType.Float32: return a.f32 == b.f32;
                case DataType.Float64: return a.f64 == b.f64;
                case DataType.String: return a.str == b.str;
                default: return false;
            } 
        }
         
        public static bool operator!=(DataValue a, DataValue b) => !(a == b);

        public override bool Equals(object obj)
        {
            return (obj is DataValue value) && (this == value);
        }

        public override int GetHashCode()
        {
            switch(type)
            {
                case DataType.Int32: return i32.GetHashCode();
                case DataType.Int64: return i64.GetHashCode();
                case DataType.Float32: return f32.GetHashCode();
                case DataType.Float64: return f64.GetHashCode();
                case DataType.String: return str.GetHashCode();
                default: return type.GetHashCode();
            }
        }
        
        public override string ToString() => $"[{ type }({ stringPresentation })]";
    }
}