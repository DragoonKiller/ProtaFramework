using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Concurrent;

namespace Prota
{
    public class PairDictionary<A, B>
        : IEnumerable
        , IDictionary<A, B>
        , IReadOnlyDictionary<A, B>
        , IDictionary
        , IDeserializationCallback
        , ISerializable
    {
        Dictionary<A, B> dict = new Dictionary<A, B>();
        Dictionary<B, A> rev = new Dictionary<B, A>();
        
        
        
        public A GetKeyByValue(B value)
        {
            return rev[value];
        }
        
        public bool TryGetKeyByValue(B value, out A key)
        {
            return rev.TryGetValue(value, out key);
        }
        
        
        public B this[A key]
        {
            get => dict[key];
            set
            {
                dict[key] = value;
                rev[value] = key;
            }
        }
        
        public object this[object key]
        {
            get => (dict as IDictionary)[key];
            set
            {
                (dict as IDictionary)[key] = value;
                (rev as IDictionary)[value] = key;
            }
        }

        public ICollection<A> Keys => dict.Keys;

        public ICollection<B> Values => dict.Values;

        public int Count => dict.Count;

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        IEnumerable<A> IReadOnlyDictionary<A, B>.Keys => dict.Keys;

        ICollection IDictionary.Keys => dict.Keys;

        IEnumerable<B> IReadOnlyDictionary<A, B>.Values => dict.Values;

        ICollection IDictionary.Values => dict.Values;

        public void Add(A key, B value)
        {
            dict.Add(key, value);
            rev.Add(value, key);
        }

        public void Add(KeyValuePair<A, B> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(object key, object value)
        {
            (this as IDictionary).Add(key, value);
        }

        public void Clear()
        {
            dict.Clear();
            rev.Clear();
        }

        public bool Contains(KeyValuePair<A, B> item)
        {
            return (dict as IDictionary).Contains(item);
        }

        public bool Contains(object key)
        {
            return (dict as IDictionary).Contains(key);
        }

        public bool ContainsKey(A key)
        {
            return dict.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<A, B>[] array, int arrayIndex)
        {
            (dict as ICollection<KeyValuePair<A, B>>).CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            (dict as ICollection).CopyTo(array, index);
        }

        public IEnumerator<KeyValuePair<A, B>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            dict.GetObjectData(info, context);
        }

        public void OnDeserialization(object sender)
        {
            dict.OnDeserialization(sender);
        }

        public bool Remove(A key)
        {
            if(!dict.TryGetValue(key, out var value)) return false;
            dict.Remove(key);
            rev.Remove(value);
            return true;
        }

        public bool Remove(KeyValuePair<A, B> item)
        {
            return this.Remove(item.Key);
        }

        public void Remove(object key)
        {
            if(!(key is A keyA)) return;
            this.Remove(keyA);
        }

        public bool TryGetValue(A key, out B value)
        {
            return dict.TryGetValue(key, out value);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (dict as System.Collections.IEnumerable).GetEnumerator();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => (dict as IDictionary).GetEnumerator();
    }


}
