using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Runtime.Serialization;
using System.Collections;

namespace Prota
{
    public struct BitValue
    {
        public readonly int value;
        const int bitCount = 32;
        
        public BitValue(int x) => value = x;
        
        
        public static BitValue full => new BitValue(~0);
        public static BitValue none => new BitValue();
        
        
        public static BitValue operator&(BitValue a, BitValue b) => new BitValue(a.value & b.value);
        public static BitValue operator|(BitValue a, BitValue b) => new BitValue(a.value | b.value);
        public static BitValue operator~(BitValue a) => new BitValue(~a.value);
        public static BitValue operator!(BitValue a) => new BitValue(~a.value);


        public struct BitEnum : IEnumerable<int>, IEnumerator<int>
        {
            public int value;
            public int current;
            public bool targetIsOne;
            public bool posMode;
            
            public int Current => posMode ? current : unchecked(1 << current);
            
            object IEnumerator.Current => current;
            
            public void Dispose() { }

            public IEnumerator<int> GetEnumerator()
            {
                return new BitEnum() { value = value, current = -1, targetIsOne = targetIsOne, posMode = posMode };
            }

            public bool MoveNext()
            {
                for(int i = current + 1; i < bitCount; i++)
                {
                    if(((unchecked(1L << i) & value) == 0) ^ targetIsOne)
                    {
                        current = i;
                        return true;
                    }
                }
                return false;
            }

            public void Reset()
            {
                current = -1;
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        public BitEnum EnumOnePos() => new BitEnum() { value = this.value, current = -1, targetIsOne = true, posMode = true };
        public BitEnum EnumZeroPos() => new BitEnum() { value = this.value, current = -1, targetIsOne = false, posMode = true };
        
        public BitEnum EnumOnes() => new BitEnum() { value = this.value, current = -1, targetIsOne = true, posMode = false };
        public BitEnum EnumZeros() => new BitEnum() { value = this.value, current = -1, targetIsOne = false, posMode = false };
        
        
        public int BitAt(int k)
        {
            return ValueAt(k) == 0 ? 0 : 1;
        }
        
        public bool BoolAt(int k)
        {
            return ValueAt(k) != 0;
        }
        
        public int ValueAt(int k)
        {
            return unchecked(1 << k) & value;
        }
        
        public bool this[int k]
        {
            get => this.ValueAt(k) != 0;
        }
        
        public BitValue WithBit(int k, bool v = true)
        {
            var zz = this.value & ~unchecked(1 << k);
            if(v) zz |= unchecked(1 << k);
            return new BitValue(zz);
        }
        
        public override string ToString() => value.ToString();
        public string ToString(string format) => value.ToString(format);
        
        
        [ThreadStatic]
        static StringBuilder sb;
        
        public string To01String()
        {
            if(sb == null) sb = new StringBuilder();
            sb.Clear();
            sb.Append('[');
            for(int i = 0; i < bitCount; i++)
            {
                sb.Append(this.BoolAt(i) ? '1' : '0');
                if((i + 1) % 8 == 0 && i != bitCount - 1) sb.Append(' ');
            }
            sb.Append(']');
            return sb.ToString();
        }
        
        
        
        public static void UnitTest(Action<string> output = null)
        {
            if(output == null) output = System.Console.WriteLine;
            
            var x = new BitValue(0).WithBit(12, true).WithBit(14, true);
            output(x.To01String());
            output(x.ToString());
            
            
            var y = new BitValue(0).WithBit(16, true).WithBit(19, true);
            output(y.To01String());
            output((~x & ~y).To01String());
            output((x | y).To01String());
            
            foreach(var t in (x | y).EnumOnePos())
            {
                output(t.ToString());
            }
        }
        
    }
    
    
}
