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
        
        
        public static F SetSync<K, G, H, F>(
            this F target,
            Dictionary<K, H> val,
            Func<K, H, G> newFunc,
            Action<K, G, H> updateFunc,
            Action<K, G> removeFunc
        ) where F: IDictionary<K, G>
        {
            // 新增.
            foreach(var e in val)
            {
                if(!target.ContainsKey(e.Key))
                {
                    target.Add(e.Key, newFunc(e.Key, e.Value));
                }
            }
            
            var temp = StaticTempList<K>.list;
            
            foreach(var e in target)
            {
                // 有 key 的更新.
                if(val.ContainsKey(e.Key))
                {
                    updateFunc(e.Key, e.Value, val[e.Key]);
                }
                // 没 key 的取消.
                else
                {
                    removeFunc(e.Key, e.Value);
                    temp.Add(e.Key);
                }
            }
            
            // 没有的删除.
            foreach(var k in temp) target.Remove(k);
            
            return target;
        }
        
    }
}