using System;
using UnityEngine;

using System.Collections.Generic;

namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        public static T GetOrCreate<T>(this GameObject g) where T : Component
        {
            if(g.TryGetComponent<T>(out var res)) return res;
            var r = g.AddComponent<T>();
            return r;
        }
        
        public static string GetNamePath(this GameObject g) => g.transform.GetNamePath();
    }
    
}