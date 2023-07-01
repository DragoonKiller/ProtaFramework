using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;
using System.Collections;

namespace Prota
{
    [Serializable]
    public readonly struct SerializableLinkedListKey : IEquatable<SerializableLinkedListKey>
    {
        [SerializeField, Readonly] public readonly int id;
        [SerializeReference, Readonly] public readonly ISerializableLinkedList list;
        
        public SerializableLinkedListKey(int id, ISerializableLinkedList list)
        {
            this.id = id;
            this.list = list;
        }
        
        public bool Equals(SerializableLinkedListKey other) => id == other.id && list == other.list;
        
        public bool valid => list != null && id >= 0;
        
        public bool Valid(ISerializableLinkedList list) => this.list == list;
        
        public static SerializableLinkedListKey none => new SerializableLinkedListKey(-1, null);

        public override bool Equals(object obj) => obj is SerializableLinkedListKey other && Equals(other);
        public static bool operator==(SerializableLinkedListKey a, SerializableLinkedListKey b)
            => a.Equals(b);
        public static bool operator!=(SerializableLinkedListKey a, SerializableLinkedListKey b) => !a.Equals(b);
        public override int GetHashCode() => HashCode.Combine(id, list);
        
        // * 表示无效.
        public override string ToString() => $"LinkedListKey[{id}{ (list == null ? "*" : "") }]";
    }
    
    public interface ISerializableLinkedList { }
    
    // Unity serialization supported version of ArrayLinkedList.
    [Serializable]
    public class SerializableLinkedList<T> : ISerializableLinkedList, IEnumerable<T>, IReadOnlyCollection<T>, IEnumerable
    {
        [Serializable]
        struct InternalData
        {
            public T value;         // 存储的值.
            public int next;        // 下一个元素的下标. 没有填-1.
            public int prev;        // 上一个元素的下标. 没有填-1.
            public bool inuse;      // 该元素是否正在使用.
        }
        
        [SerializeField] InternalData[] data = null;
        
        // 使用中的元素数.
        [field: SerializeField]
        public int Count { get; private set; } = 0;         // count in use.
        
        // 数组容量.
        public int capacity => data.IsNullOrEmpty() ? 0 : data.Length;
        
        // 数据链表头下标. 没有数据则是-1.
        public int head = -1;
        
        // 没有数据的链表头下标. 数据填满了则是-1.
        public int freeHead = -1;
        
        // 还有多少个没有使用的节点.
        public int freeCount => capacity - Count;
        
        // 取元素.
        public ref T this[SerializableLinkedListKey i] => ref data[i.id].value;
        
        public IEnumerable<SerializableLinkedListKey> keys
        {
            get
            {
                for(int i = 0; i < capacity; i++)
                {
                    if(data[i].inuse) yield return new SerializableLinkedListKey(i, this);
                }
            }
        }
        
        // 在链表中新增一个元素s.
        public SerializableLinkedListKey Take()
        {
            if(freeCount == 0) Resize();
            System.Diagnostics.Debug.Assert(freeCount > 0);
            return Use();
        }
        
        // 释放一个链表中的元素.
        public bool Release(SerializableLinkedListKey i)
        {
            if(this != i.list) return false;
            if(i.id >= capacity) return false;
            if(!data[i.id].inuse) return false;
            Free(i.id);
            return true;
        }
        
        public SerializableLinkedListKey GetIndex(int i)
        {
            return new SerializableLinkedListKey(i, this);
        }
        
        public T ElementAt(int i)
        {
            if(!data[i].inuse) throw new IndexOutOfRangeException();
            return data[i].value;
        }
        
        public SerializableLinkedList<T> Clear()
        {
            data = null;
            Count = 0;
            head = freeHead = -1;
            return this;
        }
        
        // 池子扩容.
        void Resize()
        {
            const int initialSize = 4;
            
            var originalSize = data.IsNullOrEmpty() ? 0 : data.Length;
            int nextSize = originalSize == 0 ? initialSize : Mathf.CeilToInt(originalSize * 1.6f);
            
            data = data.Resize(nextSize);
            
            for(int i = originalSize; i < capacity; i++) Free(i);
        }
        
        
        void Free(int cur)
        {
            if(cur == -1) return;
            (0 <= cur && cur < capacity).Assert();
            if(data[cur].inuse)
            {
                var p = data[cur].prev;
                var n = data[cur].next;
                if(n != -1) data[n].prev = p;
                if(p != -1) data[p].next = n;
                if(head == cur) head = n;
                Count -= 1;
            }
            
            var ori = freeHead;
            data[cur].next = ori;
            if(ori != -1) data[ori].prev = cur;
            data[cur].prev = -1;
            freeHead = cur;
            
            data[cur].inuse = false;
        }
        
