using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{
    
    // IEnumerator<T> => IEnumerator<object>
    public class IEnumeratorAdapter : IEnumerator<object>, IEnumerator
    {
        object d;
        Type type;
        ProtaReflection pref;
        public IEnumeratorAdapter(object x)
        {
            d = x;
            type = d.GetType();
            pref = new ProtaReflection(d);
        }
        public object Current => pref.Call("get_Current");
        public void Dispose() { }
        public bool MoveNext() => (bool)pref.Call("MoveNext");
        public void Reset() => pref.Call("Reset");
    }

}