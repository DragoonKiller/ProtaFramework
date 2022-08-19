using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static void InvokeAll<G>(this IEnumerable<G> list, object[] args = null)
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
        
        public static T Last<T>(this List<T> l, T v) => l[l.Count - 1] = v;
        
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
        
        public static F SetLength<T, F>(this F l, int n, Func<int, T> onCreate, Action<int, T> onEnable, Action<int, T> onDisable)
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
        
        // map K => T with i.
        public static F SetEnumList< F, G, T, K>(this F l, G data, Func<int, K, T> onCreate, Action<int, T, K> onEnable, Action<int, T> onDisable)
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
        
        public static List<T> Fill<T>(this List<T> list, int n, Func<int, T> content)
        {
            for(int i = 0; i < n; i++)
            {
                list.Add(content(i));
            }
            return list;
        }
        
    }
}