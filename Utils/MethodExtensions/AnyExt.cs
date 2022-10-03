using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        static Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
        
        public static T SwapSet<T>(this ref T t, T value) where T: struct
        {
            var original = t;
            t = value;
            return original;
        }
        
    }
}