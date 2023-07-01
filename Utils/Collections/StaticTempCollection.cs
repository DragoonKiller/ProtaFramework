using System;
using System.Collections.Generic;

namespace Prota
{
    public struct TempList
    {
        public static ConcurrentPool<List<T>>.Handle Get<T>(out List<T> value)
        {
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
            ConcurrentPool<Dictionary<K, V>>.instance.onReturn = list => list.Clear();
            var handle = ConcurrentPool<Dictionary<K, V>>.instance.Get();
            value = handle.value;
            return handle;
        }
    }
}
