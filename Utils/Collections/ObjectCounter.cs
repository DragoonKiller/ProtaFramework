using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Runtime.Serialization;
using System.Collections;

namespace Prota
{
    public class ObjectCounter<T>
    {
        readonly Dictionary<T, int> x = new Dictionary<T, int>();
        
        public int this[T t]
        {
            get
            {
                if(x.TryGetValue(t, out var v)) return v;
                return 0;
            }
            set
            {
                if(value == 0) x.Remove(t);
                else x[t] = value;
            }
        }
        
        public void Inc(T t, int v = 1)
        {
            if(x.TryGetValue(t, out var vv)) x[t] = vv + v;
            else x[t] = v;
        }
        
        public bool Dec(T t, int v = 1)
        {
            if(x.TryGetValue(t, out var vv))
            {
                if(vv <= v) x.Remove(t);
                else x[t] = vv - v;
                return true;
            }
            return false;
        }
    }

}
