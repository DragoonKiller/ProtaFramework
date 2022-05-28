using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{

    // ICollection<T> => ICollection<object>
    public class ICollectionAdapter : ICollection<object>, ICollection
    {
        object d;
        Type type;
        ProtaReflection pref;
        
        public ICollectionAdapter(object x)
        {
            d = x;
            type = d.GetType();
            pref = new ProtaReflection(d);
        }

        public int Count => (int)pref.Call("get_Count");
        public bool IsReadOnly => (bool)pref.Call("get_IsReadOnly");
        public bool IsSynchronized => false;
        public object SyncRoot => false;
        public void Add(object item) => pref.Call("Add", item);
        public void Clear() => pref.Call("Clear");
        public bool Contains(object item) => (bool)pref.Call("Contains", item);
        public void CopyTo(object[] array, int arrayIndex) => pref.Call("CopyTo", array, arrayIndex);
        public void CopyTo(Array array, int index) => pref.Call("CopyTo", array, index);

        public IEnumerator<object> GetEnumerator() => new IEnumeratorAdapter(pref.Call("GetEnumerator"));
        public bool Remove(object item) => (bool)pref.Call("Remove", item);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

}