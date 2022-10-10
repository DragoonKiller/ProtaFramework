using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static ref T Swap<T>(this ref T t, ref T value) where T: struct
        {
            var original = t;
            t = value;
            value = original;
            return ref t;
        }
        
        public static T SwapSet<T>(this ref T t, T value) where T: struct
        {
            var original = t;
            t = value;
            return original;
        }
        
    }
}