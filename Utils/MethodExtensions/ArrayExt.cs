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
        
        public static ArraySegment<T> AsSegment<T>(this T[] arr, int start = 0, int? count = null) => new ArraySegment<T>(arr, start, count ?? arr.Length - start);
        
        
        public static T[] Fill<T>(this T[] list, Func<int, T> content)
        {
            for(int i = 0; i < list.Length; i++)
            {
                list[i] = content(i);
            }
            return list;
        }
        
        public static T[,] Fill<T>(this T[,] list, Func<int, int, T> content)
        {
            for(int i = 0; i < list.GetLength(0); i++)
            for(int j = 0; j < list.GetLength(1); j++)
            {
                list[i, j] = content(i, j);
            }
            return list;
        }
        
        
        public static T[,,] Fill<T>(this T[,,] list, Func<int, int, int, T> content)
        {
            for(int i = 0; i < list.GetLength(0); i++)
            for(int j = 0; j < list.GetLength(1); j++)
            for(int k = 0; k < list.GetLength(2); k++)
            {
                list[i, j, k] = content(i, j, k);
            }
            return list;
        }
        
        public static T[] Shuffle<T>(this T[] list, int? count)
        {
            var n = count ?? list.Length;
            for(int i = 0; i < n; i++)
            {
                var a = rd.Next(0, n);
                var t = list[a];
                list[a] = list[i];
                list[i] = t;
            }
            return list;
        }
    }
}