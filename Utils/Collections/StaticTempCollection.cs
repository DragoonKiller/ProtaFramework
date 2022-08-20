using System;
using System.Collections.Generic;

namespace Prota
{
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
    }
}