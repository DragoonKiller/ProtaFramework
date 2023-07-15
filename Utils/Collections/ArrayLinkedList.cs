using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Collections;

namespace Prota
{
    public readonly struct ArrayLinkedListKey : IEquatable<ArrayLinkedListKey>
    {
        public readonly int id;
        public readonly int version;
        public readonly IArrayLinkedList list;
        
        public ArrayLinkedListKey(int id, int version, IArrayLinkedList list)
        {
            this.id = id;
            this.version = version;
            this.list = list;
        }
        
        public static ArrayLinkedListKey none => new ArrayLinkedListKey(0, 0, null);
        
        public bool isNone => list == null;
        
        public bool valid => list != null && list.ValidateKey(this);

        public bool Equals(ArrayLinkedListKey other) => this == other;
        
        public override bool Equals(object x) => x is ArrayLinkedListKey arrKey && arrKey == this;
        
        public override int GetHashCode()
        {
            return HashCode.Combine(id, version, list);
        }
        
        public static bool operator==(ArrayLinkedListKey a, ArrayLinkedListKey b)
        {
            return a.id == b.id && a.version == b.version && a.list == b.list;
        }
        
        public static bool operator!=(ArrayLinkedListKey a, ArrayLinkedListKey b)
        {
            return a.id != b.id || a.version != b.version || a.list != b.list;
        }

        public override string ToString() => $"key[{ id }:{ version }]";
    }
    
    public interface IArrayLinkedList
    {
        bool ValidateKey(ArrayLinkedListKey key);
    }
    
    // 链表, 但是以数组的形式存储.
    // 通过 ArrayLinkedListKey 操作里面的数据.
    public class ArrayLinkedList<T> : IEnumerable<T>, IReadOnlyCollection<T>, IArrayLinkedList
        // where T: struct
    {
        public struct InternalData
        {
            public T value;
            public int next;        // 下一个元素的下标. 没有填-1.
            public int prev;        // 上一个元素的下标. 没有填-1.
            public int version;     // 该元素的版本号.
            public bool inuse;      // 该元素是否正在使用.
        }
        
        // 数据数组.
        public InternalData[] data { get; private set; } = null;
        
        // 使用中的元素数.
        public int Count { get; private set; } = 0;         // count in use.
        
        // 数组容量.
        public int capacity => data?.Length ?? 0;
        
        public ArrayLinkedListKey head => new ArrayLinkedListKey(headIndex, data[headIndex].version, this);
        
        // 数据链表头下标. 没有数据则是-1.
        public int headIndex { get; private set; } = -1;
        
        // 没有数据的链表头下标. 数据填满了则是-1.
        public int freeHeadIndex { get; private set; } = -1;
        
        // 还有多少个没有使用的节点.
        public int freeCount => capacity - Count;
        
        // 取元素.
        public ref T this[ArrayLinkedListKey i] => ref data[i.id].value;
        
        // 在链表中新增一个元素s.
        public ArrayLinkedListKey Take()
        {
            if(freeCount == 0) Resize();
            System.Diagnostics.Debug.Assert(freeCount > 0);
            return Use();
        }
        
        // 释放一个链表中的元素.
        public bool Release(ArrayLinkedListKey i)
        {
            if(this != i.list) return false;
            if(i.id >= capacity) return false;
            if(!data[i.id].inuse) return false;
            if(data[i.id].version != i.version) return false;
            Free(i.id);
            return true;
        }
        
        public ArrayLinkedList<T> Clear()
        {
            data = null;
            Count = 0;
            headIndex = freeHeadIndex = -1;
            return this;
        }
        
        // 池子扩容.
        void Resize()
        {
            const int initialSize = 4;
            
            var originalSize = data?.Length ?? 0;
            int nextSize = data == null ? initialSize : (int)Math.Ceiling(originalSize * 1.6);
            
            data = data.Resize(nextSize);
            
            for(int i = originalSize; i < capacity; i++) Free(i);
        }
        
        
        void Free(int cur)
        {
            if(cur == -1) return;
            
            if(data[cur].inuse)
            {
                var p = data[cur].prev;
                var n = data[cur].next;
                if(n != -1) data[n].prev = p;
                if(p != -1) data[p].next = n;
                if(headIndex == cur) headIndex = n;
                Count -= 1;
            }
            
            var ori = freeHeadIndex;
            data[cur].next = ori;
            if(ori != -1) data[ori].prev = cur;
            data[cur].prev = -1;
            freeHeadIndex = cur;
            
            data[cur].inuse = false;
            unchecked { data[cur].version += 1; }
        }
        
        ArrayLinkedListKey Use()
        {
            var cur = freeHeadIndex;
            freeHeadIndex = data[cur].next;
            if(freeHeadIndex != -1) data[freeHeadIndex].prev = -1;
            
            data[cur].next = headIndex;
            data[cur].prev = -1;
            if(headIndex != -1) data[headIndex].prev = cur;
            headIndex = cur;
            
            data[cur].inuse = true;
            Count += 1;
            unchecked { data[cur].version += 1; }
            return new ArrayLinkedListKey(cur, data[cur].version, this);
        }
        
        
        public struct IndexEnumerator : IEnumerator<ArrayLinkedListKey>
        {
            public int index;
            public ArrayLinkedList<T> list;
            public ArrayLinkedListKey Current => new ArrayLinkedListKey(index, list.data[index].version, list);
            object IEnumerator.Current => index;

            public void Dispose() => index = -1;

            public bool MoveNext()
            {
                if(index == -1) index = list.headIndex;
                else index = list.data[index].next;
                if(index == -1) return false;
                return true;
            }

            public void Reset() => index = -1;
        }

        public struct IndexEnumerable : IEnumerable<ArrayLinkedListKey>
        {
            public ArrayLinkedList<T> list;
            
            public IEnumerator<ArrayLinkedListKey> GetEnumerator() => new IndexEnumerator() { index = -1, list = list };

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
        
        // 检查给定的 key 是不是这个列表的 key, 是不是当前有效的 key.
        public bool ValidateKey(ArrayLinkedListKey key)
        {
            if(key.list != this) return false;
            if(key.id >= capacity) return false;
            if(key.version != data[key.id].version) return false;
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
