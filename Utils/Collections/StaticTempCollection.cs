using System;
using System.Collections.Generic;

namespace Prota
{
    public class TempList<T>
    {    
        public struct Handle : IDisposable
        {
            public readonly List<T> value;

            public Handle(List<T> value) => this.value = value;
            
            public void Dispose() => TempList<T>.Return(value);
        }
        
        static HashSet<List<T>> unused = new HashSet<List<T>>();
        static HashSet<List<T>> inuse = new HashSet<List<T>>();
        public static Handle Get()
        {
            if(unused.Count == 0) unused.Add(new List<T>());
            var res = unused.FirstElement();
            unused.Remove(res);
            inuse.Add(res);
            if(unused.Count > inuse.Count + 1) unused.Remove(unused.FirstElement());
            return new Handle(res);
        }
        
        public static void Return(List<T> value)
        {
            if(!inuse.Contains(value)) return;
            value.Clear();
            unused.Remove(value);
            unused.Add(value);
        }
    }
    
    public class TempHashSet<T>
    {    
        public struct Handle : IDisposable
        {
            public readonly HashSet<T> value;

            public Handle(HashSet<T> value) => this.value = value;

            public void Dispose() => TempHashSet<T>.Return(value);
        }
    
        static HashSet<HashSet<T>> unused = new HashSet<HashSet<T>>();
        static HashSet<HashSet<T>> inuse = new HashSet<HashSet<T>>();
        public static Handle Get()
        {
            if(unused.Count == 0) unused.Add(new HashSet<T>());
            var res = unused.FirstElement();
            unused.Remove(res);
            inuse.Add(res);
            if(unused.Count > inuse.Count + 1) unused.Remove(unused.FirstElement());
            return new Handle(res);
        }
        
        public static void Return(HashSet<T> value)
        {
            if(!inuse.Contains(value)) return;
            value.Clear();
            unused.Remove(value);
            unused.Add(value);
        }
    }
    
    public static class TempDict<K, V>
    {
        public struct Handle : IDisposable
        {
            public readonly Dictionary<K, V> value;

            public Handle(Dictionary<K, V> value) => this.value = value;

            public void Dispose() => TempDict<K, V>.Return(value);
        }
    
        static HashSet<Dictionary<K, V>> unused = new HashSet<Dictionary<K, V>>();
        static HashSet<Dictionary<K, V>> inuse = new HashSet<Dictionary<K, V>>();
        public static Handle Get()
        {
            if(unused.Count == 0) unused.Add(new Dictionary<K, V>());
            var res = unused.FirstElement();
            unused.Remove(res);
            inuse.Add(res);
            if(unused.Count > inuse.Count + 1) unused.Remove(unused.FirstElement());
            return new Handle(res);
        }
        
        public static void Return(Dictionary<K, V> value)
        {
            if(!inuse.Contains(value)) return;
            value.Clear();
            unused.Remove(value);
            unused.Add(value);
        }
    }
}