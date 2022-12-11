using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;
using System.Collections;

namespace Prota
{
    // 参考 https://referencesource.microsoft.com/#mscorlib/system/collections/generic/dictionary.cs,cc27fcdd81291584,references
    // 使用自定义链表.
    // 必须使用 SerializedReference 才能正确序列化.
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        public const int baseCapacity = 6;
        
        struct KeysCollection : ICollection<TKey>, IReadOnlyCollection<TKey>
        {
            public readonly SerializableDictionary<TKey, TValue> dict;
            public KeysCollection(SerializableDictionary<TKey, TValue> dict) => this.dict = dict;
            public int Count => dict.Count;
            public bool IsReadOnly => false;
            public void Add(TKey item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(TKey item) => dict.ContainsKey(item);
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                int c = 0;
                foreach(var v in this) if(c + arrayIndex < array.Length)
                {
                    array[c + arrayIndex] = v;
                    c++;
                }
            }
            public void CopyTo(Array array, int index)
            {
                if(!(array is TKey[] target)) throw new NotSupportedException();
                this.CopyTo(target, index);
            }
            public IEnumerator<TKey> GetEnumerator()
            {
                var e = dict.entries;
                for(int i = 0; i < e.capacity; i++)
                {
                    if(!e.inUse[i]) continue;
                    yield return e.arr[i].key;
                }
            }
            public bool Remove(TKey item) => throw new NotSupportedException();
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        struct ValuesCollection : ICollection<TValue>, IReadOnlyCollection<TValue>
        {
            public readonly SerializableDictionary<TKey, TValue> dict;
            public ValuesCollection(SerializableDictionary<TKey, TValue> dict) => this.dict = dict;
            public int Count => dict.Count;
            public bool IsReadOnly => false;
            public void Add(TValue item) => throw new NotSupportedException();
            public void Clear() => throw new NotSupportedException();
            public bool Contains(TValue item)
            {
                foreach(var v in this) if(item.Equals(v)) return true;
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                int c = 0;
                foreach(var v in this) if(c + arrayIndex < array.Length)
                {
                    array[c + arrayIndex] = v;
                    c++;
                }
            }

            public void CopyTo(Array array, int index)
            {
                if(!(array is TValue[] target)) throw new NotSupportedException();
                this.CopyTo(target, index);
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                var e = dict.entries;
                for(int i = 0; i < e.capacity; i++)
                {
                    if(!e.inUse[i]) continue;
                    yield return e.arr[i].value;
                }
            }
            public bool Remove(TValue item) => throw new NotSupportedException();
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
        
        [Serializable]
        public struct Entry
        {
            public TKey key;
            public TValue value;
            public SerializableLinkedListKey? next;

            public Entry(TKey key, TValue value, SerializableLinkedListKey? next)
            {
                this.key = key;
                this.value = value;
                this.next = next;
            }
        }
        
        // 只是一个可复用节点集合, 我们不使用里面的链表结构.
        // 有效节点个数(count)就是 hashmap 元素个数.
        [SerializeField] SerializableLinkedList<Entry> _entries = new SerializableLinkedList<Entry>();
        public SerializableLinkedList<Entry> entries { get => _entries; }
        // 链表头元素的节点的下标.
        public List<SerializableLinkedListKey?> heads = new List<SerializableLinkedListKey?>().Fill(baseCapacity);
        int modnum => heads.Count;
        public ICollection<TKey> Keys => new KeysCollection(this);
        public ICollection<TValue> Values => new ValuesCollection(this);
        public int Count => entries.Count;
        public bool IsReadOnly => false;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => new KeysCollection(this);
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => new ValuesCollection(this);
        public TValue this[TKey key]
        {
            get
            {
                if(TryGetValue(key, out var res)) return res;
                throw new ArgumentException($"key not found: { key }");
            }
            set
            {
                if(TryGetEntry(key, out var headIndex, out var index))
                {
                    // 替换 value.
                    entries[index].value = value;
                    return;
                }
                else AddInternal(headIndex, key, value);
            }
        }
        
        bool TryGetEntry(TKey key, out int headIndex, out SerializableLinkedListKey index)
        {
            index = default;
            headIndex = key.GetHashCode() % modnum;
            var head = heads[headIndex];
            if(head.HasValue)       // 有条目, 不知道是不是这个 key.
            {
                for(var i = head; i.HasValue; i = entries[i.Value].next)
                {
                    var entry = entries[i.Value];
                    if(entry.key.Equals(key))
                    {
                        index = i.Value;
                        return true;
                    }
                }
            }
            return false;
        }
        
        int GetListId(TKey key) => key.GetHashCode() % modnum;
        
        public void Add(TKey key, TValue value)
        {
            if(TryGetEntry(key, out var headIndex, out var index)) throw new Exception("entry already exists.");
            AddInternal(headIndex, key, value);
        }
        
        void AddInternal(int headIndex, TKey key, TValue value)
        {
            if(entries.Count > heads.Count) Rehash();
            // 新增一个节点.
            var newIndex = entries.Take();
            entries[newIndex] = new Entry(key, value, heads[headIndex]);
            heads[headIndex] = newIndex;
        }
        
        void Rehash()
        {
            var nextN = Mathf.CeilToInt(heads.Count * 1.6f);
            
            heads.Clear();
            heads.Fill(nextN);
            
            for(int i = 0; i < entries.Count; i++)
            {
                var e = entries.arr[i];
                
                // 这是新的 hashcode.
                var headIndex = e.key.GetHashCode() % modnum;
                
                // 遍历到的 next 都是要改的. 接到表头那边去.
                e.next = heads[headIndex];
                
                // 重新整理表头.
                heads[headIndex] = new SerializableLinkedListKey(i, entries.version[i], entries);
            }
        }

        public bool ContainsKey(TKey key) => TryGetEntry(key, out _, out _);
        
        public bool Remove(TKey key)
        {
            var headIndex = GetListId(key);
            var index = heads[headIndex];
            if(!index.HasValue) return false; // 整个链表没东西, 没有那个元素.
            
            for(SerializableLinkedListKey? prev = null; index.HasValue; index = entries[index.Value].next)
            {
                // 找到了想要删除的元素.
                if(key.Equals(entries[index.Value].key))
                {
                    var next = entries[index.Value].next;
                    if(prev == null)        // 想要删除的元素是表头.
                    {
                        heads[headIndex] = next;    // 直接接上表头就行.
                    }
                    else
                    {
                        entries[prev.Value].next = next;    // 让上一个节点跳过这个元素.
                    }
                    entries.Release(index.Value);   // 要把元素删除.
                    return true;
                }
                prev = index;
            }
            
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default;
            if(!TryGetEntry(key, out _, out var index)) return false;
            value = entries[index].value;
            return true;
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            heads.Clear();
            heads.Fill(baseCapacity);
            entries.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if(!TryGetEntry(item.Key, out _, out var index)) return false;
            if(!entries[index].value.Equals(item.Value)) return false;
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int c = 0;
            foreach(var i in this) if(c + arrayIndex < array.Length)
            {
                array[c + arrayIndex] = i;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if(!TryGetEntry(item.Key, out var headIndex, out var index)) return false;
            if(!entries[index].value.Equals(item.Value)) return false;
            var next = entries[index].next;
            if(heads[headIndex]?.Equals(index) ?? false) heads[headIndex] = next;
            entries.Release(index);
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for(int i = 0; i < entries.capacity; i++) if(entries.inUse[i])
            {
                yield return new KeyValuePair<TKey, TValue>(entries.arr[i].key, entries.arr[i].value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        
        
        
        
        public static void UnitTest()
        {
            
        }
        
    }
}