using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;

namespace Prota
{
    // 小根堆.
    public class ArrayHeap<T> : IEnumerable<T> where T: IComparable<T>
    {
        public struct Node
        {
            readonly int i;
            public int index => i - 1;
            public Node l => new Node(i * 2);
            public Node r => new Node(i * 2 + 1);
            public Node f => new Node(i / 2);
            public bool isNull => i == 0;
            public bool isRoot => i == 1;
            public Node(int index) => this.i = index + 1;
            public static Node root => new Node(0);
            public static Node none => new Node(-1);
        }
        
        readonly List<T> val = new List<T>();
        public T this[int key] => val[key];
        public T this[Node key] => val[key.index - 1];
        
        
        public ArrayHeap<T> Push(T v)
        {
            val.Add(v);
            var x = new Node(val.Count - 1);
            for(int i = 0; i < 10000000; i++)
            {
                var p = x.f;
                if(p.isNull) break;
                if(val[p.index].CompareTo(val[x.index]) >= 0) break;
                val.Swap(p.index, x.index);
                x = p;
            }
            return this;
        }
        
        public bool TryPop(out T v)
        {
            v = default;
            if(val.Count == 0) return false;
            v = Pop();
            return true;
        }
        
        public T Pop()
        {
            val.Swap(0, val.Count - 1);
            var res = val.Pop();
            var x = Node.root;
            for(int i = 0; i < 10000000; i++)
            {
                var largerThanLeft = x.l.index < val.Count && val[x.index].CompareTo(val[x.l.index]) > 0;
                var largerThanRight = x.r.index < val.Count && val[x.index].CompareTo(val[x.r.index]) > 0;
                if(largerThanLeft && largerThanRight)
                {
                    if(val[x.l.index].CompareTo(val[x.r.index]) < 0)        // 左边的小, 降左边.
                    {
                        val.Swap(x.l.index, x.index);
                        x = x.l;
                    }
                    else        // 右边的小, 降右边.
                    {
                        val.Swap(x.r.index, x.index);
                        x = x.r;
                    }
                }
                else if(largerThanLeft)     // 下降到左边.
                {
                    val.Swap(x.l.index, x.index);
                    x = x.l;
                }
                else if(largerThanRight)        // 下降到右边.
                {
                    val.Swap(x.r.index, x.index);
                    x = x.r;
                }
                else        // 两边都不需要下降.
                {
                    break;
                }
            }
            return res;
        }

        public IEnumerator<T> GetEnumerator() => val.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => val.GetEnumerator();
    }
}