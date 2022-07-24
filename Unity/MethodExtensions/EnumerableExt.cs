using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;
using System.Buffers;
using System;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static void DestroyAll<T>(this IEnumerable<T> list) where T : UnityEngine.Object
        {
            foreach(var g in list) UnityEngine.Object.Destroy(g);
        }
        
        public static void DestroyAllImmediate<T>(this IEnumerable<T> list) where T : UnityEngine.Object
        {
            foreach(var g in list) UnityEngine.Object.DestroyImmediate(g);
        }
    }
    
}