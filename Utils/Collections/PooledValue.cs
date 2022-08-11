using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;

namespace Prota
{
    public class PooledValue<T>
    {
        public int count => values.Count;
        public int used { get; private set; }
        readonly HashSet<T> values = new HashSet<T>();
        readonly Func<int, T> onCreate;
        public PooledValue(Func<int, T> onCreate) => this.onCreate = onCreate;
        public T Get()
        {
            if(values.Count == 0)
            {
                var ret = onCreate(count + 1);
                used += 1;
                return ret;
            }
            
            var v = values.First();
            values.Remove(v);
            return v;
        }
        public PooledValue<T> Return(T value)
        {
            values.Add(value);
            return this;
        }
    }
}