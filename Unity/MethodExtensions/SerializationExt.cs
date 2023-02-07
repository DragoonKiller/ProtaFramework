using System;
using UnityEngine;
using System.Collections.Generic;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static void SerializeToList<K, V>(this Dictionary<K, V> dict, List<K> key, List<V> value)
        {
            key.Clear();
            value.Clear();
            foreach(var c in dict)
            {
                key.Add(c.Key);
                value.Add(c.Value);
            }
            // dict.Clear();
        }
        
        
        public static void DeserializeFromList<K, V>(this Dictionary<K, V> dict, List<K> key, List<V> value)
        {
            dict.Clear();
            for(int i = 0; i < key.Count; i++)
            {
                if(i >= value.Count) break;
                dict.Add(key[i], value[i]); 
            }
            key.Clear();
            value.Clear();
        }
        
    }
}