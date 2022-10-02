using System.Collections;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static Dictionary<K, V> GetOrCreate<K, V>(this Dictionary<K, V> d, K key, out V val) where V: new()
        {
            if(d.TryGetValue(key, out val)) return d;
            val = d[key] = new V();
            return d;
        }
        
        // dictionary K => H pairing with K => G providing H.
        public static F SetSync<K, G, V, F>(
            this F target,
            GetKVEnumerableFunc<K, V> getEnumerable,
            TryGetValueFunc<K, V> tryGetValue,
            Func<K, V, G> newFunc,
            Action<K, V, G> updateFunc,
            Action<K, G> removeFunc
        ) where F: IDictionary<K, G>
        {
            // 新增.
            foreach(var e in getEnumerable())
            {
                if(!target.ContainsKey(e.Key))
                {
                    target.Add(e.Key, newFunc(e.Key, e.Value));
                }
            }
            
            using(var temp = TempList<K>.Get())
            {
                foreach(var e in target)
                {
                    // 有 key 的更新.
                    if(tryGetValue(e.Key, out var v))
                    {
                        updateFunc(e.Key, v, e.Value);
                    }
                    // 没 key 的取消.
                    else
                    {
                        removeFunc(e.Key, e.Value);
                        temp.value.Add(e.Key);
                    }
                }
                
                // 没有的删除.
                foreach(var k in temp.value) target.Remove(k);
            }
            
            return target;
        }
        
        public static Dictionary<K, V> Clone<K, V>(this Dictionary<K, V> x)
        {
            return new Dictionary<K, V>(x);
        }
    }
}
