using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static HashSet<T> AddRange<T>(this HashSet<T> x, IEnumerable<T> g)
        {
            foreach(var i in g) x.Add(i);
            return x;
        }
        
        public static HashSet<T> RemoveRange<T>(this HashSet<T> x, IEnumerable<T> g)
        {
            foreach(var i in g) x.Remove(i);
            return x;
        }
        
        public static HashSet<T> Clone<T>(this HashSet<T> x)
        {
            return new HashSet<T>(x);
        }
        
    }
}
