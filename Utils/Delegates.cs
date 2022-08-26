using System.Collections.Generic;

namespace Prota
{
    
    public delegate bool TryGetValueFunc<K, V>(K key, out V value);
    
    public delegate IEnumerable<T> GetEnumerableFunc<T>();
    
    public delegate IEnumerable<KeyValuePair<K, V>> GetKVEnumerableFunc<K, V>();
    
    public delegate IEnumerator<T> GetEnumeratorFunc<T>();
    
    public delegate IEnumerator<KeyValuePair<K, V>> GetKVEnumeratorFunc<K, V>();
    
}