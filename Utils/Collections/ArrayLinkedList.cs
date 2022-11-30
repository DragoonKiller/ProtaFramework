using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Collections;

namespace Prota
{
    public struct ArrayLinkedListKey : IEquatable<ArrayLinkedListKey>
    {
        public readonly int id;
        public readonly int version;
        public readonly IArrayLinkedList list;
        public bool valid => list?.Valid(this) ?? false;
        
        public ArrayLinkedListKey(int id, int version, IArrayLinkedList list)
        {
            this.id = id;
            this.version = version;
            this.list = list;
        }

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
        bool Valid(ArrayLinkedListKey key);
    }
    
    // 链表, 但是以数组的形式存储.
    // 通过 ArrayLinkedListKey 操作里面的数据.
    public class ArrayLinkedList<T> : IEnumerable<T>, IReadOnlyCollection<T>, IArrayLinkedList
        where T: struct
    {
        // 数据数组.
        public T[] arr { get; private set; } = null;
        
        // 下一个元素的下标. 没有填-1.
        public int[] next { get; private set; } = null;
        
        // 上一个元素的下标. 没有填-1.
        public int[] prev { get; private set; } = null;
        
        // 该元素是否正在使用.
        public bool[] inUse { get; private set; } = null;
        
        // 该元素的版本号.
        public int[] version { get; private set; } = null;
        
        // 使用中的元素数.
        public int Count { get; private set; } = 0;         // count in use.
        
        // 数组容量.
        public int capacity => arr?.Length ?? 0;
        
        // 数据链表头下标. 没有数据则是-1.
        public int head { get; private set; } = -1;
        
        // 没有数据的链表头下标. 数据填满了则是-1.
        public int freeHead { get; private set; } = -1;
        
        // 还有多少个没有使用的节点.
        public int freeCount => capacity - Count;
        
        // 取元素.
        public ref T this[ArrayLinkedListKey i] => ref arr[i.id];
        
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
            if(!inUse[i.id]) return false;
            if(version[i.id] != i.version) return false;
            Free(i.id);
            return true;
        }
        
        // 池子扩容.
        void Resize()
        {
            const int initialSize = 4;
            
            var originalSize = arr?.Length ?? 0;
            int nextSize = arr == null ? initialSize : (int)Math.Ceiling(arr.Length * 1.6);
            
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
        
        ArrayLinkedListKey Use()
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
            return new ArrayLinkedListKey(cur, version[cur], this);
        }
        
        
        public struct IndexEnumerator : IEnumerator<ArrayLinkedListKey>
        {
            public int index;
            public ArrayLinkedList<T> list;
            public ArrayLinkedListKey Current => new ArrayLinkedListKey(index, list.version[index], list);
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
                yield return arr[index.id];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Valid(ArrayLinkedListKey key)
        {
            if(key.list != this) return false;
            if(key.id >= capacity) return false;
            if(key.version < version[key.id]) return false;
            return true;
        }
    }
    
    public static class ArrayLinkedListUnitTest
    {
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