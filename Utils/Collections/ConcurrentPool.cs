using System;
using System.Collections.Generic;

using System.Collections.Concurrent;

namespace Prota
{
    
    
    public class ConcurrentPool<T> where T: class, new()
    {
        public struct Handle : IDisposable
        {
            public readonly ConcurrentPool<T> src;
            
            public readonly T value;
            
            public readonly int version;
            
            public Handle(ConcurrentPool<T> src, T value, int version)
            {
                this.src = src;
                this.value = value;
                this.version = version;
            }

            public void Dispose()
            {
                src.Return(value, version);
            }
        }
        
        
        static ConcurrentPool<T> _instance;
        public static ConcurrentPool<T> instance
        {
            get
            {
                if(_instance == null) _instance = new ConcurrentPool<T>();
                return _instance;
            }
        }
        
        object lockobj = new Object();
        
        HashSet<T> unused = new HashSet<T>();
        Dictionary<T, int> inuse = new Dictionary<T, int>();
        
        public Action<T> onGet;
        
        public Action<T> onReturn;
        
        public int version = 0;
        
        ConcurrentPool() { }
        
        public Handle Get()
        {
            lock(lockobj)
            {
                if(unused.Count == 0) unused.Add(new T());
                var res = unused.FirstElement();
                unused.Remove(res);
                var v = ++version;
                inuse.Add(res, v);
                if(unused.Count > inuse.Count + 1) unused.Remove(unused.FirstElement());
                onGet?.Invoke(res);
                return new Handle(this, res, v);
            }
        }
        
        void Return(T value, int specVersion)
        {
            lock(lockobj)
            {
                if(!inuse.TryGetValue(value, out var version)) return;
                if(version != specVersion) return;
                onReturn?.Invoke(value);
                unused.Remove(value);
                unused.Add(value);
            }
        }
        
        
    }
}