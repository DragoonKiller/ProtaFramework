using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

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
    }
}