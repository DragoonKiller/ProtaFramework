using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static T Last<T>(this T[] l) => l[l.Length - 1];
        
        public static T Last<T>(this T[] l, T v) => l[l.Length - 1] = v;
        
    }
}