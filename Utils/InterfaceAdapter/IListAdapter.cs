using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{
    // Adapter: 从 Class<T> 到 Class<object>
    
    // IList<T> => IList<object>
    public class IListAdapter : IList<object>
    {
        object d;
        Type type;
        ProtaReflection pref;
        
        public IListAdapter(object x)
        {
            d = x;
            type = d.GetType();
            pref = new ProtaReflection(d);
        }
        
        public object this[int index]
        {
            get => pref.Call("get_Item", index);
            set => pref.Call("set_Item", index, value);
        }
        public int Count => (int)pref.Call("get_Count");
        public bool IsReadOnly => (bool)pref.Call("get_IsReadOnly");
        public void Add(object item) => pref.Call("Add", item);
        public void Clear() => pref.Call("Clear");
        public bool Contains(object item) => (bool)pref.Call("Contains", item);
        public void CopyTo(object[] array, int arrayIndex) => pref.Call("CopyTo", array, arrayIndex);
        public IEnumerator<object> GetEnumerator()
        {
            var iter = (IEnumerator)pref.Call("GetEnumerator");
            var t = new IEnumeratorAdapter(iter);
            while(t.MoveNext()) yield return t.Current;
        }

        public int IndexOf(object item) => (int)pref.Call("IndexOf", item);
        public void Insert(int index, object item) => pref.Call("Insert", index, item);
        public bool Remove(object item) => (bool)pref.Call("Remove", item);
        public void RemoveAt(int index) => pref.Call("RemoveAt", index);
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }



}