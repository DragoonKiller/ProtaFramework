using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        public static void GetAndModify<T>(this List<T> a, int i, Func<T, T> f)
        {
            a[i] = f(a[i]);
        }
        
        public static List<T> Sorted<T>(this List<T> l, IComparer<T> c)
        {
            l.Sort(c);
            return l;
        }
        
        public static List<T> Sorted<T>(this List<T> l, Comparison<T> c)
        {
            l.Sort(c);
            return l;
        }
        
        public static T Last<T>(this List<T> l) => l[l.Count - 1];
        
        public static T Last<T>(this List<T> l, T v) => l[l.Count - 1] = v;
        
        
        public static bool TryGetValue<T>(this List<T> l, int i, out T x)
        {
            if(0 <= i && i < l.Count)
            {
                x = l[i];
                return true;
            }
            x = default;
            return false;
        }
        
        public static List<T> RemoveBySwap<T>(this List<T> l, int i)
        {
            var temp = l[l.Count - 1];
            l[l.Count - 1] = l[i];
            l[i] = temp;
            l.RemoveAt(l.Count - 1);
            return l;
        }
        
        public static IList<T> RemoveLast<T>(this IList<T> l)
        {
            l.RemoveAt(l.Count - 1);
            return l;
        }
        
        public static IList<T> SetListLength<T>(List<T> l, int n, Func<int, T> onCreate, Action<int, T> onDisable, Action<int, T> onEnable)
        {
            for(int i = 0; i < n; i++)
            {
                if(i >= l.Count)
                {
                    l.Add(onCreate(i));
                }
                
                onEnable(i, l[i]);
            }
            
            for(int i = n; i < l.Count; i++) onDisable(i, l[i]);
            return l;
        }
    }
}