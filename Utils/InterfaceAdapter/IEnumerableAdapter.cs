using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{
    

    // IEnumerable<T> => IEnumerable<object>
    public class IEnumerableAdapter : IEnumerable<object>, IEnumerable
    {
        object d;
        public IEnumerableAdapter(object x) => d = x;
        public IEnumerator<object> GetEnumerator()
        {
            return new IEnumeratorAdapter(d);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

}