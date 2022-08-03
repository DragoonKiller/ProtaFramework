using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Collections;

namespace Prota
{
    public class ArrayLinkedList<T> : IEnumerable<T>, IReadOnlyCollection<T>
        where T: struct
    {
        public T[] arr { get; private set; } = null;
        public int[] next { get; private set; } = null;
        public int[] prev { get; private set; } = null;
        public bool[] inUse { get; private set; } = null;
        public int Count { get; private set; } = 0;         // count in use.
        public int capacity => arr?.Length ?? 0;
        public int head { get; private set; } = -1;
        public int freeHead { get; private set; } = -1;
        public int freeCount => capacity - Count;


        public ref T this[int i] => ref arr[i];
        
        public int Take()
        {
            if(freeCount == 0) Resize();
            System.Diagnostics.Debug.Assert(freeCount > 0);
            return Use();
        }
        
        public void Release(int i)
        {
            if(!inUse[i]) return;
            Free(i);
        }
        
        void Resize()
        {
            const int initialSize = 4;
            
            var originalSize = arr?.Length ?? 0;
            int nextSize = arr == null ? initialSize : (int)Math.Ceiling(arr.Length * 1.6);
            
            arr = arr.Resize(nextSize);
            next = next.Resize(nextSize);
            prev = prev.Resize(nextSize);
            inUse = inUse.Resize(nextSize);
            
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
        }
        
        int Use()
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
            return cur;
        }
        
        
        public struct IndexEnumerator : IEnumerator<int>
        {
            public int index;
            public ArrayLinkedList<T> list;
            public int Current => index;
            object IEnumerator.Current => index;

            public void Dispose() => index = -1;

            public bool MoveNext()
            {
                if(list.capacity == 0) return false;
                if(list.head == -1) return false;
                if(index == -1) index = list.head;
                else index = list.next[index];
                return true;
            }

            public void Reset() => index = -1;
        }

        public struct IndexEnumerable : IEnumerable<int>
        {
            public ArrayLinkedList<T> list;
            
            public IEnumerator<int> GetEnumerator() => new IndexEnumerator() { index = -1, list = list };

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        public IndexEnumerable EnumerateIndex() => new IndexEnumerable() { list = this };

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var index in EnumerateIndex())
            {
                yield return arr[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        
        
        
        
        
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
            
            
        }
    }
}