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
        
        // 集合映射.
        // 同步目标是一个提供 IEnumerator<KeyValuePair<K, V>> 和 TryGetValue(K, out V) 的字典类结构(不必是字典).
        // 同步者是 IDictionary<K, G> target.
        // 同步时需要提供 V => G 的对映逻辑.
        public static F SyncData<K, G, V, F>(
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
            
            using var _ = TempList.Get<K>(out var temp);
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
                    temp.Add(e.Key);
                }
            }
            
            // 没有的删除.
            foreach(var k in temp) target.Remove(k);
            
            return target;
        }
        
        // 集合映射.
        // 同步目标是一个字典 IDictionary<K, V> dict.
        // 同步者是 IDictionary<K, G> target.
        // 同步时需要提供 V => G 的对映逻辑.
        public static F SyncData<K, V, G, F>(
            this F target,
            IDictionary<K, V> dict,
            Func<K, V, G> newFunc,
            Action<K, V, G> updateFunc,
            Action<K, G> removeFunc
        ) where F: IDictionary<K, G>
        {
            return target.SyncData(() => dict, dict.TryGetValue, newFunc, updateFunc, removeFunc);
        }
        
        public static Dictionary<K, V> Clone<K, V>(this Dictionary<K, V> x)
        {
            return new Dictionary<K, V>(x);
        }
        
        public static Dictionary<K, V> AddRange<K, V>(this Dictionary<K, V> x, IEnumerable<KeyValuePair<K, V>> y)
        {
            foreach(var e in y) x.Add(e.Key, e.Value);
            return x;
        }
        
        public static Dictionary<K, V> AddRange<K, V, P>(this Dictionary<K, V> x, IEnumerable<P> y, Func<P, K> fkey, Func<P, V> fval)
        {
            foreach(var e in y) x.Add(fkey(e), fval(e));
            return x;
        }
        
        
    }
}
