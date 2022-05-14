using System;

namespace Prota
{
    public class VectorBuffer
    {
        public const float extendMult = 1.5f;
        
        public byte[] data { get; private set; }
        
        public int cap => data?.Length ?? 0;
        
        public int nextCap => data != null ? (int)Math.Ceiling(data.Length * extendMult) : 1;
        
        public int prevCap => data != null ? (int)Math.Ceiling(data.Length / extendMult) : 0;
        
        public unsafe void WithPtr(Action<IntPtr> f)
        {
            lock(data)
            {
                fixed(byte* d = data)
                {
                    f(new IntPtr(d));
                }
            }
        }
        
        public void Resize(int n)
        {
            if(n <= 0) { data = null; return; }
            
            var target = new byte[n];
            if(data != null)
            {
                unsafe
                {
                    fixed(byte* to = target)
                    fixed(byte* from = data)
                    {
                        System.Buffer.MemoryCopy(from, to, n, data.Length);
                    }
                }
            }
        }
        
        // 扩充到恰好能够容下 n 个字节.
        public VectorBuffer Preserve(int n)
        {
            while(n > cap) Resize(nextCap);
            return this;
        }
        
        // 缩减到恰好能够容下 n 个字节.
        public VectorBuffer Shrink(int n)
        {
            while(n < prevCap) Resize(prevCap);
            return this;
        }
    }
}