using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public struct ListElementReference<T>
        {
            public ListElementReference(IList<T> list, int index)
            {
                this.list = list;
                this.index = index;
            }

            public readonly IList<T> list;
            public readonly int index;
            
            public bool exist => index >= 0 && index < list.Count;
            
            public T value
            {
                get => list[index];
                set => list[index] = value;
            }
            
            public void Remove() => list.RemoveAt(index);
            
        }
        
        public static ListElementReference<T> ElementReference<T>(this IList<T> list, int index)
            => new ListElementReference<T>(list, index);
        
        // w * h array, with lines first and column second.
        // point (a, b) will point to a * w + b.
        public struct ListView2D<T>
        {
            public readonly IList<T> list;
            public readonly int w, h;

            public ListView2D(IList<T> list, int w, int h)
            {
                this.list = list;
                this.w = w;
                this.h = h;
            }

            public T this[int l, int c]
            {
                get => list[l * h + c];
                set => list[l * h + c] = value;
            }
        }
        
        public static ListView2D<T> View2D<T>(this IList<T> list, int w, int h)
            => new ListView2D<T>(list, w, h);
        
        public static bool IsNullOrEmpty<T>(this List<T> list) => list == null || list.Count == 0;
        
        public static List<T> EnsureSize<T>(this List<T> list, int n)
        {
            if(list.Count < n) list.Resize(n);
            return list;
        }
        
        public static List<T> Resize<T>(this List<T> list, int n)
        {
            while(list.Count < n) list.Add(default);
            while(list.Count > n) list.Pop();
            return list;
        }
        
        public static List<T> Clone<T>(this List<T> x)
        {
            return new List<T>(x);
        }
        
        public static void InvokeAll<G>(this IEnumerable<G> list, params object[] args)
            where G: Delegate
        {
            foreach(var i in list) i?.DynamicInvoke(args);
        }
        
        public static void GetAndModify<T>(this List<T> a, int i, Func<T, T> f)
        {
            a[i] = f(a[i]);
        }
        
        public static List<T> Sorted<T>(this List<T> l, IComparer<T> c)
        {
            l.Sort(c);
            return l;
        }
        
        public static List<T> Sorted<T>(this List<T> l, Comparison<T> c)
        {
            l.Sort(c);
            return l;
        }
        
        public static T Last<T>(this List<T> l) => l[l.Count - 1];
        
        public static T LastOrDefault<T>(this List<T> l) => l.Count == 0 ? default : l.Last();
        
        public static T SetLast<T>(this List<T> l, T v) => l[l.Count - 1] = v;
        
        public static T Pop<T>(this List<T> l)
        {
            var res = l.Last();
            l.RemoveLast();
            return res;
        }
        
        public static bool TryGetValue<T>(this List<T> l, int i, out T x)
        {
            if(0 <= i && i < l.Count)
            {
                x = l[i];
                return true;
            }
            x = default;
            return false;
        }
        
        public static void Swap<T>(this IList<T> x, int i1, int i2)
        {
            var a1 = x[i1];
            x[i1] = x[i2];
            x[i2] = a1;
        }
        
        
        public static List<T> RemoveBySwap<T>(this List<T> l, int i)
        {
            var temp = l[l.Count - 1];
            l[l.Count - 1] = l[i];
            l[i] = temp;
            l.RemoveAt(l.Count - 1);
            return l;
        }
        
        public static IList<T> RemoveLast<T>(this IList<T> l)
        {
            l.RemoveAt(l.Count - 1);
            return l;
        }
        
        // 数量映射.
        public static F SyncData<T, F>(this F l, int n, Func<int, T> onCreate, Action<int, T> onEnable, Action<int, T> onDisable)
            where F: List<T>
        {
            for(int i = 0; i < n; i++)
            {
                if(i >= l.Count)
                {
                    l.Add(onCreate(i));
                }
                
                onEnable(i, l[i]);
            }
            
            for(int i = n; i < l.Count; i++) onDisable(i, l[i]);
            return l;
        }
        
        // 列表映射. 同步目标是 IEnumerable<K> data, 同步者是 List<T> l.
        public static F SetEnumList<F, G, T, K>(this F l, G data, Func<int, K, T> onCreate, Action<int, T, K> onEnable, Action<int, T> onDisable)
            where F: List<T>
            where G: IEnumerable<K>
        {
            var count = data.Count();
            int i = 0;
            foreach(var e in data)
            {
                if(i >= l.Count)
                {
                    l.Add(onCreate(i, e));
                }
                
                onEnable(i, l[i], e);
                i++;
            }
            
            for(i = count; i < l.Count; i++) onDisable(i, l[i]);
            return l;
        }
        
        public static List<T> Fill<T>(this List<T> list, int n)
        {
            for(int i = 0; i < n; i++) list.Add(default);
            return list;
        }
        
        public static List<T> Fill<T>(this List<T> list, int n, Func<int, T> content)
        {
            for(int i = 0; i < n; i++) list.Add(content(i));
            return list;
        }
        
        public static List<T> Shuffle<T>(this List<T> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                var a = rand.Next(0, list.Count);
                var t = list[a];
                list[a] = list[i];
                list[i] = t;
            }
            return list;
        }
        
        public static List<T> Shrink<T>(this List<T> list, int n)
        {
            while(list.Count > n && list.Count != 0)
            {
                list.Pop();
            }
            return list;
        }
        
        public static List<T> AddNoDuplicate<T>(this List<T> list, T x)
        {
            if(!list.Contains(x)) list.Add(x);
            return list;
        }
        
        public static T[] UnderlayingArray<T>(this List<T> list)
        {
            var t = list.ProtaReflection();
            return t.Get<T[]>("_items");
        }
    }
}
