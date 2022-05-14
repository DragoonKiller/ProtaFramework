using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;

namespace Prota
{
    using Index = System.Int32;
    public class FlatData
    {
        const int indexSize = sizeof(int);
        
        byte[] data;
        
        public FlatData(IReadOnlyList<int> list)
        {
            data = new byte[list.Count * sizeof(int)];
            var offset = 0;
            foreach(var v in list) offset = Set(offset, v);
        }
        
        public FlatData(IReadOnlyList<long> list)
        {
            data = new byte[list.Count * sizeof(long)];
            var offset = 0;
            foreach(var v in list) offset = Set(offset, v);
        }
        
        public FlatData(IReadOnlyList<float> list)
        {
            data = new byte[list.Count * sizeof(float)];
            var offset = 0;
            foreach(var v in list) offset = Set(offset, v);
        }
        
        public FlatData(IReadOnlyList<double> list)
        {
            data = new byte[list.Count * sizeof(double)];
            var offset = 0;
            foreach(var v in list) offset = Set(offset, v);
        }
        
        public FlatData(IReadOnlyList<string> list)
        {
            var offset = 0;
            foreach(var v in list) offset = Set(offset, v);
        }
        
        
        
        public void EnsureSize(Index n)
        {
            if(data.Length > n) return;
            var newData = new byte[(Index)Math.Floor(data.Length * 1.5f)];
            Buffer.BlockCopy(data, 0, newData, 0, data.Length);
            data = newData;
        }
        
        public int Set(Index offset, int v)
        {
            EnsureSize(offset + sizeof(int));
            BinaryPrimitives.WriteInt32BigEndian(data.AsSpan(offset), v);
            return offset + sizeof(int);
        }
        
        public int Set(Index offset, long v)
        {
            EnsureSize(offset + sizeof(long));
            BinaryPrimitives.WriteInt64BigEndian(data.AsSpan(offset), v);
            return offset + sizeof(long);
        }
        
        public int Set(Index offset, float v)
        {
            return Set(offset, BitConverter.SingleToInt32Bits(v));
        }
        
        public int Set(Index offset, double v)
        {
            return Set(offset, BitConverter.DoubleToInt64Bits(v));
        }
        
        public int Set(Index offset, string s)
        {
            EnsureSize(offset + sizeof(int) + Encoding.UTF8.GetByteCount(s));
            offset = Set(offset, Encoding.UTF8.GetByteCount(s));
            int len = Encoding.UTF8.GetBytes(s, 0, s.Length, data, offset);
            return offset + len;
        }
        
        
        public int ReadInt32(Index offset)
        {
            return BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(offset));
        }
        
        public long ReadInt64(Index offset)
        {
            return BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(offset));
        }
        
        public float ReadFloat(Index offset)
        {
            var x = ReadInt32(offset);
            return BitConverter.Int32BitsToSingle(x);
        }
        
        public double ReadDouble(Index offset)
        {
            var x = ReadInt64(offset);
            return BitConverter.Int64BitsToDouble(x);
        }
        
        public string ReadString(Index offset)
        {
            var len = ReadInt32(offset);
            offset += sizeof(int);
            return Encoding.UTF8.GetString(data, offset, len);
        }
        
        
    }
}