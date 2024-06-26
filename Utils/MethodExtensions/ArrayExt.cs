using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static bool IsNullOrEmpty<T>(this T[] list) => list == null || list.Length == 0;
        
        public static T Last<T>(this T[] l) => l[l.Length - 1];
        
        public static T LastOrDefault<T>(this T[] l) => l.Length == 0 ? default : l.Last();
        
        public static T Last<T>(this T[] l, T v) => l[l.Length - 1] = v;
        
        public static T[] Resize<T>(this T[] original, int size)
        {
            if(original == null) return new T[size <= 0 ? 4 : size];
            var arr = new T[size];
            for(int i = 0, limit = arr.Length.Min(original?.Length ?? 0); i < limit; i++)
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
        
        public static T[] Fill<T>(this T[] list, int start, int count, Func<int, T> content)
        {
            for(int i = start, limit = list.Length.Min(start + count); i < limit; i++)
            {
                list[i] = content(i);
            }
            return list;
        }
        
        public static T[,] Fill<T>(this T[,] list, int start1, int start2, int count1, int count2, Func<int, int, T> content)
        {
            for(int i = start1, limit1 = list.GetLength(0).Min(start1 + count1); i < limit1; i++)
            for(int j = start2, limit2 = list.GetLength(1).Min(start1 + count2); j < limit2; j++)
            {
                list[i, j] = content(i, j);
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
                var a = rand.Next(0, n);
                var t = list[a];
                list[a] = list[i];
                list[i] = t;
            }
            return list;
        }
        
        public static bool Contains<T>(this T[] list, T element)
        {
            for(int i = 0; i < list.Length; i++) if(
                EqualityComparer<T>.Default.Equals(list[i], element))
                    return true;
            return false;
        }
        
        public static T[] WithDuplicateRemoved<T>(this T[] list)
        {
            var l = new List<T>();
            for(int i = 0; i < list.Length; i++) if(!l.Contains(list[i])) l.Add(list[i]);
            return l.ToArray();
        }
        
        public static int FindIndex<T>(this T[] arr, Predicate<T> match)
        {
            for(int i = 0; i < arr.Length; i++) if(match(arr[i])) return i;
            return -1;
        }
        
        public static int FindIndex<T>(this T[] arr, T value)
        {
            for(int i = 0; i < arr.Length; i++) if(EqualityComparer<T>.Default.Equals(arr[i], value)) return i;
            return -1;
        }
        
        public static T[] Remove<T>(this T[] arr, T value)
        {
            return arr.Where(v => !EqualityComparer<T>.Default.Equals(v, value)).ToArray();
        }
        
        public static T[] Remove<T>(this T[] arr, Predicate<T> match)
        {
            return arr.Where(v => !match(v)).ToArray();
        }
        
        public static T[] RemoveAt<T>(this T[] arr, int index)
        {
            var l = new List<T>(arr);
            l.RemoveAt(index);
            return l.ToArray();
        }
        
        public static void CopyTo<T>(this T[] arr, T[] target)
        {
            if(arr == null) throw new ArgumentNullException("arr");
            if(target == null) throw new ArgumentNullException("target");
            var n = arr.Length.Min(target.Length);
            for(int i = 0; i < n; i++) target[i] = arr[i];
        }
        
        public static void CopyTo<T>(this T[] arr, int start, T[] target, int maxLength = int.MaxValue)
        {
            if(arr == null) throw new ArgumentNullException("arr");
            if(target == null) throw new ArgumentNullException("target");
            var n = (arr.Length - start).Min(maxLength).Min(target.Length);
            for(int i = 0; i < n; i++) target[i] = arr[start + i];
        }
        
        public static void CopyTo<T>(this T[] arr, int start, T[] target, int targetStart, int maxLength = int.MaxValue)
        {
            if(arr == null) throw new ArgumentNullException("arr");
            if(target == null) throw new ArgumentNullException("target");
            var n = (arr.Length - start).Min(maxLength).Min(target.Length);
            for(int i = 0; i < n; i++) target[targetStart + i] = arr[start + i];
        }
        
        public static T[] Fill<T>(this T[] arr, T value)
        {
            for(int i = 0; i < arr.Length; i++) arr[i] = value;
            return arr;
        }
        
        internal static Span<T> AsSpan<T>(this T[] arr) => new Span<T>(arr);
    }
}
