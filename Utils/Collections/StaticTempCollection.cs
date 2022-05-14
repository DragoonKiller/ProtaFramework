using System.Collections.Generic;

namespace Prota
{
    
    public static class StaticTempList<T>
    {
        static List<T> _list = new List<T>();
        public static List<T> list
        {
            get
            {
                _list.Clear();
                return _list;
            }
        }
    }
    
    public static class StaticTempDictionary<K, V>
    {
        static Dictionary<K, V> _dict = new Dictionary<K, V>();
        public static Dictionary<K, V> dict
        {
            get
            {
                return _dict;
            }
        }
    }
}