        SerializableLinkedListKey Use()
        {
            var cur = freeHead;
            freeHead = data[cur].next;
            if(freeHead != -1) data[freeHead].prev = -1;
            
            data[cur].next = head;
            data[cur].prev = -1;
            if(head != -1) data[head].prev = cur;
            head = cur;
            
            data[cur].inuse = true;
            Count += 1;
            return new SerializableLinkedListKey(cur, this);
        }
        
        
        public struct IndexEnumerator : IEnumerator<SerializableLinkedListKey>
        {
            public int index;
            public SerializableLinkedList<T> list;
            public SerializableLinkedListKey Current => new SerializableLinkedListKey(index, list);
            object IEnumerator.Current => index;

            public void Dispose() => index = -1;

            public bool MoveNext()
            {
                if(index == -1) index = list.head;
                else index = list.data[index].next;
                if(index == -1) return false;
                return true;
            }

            public void Reset() => index = -1;
        }

        public struct IndexEnumerable : IEnumerable<SerializableLinkedListKey>
        {
            public SerializableLinkedList<T> list;
            
            public IEnumerator<SerializableLinkedListKey> GetEnumerator() => new IndexEnumerator() { index = -1, list = list };

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        public IndexEnumerable EnumerateKey() => new IndexEnumerable() { list = this };

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var index in EnumerateKey())
            {
                yield return data[index.id].value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Valid(SerializableLinkedListKey key)
        {
            if(key.list != this) return false;
            if(key.id >= capacity) return false;
            return true;
        }
        
        public string InternalArrayToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("[");
            for(int i = 0; i < capacity; i++)
            {
                if(i != 0) sb.Append("\n");
                sb.Append($"{i} :: prev[{ data[i].prev }] next[{ data[i].next }] inuse[{ data[i].inuse }]");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
    
    public static partial class UnitTest
    {
        public static void UnitTestSerializableLinkedList(Action<string> log)
        {
            var ax = new ArrayLinkedList<(int a, int b, int c)>();
            var a1 = ax.Take();
            var a2 = ax.Take();
            var a3 = ax.Take();
            log(a1.ToString());                 // 3, reversed arrangement.
            log(a2.ToString());                 // 2
            log(a3.ToString());                 // 1
            log(ax.capacity.ToString());        // 4
            log(ax.freeCount.ToString());       // 1
            
            var a4 = ax.Take();
            var a5 = ax.Take();
            log(a4.ToString());                 // 0
            log(a5.ToString());                 // n - 1
            log(ax.capacity.ToString());        // n
            log(ax.freeCount.ToString());       // n - 5
            
            ax.Release(a4);
            log(ax.freeCount.ToString());       // n - 4
            
            ax.Release(a2);
            log(ax.Count.ToString());           // 3
            log(ax.freeCount.ToString());       // n - 3
            
            
            ax[a1].a = 12;
            ax[a1].b = 13;
            ax[a1].c = 14;
            log(ax[a1].a.ToString());           // 12
            log(ax[a1].b.ToString());           // 13
            log(ax[a1].c.ToString());           // 14
            
            foreach(var i in ax.EnumerateKey())
            {
                log(i.ToString());
            }
            
            {
                var list = new SerializableLinkedList<int>();
                var set = new HashSet<int>();
                
                // 添加元素
                for (int i = 0; i < 100; i++)
                {
                    var key = list.Take();
                    list[key] = i;
                    set.Add(i);
                }
                
                // 检查元素数量
                Debug.Assert(list.Count == set.Count);
                
                // 检查元素是否相同
                foreach(var item in list) Debug.Assert(set.Contains(item));
                foreach(var i in list.keys) Debug.Assert(set.Contains(list[i]));
                
                // 删除元素
                foreach (var key in list.keys.ToArray())
                {
                    if(key.id % 2 != 0) continue;
                    // Debug.Log($"remove { key.id } start { list.freeHead } { list.head }\n{ list.InternalArrayToString() }");
                    var val = list[key];
                    list.Release(key);
                    // Debug.Log($"remove { key.id } finish { list.freeHead } { list.head }\n{ list.InternalArrayToString() }");
                    set.Remove(val);
                    // 检查元素数量
                    Debug.Assert(list.Count == set.Count);
                }
                
                // 检查元素是否相同
                foreach (var item in list) Debug.Assert(set.Contains(item));
            }
        }
    }

}
