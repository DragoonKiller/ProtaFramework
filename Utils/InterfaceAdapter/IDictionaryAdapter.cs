using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{

    // IDictionary<K, V> => IDictionary<object, object>
    public class IDictionaryAdapter : IDictionary<object, object>, IDictionary
    {
        object d;
        Type type;
        ProtaReflection pref;
        
        public IDictionaryAdapter(object x)
        {
            d = x;
            type = d.GetType();
            pref = new ProtaReflection(d);
        }
        
        
        public object this[object key]
        {
            get => pref.Call("get_item", key);
            set => pref.Call("set_Item", key, value);
        }

        public ICollection<object> Keys => new ICollectionAdapter(pref.Call("get_Keys"));
        public ICollection<object> Values => new ICollectionAdapter(pref.Call("get_Values"));
        public int Count => (int)pref.Call("get_Count");
        public bool IsReadOnly => (bool)pref.Call("get_IsReadOnly");
        public bool IsFixedSize => false;
        public bool IsSynchronized => false;
        public object SyncRoot => false;
        ICollection IDictionary.Keys => (ICollectionAdapter)this.Keys;
        ICollection IDictionary.Values => (ICollectionAdapter)this.Values;
        public void Add(object key, object value) => pref.Call("Add", key, value);
        public void Add(KeyValuePair<object, object> item) => pref.Call("Add", item);
        public void Clear() => pref.Call("Clear");
        public bool Contains(KeyValuePair<object, object> item) => (bool)pref.Call("Contains", item);
        public bool Contains(object key) => (bool)pref.Call("Contains", key);
        public bool ContainsKey(object key) => (bool)pref.Call("ContainsKey", key);
        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex) => pref.Call("CopyTo", array, arrayIndex);
        public void CopyTo(Array array, int index) => pref.Call("CopyTo", array, index);

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            var iter = new IEnumeratorAdapter(pref.Call("GetEnumerator"));
            while(iter.MoveNext()) yield return (KeyValuePair<object, object>)iter.Current;
        }

        public bool Remove(object key) => (bool)pref.Call("Remove", key);
        public bool Remove(KeyValuePair<object, object> item) => (bool)pref.Call("Remove", item);
        public bool TryGetValue(object key, out object value)
        {
            var g = new object[2] { key, null };
            var res = (bool)pref.Call("TryGetValue", g);
            value = g[1];
            return res;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => throw new NotSupportedException();

        void IDictionary.Remove(object key) => this.Remove(key);
    }

    
}