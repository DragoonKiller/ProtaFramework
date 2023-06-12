using System;
using System.Collections.Generic;

namespace Prota
{
    [Serializable]
    public struct CollectionCache<K, V>
    {
        public Dictionary<K, V> cache { get; private set; }
        
        public IEnumerable<V> listInUse { get; private set; }
        
        public Func<IEnumerable<V>> listGetter { get; private set; }
        
        public V this[K index]
        {
            get
            {
                if(cache == null) cache = new Dictionary<K, V>();
                
                var list = listGetter();
                if(this.listInUse != list)
                {
                    cache.Clear();
                    this.listInUse = list;
                }
                
                if(cache.TryGetValue(index, out V value)) return value;
                value = getter(index);
                cache.Add(index, value);
                return value;
            }
        }
        
        Func<K, V> getter;
        
        public CollectionCache<K, V> Setup(Func<K, V> getter)
        {
            this.getter = getter;
            return this;
        }
        
        public CollectionCache<K, V> Setup(IEnumerable<V> list, Func<V, K> keyGetter)
        {
            return Setup(() => list, keyGetter);
        }
        
        public CollectionCache<K, V> Setup(Func<IEnumerable<V>> listGetter, Func<V, K> keyGetter)
        {
            this.listGetter = listGetter;
            this.getter = key =>
            {
                var list = listGetter();
                foreach (var item in list)
                {
                    if(keyGetter(item).Equals(key)) return item;
                }
                return default;
            };
            return this;
        }
        
        
        public CollectionCache<K, V> Invalidate()
        {
            cache?.Clear();
            return this;
        }
    }
}
