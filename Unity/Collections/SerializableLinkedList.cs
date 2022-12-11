using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;
using System.Collections;

namespace Prota
{
    
    public struct SerializableLinkedListKey : IEquatable<SerializableLinkedListKey>
    {
        public readonly int id;
        public readonly int version;
        public readonly ISerializableLinkedList list;
        
        public SerializableLinkedListKey(int id, int version, ISerializableLinkedList list)
        {
            this.id = id;
            this.version = version;
            this.list = list;
        }
        
        public bool Equals(SerializableLinkedListKey other) => object.ReferenceEquals(list, other);
    }
    
    public interface ISerializableLinkedList { }
    
    // Unity serialization supported veersion of ArrayLinkedList.
    [Serializable]
    public class SerializableLinkedList<T> : ISerializableLinkedList, IEnumerable<T>, IReadOnlyCollection<T>, IEnumerable
    {
        // 数据数组.
        [SerializeField] public T[] arr = null;
        
        // 下一个元素的下标. 没有填-1.
        public int[] next = null;
        
        // 上一个元素的下标. 没有填-1.
        public int[] prev = null;
        
        // 该元素是否正在使用.
        public bool[] inUse = null;
        
        // 该元素的版本号.
        public int[] version = null;
        
        // 使用中的元素数.
        [field: SerializeField]
        public int Count { get; private set; } = 0;         // count in use.
        
        // 数组容量.
        public int capacity => arr.IsNullOrEmpty() ? 0 : arr.Length;
        
        // 数据链表头下标. 没有数据则是-1.
        public int head = -1;
        
        // 没有数据的链表头下标. 数据填满了则是-1.
        public int freeHead = -1;
        
        // 还有多少个没有使用的节点.
        public int freeCount => capacity - Count;
        
        // 取元素.
        public ref T this[SerializableLinkedListKey i] => ref arr[i.id];
        
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
            if(!inUse[i.id]) return false;
            if(version[i.id] != i.version) return false;
            Free(i.id);
            return true;
        }
        
        public SerializableLinkedList<T> Clear()
        {
            arr = null;
            next = prev = version = null;
            inUse = null;
            Count = 0;
            head = freeHead = -1;
            return this;
        }
        
        // 池子扩容.
        void Resize()
        {
            const int initialSize = 4;
            
            var originalSize = arr.IsNullOrEmpty() ? 0 : arr.Length;
            int nextSize = originalSize == 0 ? initialSize : Mathf.CeilToInt(originalSize * 1.6f);
            
            arr = arr.Resize(nextSize);
            next = next.Resize(nextSize);
            prev = prev.Resize(nextSize);
            inUse = inUse.Resize(nextSize);
            version = version.Resize(nextSize);
            
            for(int i = originalSize; i < arr.Length; i++) Free(i);
        }
        
        
        void Free(int cur)
        {
            if(cur == -1) return;
            
            if(inUse[cur])
            {
                var p = prev[cur];
                var n = next[cur];
                if(n != -1) prev[n] = p;
                if(p != -1) next[p] = n;
                if(head == cur) head = n;
                Count -= 1;
            }
            
            var ori = freeHead;
            next[cur] = ori;
            if(ori != -1) prev[ori] = cur;
            prev[cur] = -1;
            freeHead = cur;
            
            inUse[cur] = false;
            unchecked { version[cur] += 1; }
        }
        
        SerializableLinkedListKey Use()
        {
            var cur = freeHead;
            freeHead = next[cur];
            if(freeHead != -1) prev[freeHead] = -1;
            
            next[cur] = head;
            prev[cur] = -1;
            if(head != -1) prev[head] = cur;
            head = cur;
            
            inUse[cur] = true;
            Count += 1;
            unchecked { version[cur] += 1; }
            return new SerializableLinkedListKey(cur, version[cur], this);
        }
        
        
        public struct IndexEnumerator : IEnumerator<SerializableLinkedListKey>
        {
            public int index;
            public SerializableLinkedList<T> list;
            public SerializableLinkedListKey Current => new SerializableLinkedListKey(index, list.version[index], list);
            object IEnumerator.Current => index;

            public void Dispose() => index = -1;

            public bool MoveNext()
            {
                if(index == -1) index = list.head;
                else index = list.next[index];
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
                yield return arr[index.id];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Valid(SerializableLinkedListKey key)
        {
            if(key.list != this) return false;
            if(key.id >= capacity) return false;
            if(key.version != version[key.id]) return false;
            return true;
        }
        
        
        
        public static void UnitTest(Action<string> log)
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
        }
    }

}