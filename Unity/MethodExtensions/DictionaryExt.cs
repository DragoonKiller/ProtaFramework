using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        public static Dictionary<K, V> GetOrCreate<K, V>(this Dictionary<K, V> d, K key, out V val) where V: new()
        {
            if(d.TryGetValue(key, out val)) return d;
            val = d[key] = new V();
            return d;
        }
    }
}