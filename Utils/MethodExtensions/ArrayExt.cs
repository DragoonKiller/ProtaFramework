using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static T Last<T>(this T[] l) => l[l.Length - 1];
        
        public static T Last<T>(this T[] l, T v) => l[l.Length - 1] = v;
        
        public static T[] Resize<T>(this T[] original, int size)
        {
            if(original == null) return new T[size <= 0 ? 4 : size];
            var arr = new T[size];
            for(int i = 0, limit = Math.Min(original?.Length ?? 0, arr.Length); i < limit; i++)
            {
                arr[i] = original[i];
            }
            return arr;
        }
        
    }
}