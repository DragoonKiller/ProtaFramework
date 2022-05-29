using System.Collections.Generic;

namespace Prota
{
    
    public class StaticTempList<T>
    {
        static List<T> list = new List<T>();
        public static List<T> Get()
        {
            list.Clear();
            return list;
        }
        
        
        public static void Clear() => list.Clear();
    }
    
    public static class StaticTempDictionary<K, V>
    {
        static Dictionary<K, V> dict = new Dictionary<K, V>();
        public static Dictionary<K, V> Get()
        {
            dict.Clear();
            return dict;
        }
        
        public static void Clear() => dict.Clear();
    }
}