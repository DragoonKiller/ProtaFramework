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
        
    }
}