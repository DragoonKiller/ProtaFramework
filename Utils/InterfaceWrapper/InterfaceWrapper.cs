
using System.Collections;
using System.Collections.Generic;
using System;

namespace Prota
{
    // Wrapper: 从 Class 到 Class<object>
    
    // IEnumerator => IEnumerator<object>
    public class IEnumeratorWrapper : IEnumerator<object>, IEnumerator
    {
        public IEnumerator d;
        public object Current => d.Current;
        public void Dispose() => d = null;
        public bool MoveNext() => d.MoveNext();
        public void Reset() => d.Reset();
    }
    
    
    // IEnumerable => IEnumerable<object>
    
    
    // IList => IList<object>
    public class IListWrapper : IList<object>, IList
    {
        public IList d;

        public object this[int index] { get => d[index]; set => d[index] = value; }
        public int Count => d.Count;
        public bool IsReadOnly => d.IsReadOnly;
        public bool IsFixedSize => d.IsFixedSize;
        public bool IsSynchronized => d.IsSynchronized;
        public object SyncRoot => d.SyncRoot;
        public void Add(object item) => d.Add(item);
        public void Clear() => d.Clear();
        public bool Contains(object item) => d.Contains(item);
        public void CopyTo(object[] array, int arrayIndex) => d.CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => d.CopyTo(array, index);

        public IEnumerator<object> GetEnumerator()
        {
            foreach(var i in d) yield return i;
        }
        public int IndexOf(object item) => d.IndexOf(item);
        public void Insert(int index, object item) => d.Insert(index, item);
        public bool Remove(object item)
        {
            int index = d.IndexOf(item);
            if(index < 0) return false;
            d.Remove(item);
            return true;
        }
        public void RemoveAt(int index) => d.RemoveAt(index);
        int IList.Add(object value)
        {
            this.Add(value);
            return this.Count - 1;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        void IList.Remove(object value) => this.Remove(value);
    }
    
    
    
    
    
    // ICollection => ICollection<object>
    public class ICollectionWrapper : ICollection<object>, ICollection
    {
        public ICollection d;
        public int Count => d.Count;
        public bool IsSynchronized => d.IsSynchronized;
        public object SyncRoot => d.SyncRoot;
        int ICollection<object>.Count => d.Count;
        bool ICollection<object>.IsReadOnly => false;
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        void ICollection<object>.Add(object item) => throw new NotSupportedException();
        void ICollection<object>.Clear() => throw new NotSupportedException();
        bool ICollection<object>.Contains(object item)
        {
            foreach(var i in d) if(i == item) return true;
            return false;
        } 
        void ICollection<object>.CopyTo(object[] array, int arrayIndex) => d.CopyTo(array, arrayIndex);
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach(var i in d) yield return i;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach(var i in d) yield return i;
        }
        bool ICollection<object>.Remove(object item) => throw new NotSupportedException();
    }
    
    
    
    
    
    // IDictionary => IDictionary<object, object>
    public class DictionaryWrapper : IDictionary<object, object>, IDictionary
    {
        public IDictionary d;
        public object this[object key] { get => d[key]; set => d[key] = value; }
        public ICollection<object> Keys => new ICollectionWrapper(){ d = d.Keys };
        public ICollection<object> Values => new ICollectionWrapper(){ d = d.Values };
        public int Count => d.Count;
        public bool IsReadOnly => d.IsReadOnly;
        public bool IsFixedSize => d.IsFixedSize;
        public bool IsSynchronized => d.IsSynchronized;
        public object SyncRoot => d.SyncRoot;
        ICollection IDictionary.Keys => d.Keys;
        ICollection IDictionary.Values => d.Values;
        public void Add(object key, object value) => d.Add(key, value);
        public void Add(KeyValuePair<object, object> item) => d.Add(item.Key, item.Value);
        public void Clear() => d.Clear();
        public bool Contains(KeyValuePair<object, object> item) => d.Contains(item.Key);
        public bool Contains(object key) => this.ContainsKey(key);
        public bool ContainsKey(object key) => d.Contains(key);
        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex) => throw new NotSupportedException();
        public void CopyTo(Array array, int index) => d.CopyTo(array, index);

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            foreach(var key in d.Keys)
            {
                yield return new KeyValuePair<object, object>(key, d[key]);
            }
        }
        public bool Remove(object key)
        {
            if(d.Contains(key))
            {
                d.Remove(key);
                return true;
            }
            return false;
        }
        public bool Remove(KeyValuePair<object, object> item)
        {
            if(d.Contains(item.Key))
            {
                d.Remove(item.Key);
                return true;
            }
            return false;
        }
        public bool TryGetValue(object key, out object value)
        {
            if(d.Contains(key))
            {
                value = d[key];
                return true;
            }
            value = null;
            return false;
        } 
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        void IDictionary.Remove(object key) => this.Remove(key);
    }
    
    
    
    
    
}