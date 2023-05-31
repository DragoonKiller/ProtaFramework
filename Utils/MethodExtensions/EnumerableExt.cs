using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static T FirstElement<T>(this IEnumerable<T> e)
        {
            var s = e.GetEnumerator();
            if(!s.MoveNext()) throw new ArgumentException("collecetion doesn't have element.");
            return s.Current;
        }
        
        public static Task DoCoroutineAsync<T>(this IEnumerable<T> x, CancellationToken? _token)
            => DoCoroutineAsync(x.GetEnumerator(), _token);
        
        public static async Task DoCoroutineAsync<T>(this IEnumerator<T> x, CancellationToken? _token)
        {
            var token = _token ?? CancellationToken.None;
            Func<bool> run = () => x.MoveNext();
            while(await Task.Run(run, token));
        }
        
        public static string ToListString<T>(this IEnumerable<T> list, Func<T, string> f, string seperator = null)
            => string.Join(seperator ?? ",",  list.Select(x => $"[{f(x)}]"));
        
        public static bool SameContent<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            if((a == null) != (b == null)) return false;
            
            using var _ = TempHashSet<T>.Get(out var ha);
            using var __ = TempHashSet<T>.Get(out var hb);
            ha.AddRange(a);
            hb.AddRange(b);
            foreach(var i in ha)
            {
                if(!hb.Contains(i)) return false;
                hb.Remove(i);
            }
            return hb.Count == 0;
        }
        
        public static bool SameContentAndOrder(this IEnumerable a, IEnumerable b)
        {
            if((a == null) != (b == null)) return false;
            
            var ea = a.GetEnumerator();
            var eb = b.GetEnumerator();
            while(true)
            {
                var na = ea.MoveNext();
                var nb = eb.MoveNext();
                if(na != nb) return false;
                if(!na) return true;
                if(!ea.Current.Equals(eb.Current)) return false;
            }
        }
        
        public static bool SameContentAndCount<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            if((a == null) != (b == null)) return false;
            
            using var _ = TempDict<T, int>.Get(out var ha);
            using var __ = TempDict<T, int>.Get(out var hb);
            
            foreach(var i in a)
            {
                if(!ha.ContainsKey(i)) ha[i] = 0;
                ha[i]++;
            }
            
            foreach(var i in b)
            {
                if(!hb.ContainsKey(i)) hb[i] = 0;
                hb[i]++;
            }
            
            foreach(var i in ha)
            {
                if(!hb.ContainsKey(i.Key)) return false;
            }
            
            foreach(var i in hb)
            {
                if(!ha.ContainsKey(i.Key)) return false;
            }
            
            foreach(var i in ha)
            {
                if(hb[i.Key] != i.Value) return false;
            }
            
            return true;
        }
    }
}
