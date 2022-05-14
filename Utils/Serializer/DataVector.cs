using System;

namespace Prota
{
    public class DataVector
    {
        // 元素个数.
        public int n { get; private set; } = 0;
        
        // 类型数组.
        public readonly VectorBuffer type = new VectorBuffer();
        
        // 数据大小.
        public int size { get; private set; } = 0;
        
        // 数据数组.
        public readonly VectorBuffer data = new VectorBuffer();
        
        public DataVector Push(byte value)
        {
            var t = BuiltinType.u8;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(sbyte value)
        {
            var t = BuiltinType.i8;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(short value)
        {
            var t = BuiltinType.i16;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(ushort value)
        {
            var t = BuiltinType.u16;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(int value)
        {
            var t = BuiltinType.i32;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(uint value)
        {
            var t = BuiltinType.u32;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(long value)
        {
            var t = BuiltinType.u64;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(ulong value)
        {
            var t = BuiltinType.u64;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(float value)
        {
            var t = BuiltinType.f32;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        public DataVector Push(double value)
        {
            var t = BuiltinType.f64;
            PushReserve(t.size);
            t.v.SetTo(type.data.AsSpan(t));
            n += 1;
            value.SetTo(data.data.AsSpan(size));
            size += t.size;
            return this;
        }
        
        DataVector PushReserve(int sz)
        {
            type.Preserve(n + 1);
            data.Preserve(size + sz);
            return this;
        }
        
        
        public byte PopU8()
        {
            var t = BuiltinType.u8;
            CheckTop(t);
            byte res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public sbyte PopI8()
        {
            var t = BuiltinType.i8;
            CheckTop(t);
            sbyte res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public short PopI16()
        {
            var t = BuiltinType.i16;
            CheckTop(t);
            short res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public ushort PopU16()
        {
            var t = BuiltinType.u16;
            CheckTop(t);
            ushort res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public int PopI32()
        {
            var t = BuiltinType.i32;
            CheckTop(t);
            int res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public uint PopU32()
        {
            var t = BuiltinType.u32;
            CheckTop(t);
            uint res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public long PopI64()
        {
            var t = BuiltinType.i64;
            CheckTop(t);
            long res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public ulong PopU64()
        {
            var t = BuiltinType.u64;
            CheckTop(t);
            ulong res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public float PopF32()
        {
            var t = BuiltinType.f32;
            CheckTop(t);
            float res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        public double PopF64()
        {
            var t = BuiltinType.f64;
            CheckTop(t);
            double res = 0;
            res.ReadFrom(data.data.AsSpan(size - t.size));
            PopShrink(t.size);
            return res;
        }
        
        void CheckTop(BuiltinType t)
        {
            if(n <= 0) throw new InvalidOperationException("size = 0");
            byte x = 0;
            x.ReadFrom(type.data.AsSpan(n - 1));
            var te = new BuiltinType(x);
            if(te != t) throw new InvalidOperationException(te.ToString());
        }
        
        void PopShrink(int sz)
        {
            n -= 1;
            size -= sz;
            type.Shrink(n);
            data.Shrink(size);
        }
        
        
    }
}