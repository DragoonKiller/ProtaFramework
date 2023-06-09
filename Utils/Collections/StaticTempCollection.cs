using System;
using System.Collections.Generic;

namespace Prota
{
    public class TempStack<T>
    {
        static Action<Stack<T>> onReturn = list => list.Clear(); 
        
        static TempStack()
        {
            ConcurrentPool<Stack<T>>.instance.onReturn = onReturn;
        }
        
        public static ConcurrentPool<Stack<T>>.Handle Get()
        {
            return ConcurrentPool<Stack<T>>.instance.Get();
        }
        
        public static ConcurrentPool<Stack<T>>.Handle Get(out Stack<T> value)
        {
            var handle = ConcurrentPool<Stack<T>>.instance.Get();
            value = handle.value;
            return handle;
        }
    }
    
    public class TempList<T>
    {
        static Action<List<T>> onReturn = list => list.Clear(); 
        
        static TempList()
        {
            ConcurrentPool<List<T>>.instance.onReturn = onReturn;
        }
        
        public static ConcurrentPool<List<T>>.Handle Get()
        {
            return ConcurrentPool<List<T>>.instance.Get();
        }
        
        public static ConcurrentPool<List<T>>.Handle Get(out List<T> value)
        {
            var handle = ConcurrentPool<List<T>>.instance.Get();
            value = handle.value;
            return handle;
        }
    }
    
    public class TempHashSet<T>
    {
        static Action<HashSet<T>> onReturn = list => list.Clear(); 
        
        static TempHashSet()
        {
            ConcurrentPool<HashSet<T>>.instance.onReturn = onReturn;
        }
        
        public static ConcurrentPool<HashSet<T>>.Handle Get()
        {
            return ConcurrentPool<HashSet<T>>.instance.Get();
        }
        
        public static ConcurrentPool<HashSet<T>>.Handle Get(out HashSet<T> value)
        {
            var handle = ConcurrentPool<HashSet<T>>.instance.Get();
            value = handle.value;
            return handle;
        }
    }
    
    public static class TempDict<K, V>
    {
        static Action<Dictionary<K, V>> onReturn = list => list.Clear(); 
        
        static TempDict()
        {
            ConcurrentPool<Dictionary<K, V>>.instance.onReturn = onReturn;
        }
        
        public static ConcurrentPool<Dictionary<K, V>>.Handle Get()
        {
            return ConcurrentPool<Dictionary<K, V>>.instance.Get();
        }
        
        public static ConcurrentPool<Dictionary<K, V>>.Handle Get(out Dictionary<K, V> value)
        {
            var handle = ConcurrentPool<Dictionary<K, V>>.instance.Get();
            value = handle.value;
            return handle;
        }
    }
}
