using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Prota.Unity
{
    
    /// <summary>
    /// 快速查询; 添加和删除是平方级的, 谨慎修改.
    /// </summary>
    [Serializable]
    public class SerializableDictionary<K, V> : IDictionary<K, V> where V : ISerializable
    {
        [NonSerialized]
        public bool sync = false;       // 每次重新加载脚本, 这个值会被重置为默认值 false.
        
        public List<K> keys = new List<K>();
        public List<V> values = new List<V>();
        
        public Dictionary<K, V> content = new Dictionary<K, V>();
        
        
        
        public V this[K key] {
            get 
            {
                Sync();
                return content[key];
            }
            set
            {
                Sync();
                content[key] = value;
            }
        }

        public ICollection<K> Keys => keys;

        public ICollection<V> Values => values;

        public int Count => keys.Count;

        public bool IsReadOnly => false;

        public void Add(K key, V value)
        {
            Sync();
            keys.Add(key);
            values.Add(value);
            content.Add(key, value);
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item) => Add(item.Key, item.Value);
        
        public void Clear()
        {
            sync = true;
            keys.Clear();
            values.Clear();
            content.Clear();
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
        {
            Sync();
            return content.TryGetValue(item.Key, out var g) && g.Equals(item.Value);
        }

        public bool ContainsKey(K key)
        {
            Sync();
            return content.ContainsKey(key);
        }
        
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            Sync();
            var i = 0;
            foreach(var c in content) array[arrayIndex + (i++)] = c;
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => content.GetEnumerator();

        public bool Remove(K key)
        {
            Sync();
            var index = keys.IndexOf(key);
            if(index < 0) return false;
            content.Remove(key);
            keys.RemoveAt(index);
            values.RemoveAt(index);
            return true;
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item) => Remove(item.Key);

        public bool TryGetValue(K key, out V value)
        {
            Sync();
            return content.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        
        void Sync()
        {
            if(sync) return;
            sync = true;
            for(int i = 0; i < keys.Count; i++)
            {
                var k = keys[i];
                var v = values[i];
                content.Add(k, v);
            }
        } 
    }


}