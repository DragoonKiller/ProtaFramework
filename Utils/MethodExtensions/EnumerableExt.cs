using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

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
    }
}