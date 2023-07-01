using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;

namespace Prota
{
    // 泛型List, 但是可以获取底层数组, 可以设置容量.
    public class FlatList<T> : IList<T>
    {
        public T[] data { get; private set; }
        
        public T this[int index]
        {
            get => data[index];
            set => data[index] = value;
        }
        
        public int Count { get; private set; }

        public bool IsReadOnly => false;
        
        public bool IsValidData(T[] data) => this.data == data;
        
        public void Add(T item)
        {
            Preserve(Count + 1);
            data[Count++] = item;
        }
        
        public void Add(params T[] items)
        {
            Preserve(Count + items.Length);
            for(int i = 0; i < items.Length; i++) data[Count++] = items[i];
        }

        public void Clear()
        {
            data = null;
            Count = 0;
        }

        public bool Contains(T item)
        {
            foreach(var i in data)
                if(EqualityComparer<T>.Default.Equals(item, i))
                    return true;
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for(int i = 0; i < Count; i++)
            {
                if(i + arrayIndex >= array.Length) return;
                array[arrayIndex + i] = data[i];
            }
        }

        public void Preserve(int n)
        {
            if(data.Length > n) return;
            var size = checked(data.Length * 3 / 2);
            while(size < n) size = checked(size * 3 / 2);
            var newData = new T[size];
            this.CopyTo(newData, 0);
            data = newData;
        }
        
        public void Shrink()
        {
            if(Count * 1.5f > data.Length) return;
            Compact();
        }
        
        public void Resize(int n)
        {
            if(n == Count) return;
            
            if(n > Count)
            {
                Preserve(n);
                Count = n;
                return;
            }
            
            Count = n;
            Shrink();
        }
        
        public void Compact()
        {
            if(Count == data.Length) return;
            var newData = new T[Count];
            this.CopyTo(newData, 0);
            data = newData;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < Count; i++) yield return data[i];
        }

        public int IndexOf(T item)
        {
            for(int i = 0; i < Count; i++)
                if(EqualityComparer<T>.Default.Equals(item, data[i]))
                    return i;
            return -1;
        }

        public void Insert(int index, T item)
        {
            Preserve(Count + 1);
            for(int i = Count; i > index; i--) data[i] = data[i - 1];
            data[index] = item; 
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if(index < 0) return false;
            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            for(int i = 0; i < Count - 1; i++) data[i] = data[i + 1];
            Count--;
            Shrink();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
    
    
}
