using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Prota
{
    public interface ISerializableDictionary
    {
        Type keyType { get; }
        Type valueType { get; }
    }
    
    // 使用自定义链表.
    // 必须使用 继承 + SerializeField 才能正确序列化.
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializableDictionary, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        readonly static EqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;
        
        readonly static EqualityComparer<TKey> keyComparer = EqualityComparer<TKey>.Default;
        
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
                foreach(var k in dict.entries.keys) yield return dict.entries[k].key;
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
                foreach(var v in this)
                    if(valueComparer.Equals(item, v))
                        return true;
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
                foreach(var k in dict.entries.keys) yield return dict.entries[k].value;
            }
            
            public bool Remove(TValue item) => throw new NotSupportedException();
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
        
        [Serializable]
        public struct Entry
        {
            [SerializeField] public TKey key;
            [SerializeField] public TValue value;
            [SerializeField] public int next;

            public Entry(TKey key, TValue value, int next)
            {
                this.key = key;
                this.value = value;
                this.next = next;
            }
        }
        
        // 一个可复用节点集合.
        // 有效节点个数(count)就是 hashmap 元素个数.
        [SerializeField] SerializableLinkedList<Entry> _entries = new SerializableLinkedList<Entry>();
        
        // 拉链法哈希表, 这些是链表表头, 一个表头就是一个桶(bucket).
        [SerializeField] List<int> heads = new List<int>();
        
        public SerializableLinkedList<Entry> entries => _entries;
        int modnum => heads.Count;
        public ICollection<TKey> Keys => new KeysCollection(this);
        public ICollection<TValue> Values => new ValuesCollection(this);
        public int Count => entries.Count;
        public bool IsReadOnly => false;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => new KeysCollection(this);
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => new ValuesCollection(this);

        public Type keyType => typeof(TKey);

        public Type valueType => typeof(TValue);

        public TValue this[TKey key]
        {
            get
            {
                if(TryGetValue(key, out var res)) return res;
                throw new ArgumentException($"key not found: { key }");
            }
            set
            {
                if(TryGetEntry(key, out var index))
                {
                    // 替换 value.
                    entries[index].value = value;
                    return;
                }
                else AddInternal(key, value);
            }
        }
        
        bool TryGetEntry(TKey key, out SerializableLinkedListKey index)
        {
            index = default;
            if(entries.Count == 0) return false;
            var bucketIndex = key.GetHashCode().Repeat(modnum);
            var head = entries.GetIndex(heads[bucketIndex]);
            
            // Debug.Log($"searching[{ key }] hash&index[{ bucketIndex }] bucket[{ head }]");
            
            // 遍历该桶的链表.
            for(var i = head; i.valid; i = entries.GetIndex(entries[i].next))
            {
                // Debug.Log($"searching[{ key }] visit { i }");
                
                var entry = entries[i];
                if(keyComparer.Equals(entry.key, key))
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
        
        public void Add(TKey key, TValue value)
        {
            if(TryGetEntry(key, out var index)) throw new Exception("entry already exists.");
            AddInternal(key, value);
        }
        
        public void Add(TKey key, TValue value, bool ignoreDuplicate)
        {
            if(ignoreDuplicate) this[key] = value;
            else Add(key, value);
        }
        
        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach(var i in collection) Add(i.Key, i.Value);
        }
        
        public void AddRange<G>(IEnumerable<G> a, Func<G, TKey> keySelector, Func<G, TValue> valueSelector)
        {
            foreach(var i in a) Add(keySelector(i), valueSelector(i));
        }
        
        void AddInternal(TKey key, TValue value)
        {
            if(entries.Count >= heads.Count) Rehash();
            
            // 新增一个节点.
            var newIndex = entries.Take();
            var bucketIndex = key.GetHashCode().Repeat(modnum);
            entries[newIndex] = new Entry(key, value, heads[bucketIndex]);
            heads[bucketIndex] = newIndex.id;
        }
        
        void Rehash()
        {
            var nextN = Mathf.CeilToInt(heads.Count * 1.6f).Max(baseCapacity);
            
            heads = new List<int>().Fill(nextN, -1);
            
            foreach(var k in entries.keys)
            {
                ref Entry e = ref entries[k];
                
                // 这是新的 hashcode.
                var bucketIndex = e.key.GetHashCode().Repeat(modnum);
                
                // 遍历到的 next 都是要改的. 接到表头那边去.
                e.next = heads[bucketIndex];
                
                // 重新整理表头.
                heads[bucketIndex] = k.id;
                
                // Debug.Log($"rehash[{ e.key }] newhash[{ bucketIndex }] [{ k }]next[{ e.next }]");
            }
        }

        public bool ContainsKey(TKey key) => TryGetEntry(key, out _);
        
        public bool Remove(TKey key)
        {
            var bucketIndex = key.GetHashCode().Repeat(modnum);
            var index = entries.GetIndex(heads[bucketIndex]);
            
            if(!index.valid) return false; // 整个链表没有那个元素.
            
            for(var prev = SerializableLinkedListKey.none; index.valid; index = entries.GetIndex(entries[index].next))
            {
                // 找到了想要删除的元素.
                if(keyComparer.Equals(key, entries[index].key))
                {
                    var next = entries[index].next;
                    if(!prev.valid)        // 想要删除的元素是表头.
                    {
                        heads[bucketIndex] = next;    // 直接接上表头就行.
                    }
                    else
                    {
                        entries[prev].next = next;    // 让上一个节点跳过这个元素.
                    }
                    entries.Release(index);   // 要把元素删除.
                    return true;
                }
                prev = index;
            }
            
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default;
            if(!TryGetEntry(key, out var index)) return false;
            value = entries[index].value;
            return true;
        }

        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            heads = new List<int>();
            _entries = new SerializableLinkedList<Entry>();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if(!TryGetEntry(item.Key, out var index)) return false;
            if(!valueComparer.Equals(entries[index].value, item.Value)) return false;
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int c = 0;
            foreach(var i in this) if(c + arrayIndex < array.Length)
            {
                array[c + arrayIndex] = i;
                c++;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if(!TryGetEntry(item.Key, out var index)) return false;
            if(!entries[index].value.Equals(item.Value)) return false;
            var next = entries[index].next;
            var bucketIndex = item.Key.GetHashCode().Repeat(modnum);
            var head = entries.GetIndex(heads[bucketIndex]);
            if(head == index) heads[bucketIndex] = next;
            entries.Release(index);
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach(var k in entries.keys)
            {
                // Debug.Log($"get key { k } : { entries[k].key }=>{ entries[k].value }");
                yield return new KeyValuePair<TKey, TValue>(entries[k].key, entries[k].value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        
        public string InternalArrayToString()
        {
            var sb = new StringBuilder();
            sb.Append("heads: \n");
            for(int i = 0; i < heads.Count; i++) sb.Append($":: i[{ i }] head[{ heads[i] }]\n");
            sb.Append("\n");
            sb.Append("entries: \n");
            foreach(var k in entries.keys)
            {
                var i = entries[k];
                sb.Append($":: index[{ k }] key[{ i.key }] value[{ i.value }] next[{ i.next }] khash[{ i.key.GetHashCode().Repeat(modnum) }]\n");
            }
            sb.Append("\n");
            sb.Append(entries.InternalArrayToString());
            sb.Append("\n");
            return sb.ToString();
        }
    }
    
    public static partial class UnityUnitTest
    {
        public static void UnitTestSerializableDictionary()
        {
            var dictionary = new SerializableDictionary<int, string>();
            
            // Test Add
            dictionary.Add(1, "one");
            ("one" == dictionary[1]).Assert();

            // Test Remove
            dictionary.Remove(1);
            (!dictionary.ContainsKey(1)).Assert();

            // Test Clear
            dictionary.Add(1, "one");
            dictionary.Clear();
            (0 == dictionary.Count).Assert();

            // Test Count
            dictionary.Add(1, "one");
            dictionary.Add(2, "two");
            (2 == dictionary.Count).Assert();
            
            // Test ContainsKey
            dictionary.ContainsKey(1).Assert();
            dictionary.ContainsKey(2).Assert();
            (!dictionary.ContainsKey(3)).Assert();
            
            // Test TryGetValue
            dictionary.TryGetValue(1, out var one);
            ("one" == one).Assert();
            dictionary.TryGetValue(2, out var two);
            ("two" == two).Assert();
            dictionary.TryGetValue(3, out var three);
            (null == three).Assert();
            
            // Test Enumerate.
            var count = 0;
            foreach(var i in dictionary)
            {
                count++;
            }
            (2 == count).Assert();
            
            var c = dictionary.ToArray();
            (c[0].Key != c[1].Key).Assert();
            (c[0].Key == 1 || c[0].Key == 2).Assert();
            (c[1].Key == 2 || c[1].Key == 1).Assert();
            
            (c[0].Value != c[1].Value).Assert();
            (c[0].Value == "one" || c[1].Value == "one").Assert();
            (c[0].Value == "two" || c[1].Value == "two").Assert();
            
            
            var ax = new Dictionary<string, string>();
            var bx = new SerializableDictionary<string, string>();
            
            for(int i = 0; i < 1000; i++)
            {
                var k = UnityEngine.Random.Range(0, 100).ToString();
                var v = UnityEngine.Random.Range(0, 100).ToString();
                ax[k] = v;
                bx[k] = v;
                // Debug.Log($"adding { i } :: { k } = { v }");
                // Debug.Log($"{ bx.InternalArrayToString() }");
                for(int j = 0; j < 100; j++)
                {
                    (ax.ContainsKey(j.ToString()) == bx.ContainsKey(j.ToString())).PassValue(out var xx).Assert(j.ToString());
                }
            }
            
            for(int i = 0; i < 1000; i++)
            {
                if(i % 2 == 0) continue;
                var k = i.ToString();
                ax.Remove(k);
                bx.Remove(k);
                for(int j = 0; j < 1000; j++) (ax.ContainsKey(j.ToString()) == bx.ContainsKey(j.ToString())).Assert(j.ToString());
                
                var z = UnityEngine.Random.Range(0, 2000);
                (ax.ContainsKey(z.ToString()) == bx.ContainsKey(z.ToString())).Assert();
            }
        }
        
    }
}
