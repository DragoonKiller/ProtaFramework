using System;
using System.Collections.Generic;

namespace Prota
{
    public struct TempList
    {
        public static ConcurrentPool<List<T>>.Handle Get<T>(out List<T> value)
        {
            if(ConcurrentPool<List<T>>.instance.onReturn == null)
                ConcurrentPool<List<T>>.instance.onReturn = list => list.Clear();
            var handle = ConcurrentPool<List<T>>.instance.Get();
            value = handle.value;
            return handle;
        }
    }
    
    public struct TempHashSet
    {
        public static ConcurrentPool<HashSet<T>>.Handle Get<T>(out HashSet<T> value)
        {
            if(ConcurrentPool<List<T>>.instance.onReturn == null)
                ConcurrentPool<HashSet<T>>.instance.onReturn = list => list.Clear();
            var handle = ConcurrentPool<HashSet<T>>.instance.Get();
            value = handle.value;
            return handle;
        }
    }
    
    public struct TempDict
    {
        public static ConcurrentPool<Dictionary<K, V>>.Handle Get<K, V>(out Dictionary<K, V> value)
        {
            if(ConcurrentPool<Dictionary<K, V>>.instance.onReturn == null)
                ConcurrentPool<Dictionary<K, V>>.instance.onReturn = list => list.Clear();
            var handle = ConcurrentPool<Dictionary<K, V>>.instance.Get();
            value = handle.value;
            return handle;
        }
    }
    
    public static partial class MethodExtensions
    {
        public static ConcurrentPool<Dictionary<K, V>>.Handle ToTempDict<K, V>(this IEnumerable<KeyValuePair<K, V>> dict, out Dictionary<K, V> value)
        {
            var res = TempDict.Get(out value);
            foreach (var pair in dict)
            {
                value.Add(pair.Key, pair.Value);
            }
            return res;
        }
        
        public static ConcurrentPool<Dictionary<K, V>>.Handle ToTempDict<A, K, V>(this IEnumerable<A> dict, Func<A, K> keySelector, Func<A, V> valSelector, out Dictionary<K, V> value)
        {
            var res = TempDict.Get(out value);
            foreach (var item in dict)
            {
                value.Add(keySelector(item), valSelector(item));
            }
            return res;
        }
        
        
        
        public static ConcurrentPool<List<T>>.Handle ToTempList<T>(this IEnumerable<T> list, out List<T> value)
        {
            var res = TempList.Get(out value);
            foreach (var item in list)
            {
                value.Add(item);
            }
            return res;
        }
        
        public static ConcurrentPool<HashSet<T>>.Handle ToTempHashSet<T>(this IEnumerable<T> hashSet, out HashSet<T> value)
        {
            var res = TempHashSet.Get(out value);
            foreach (var item in hashSet)
            {
                value.Add(item);
            }
            return res;
        }
        
        
    }
}
