using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System;

namespace Prota.Editor
{
    public static partial class MethodExtensions
    {
        public static void GetAndModify<T>(this List<T> a, int i, Func<T, T> f)
        {
            a[i] = f(a[i]);
        }
        
    }
}