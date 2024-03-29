using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Threading;
using System.Buffers;
using System;
using System.Linq;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static void SetActiveAll(this IEnumerable<UnityEngine.GameObject> obj, bool active = true)
        {
            if(obj == null) return;
            foreach(var o in obj) if(o != null) o.SetActive(active);
        }
        
        public static void ActiveDestroyAll(this IEnumerable<UnityEngine.GameObject> list)
        {
            if(list == null) return;
            foreach(var g in list) if(g != null) g.ActiveDestroy();
        }
        
    
        public static void DestroyAll<T>(this IEnumerable<T> list) where T : UnityEngine.Object
        {
            if(list == null) return;
            foreach(var g in list) if(g != null) UnityEngine.Object.Destroy(g);
        }
        
        public static void DestroyAllImmediate<T>(this IEnumerable<T> list) where T : UnityEngine.Object
        {
            if(list == null) return;
            foreach(var g in list) if(g != null) UnityEngine.Object.DestroyImmediate(g);
        }
        
        
    }
    
}
