using System.Collections.Generic;
using System;
using System.Text;
using System.Buffers.Binary;
using System.Collections;
using System.Linq;

namespace Prota
{
    public class HashMapList<A, B> : Dictionary<A, List<B>>
    {
        public IEnumerable<B> elements => this.Values.SelectMany(x => x);
        
        public bool Contains(A key, B val)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            return s.Contains(val);
        }
        
        public HashMapList<A, B> SetElement(A key, int i, B val)
        {
            this.GetOrCreate(key, out var s);
            s[i] = val;
            return this;
        }
        
        public HashMapList<A, B> InsertElement(A key, int i, B val)
        {
            this.GetOrCreate(key, out var s);
            s.Insert(i, val);
            return this;
        }
        
        public HashMapList<A, B> AddElement(A key, B val)
        {
            this.GetOrCreate(key, out var s);
            s.Add(val);
            return this;
        }
        
        public bool RemoveElementAt(A key, int i)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            s.RemoveAt(i);
            if(s.Count == 0) this.Remove(key);
            return true;
        }
        
        public bool RemoveElement(A key, B val)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            var res = s.Remove(val);
            if(s.Count == 0) this.Remove(key);
            return res;
        }
        
        public bool InsertElementNoDuplicate(A key, int i, B val)
        {
            this.GetOrCreate(key, out var s);
            if(s.Contains(val)) return false;
            s.Insert(i, val);
            return true;
        }
        
        public bool AddElementNoDuplicate(A key, B val)
        {
            this.GetOrCreate(key, out var s);
            if(s.Contains(val)) return false;
            s.Add(val);
            return true;
        }
    }
    
    public class HashMapSet<A, B> : Dictionary<A, HashSet<B>>
    {
        
        public HashMapSet<A, B> AddElement(A key, B val)
        {
            this.GetOrCreate(key, out var s);
            s.Add(val);
            return this;
        }
        
        public bool RemoveElement(A key, B val)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            var res = s.Remove(val);
            if(s.Count == 0) this.Remove(key);
            return res;
        }
    }
    
    public class HashMapDict<A, B, C> : Dictionary<A, Dictionary<B, C>>
    {
        
        public HashMapDict<A, B, C> SetElement(A key, B key2, C val)
        {
            this.GetOrCreate(key, out var s);
            s[key2] = val;
            return this;
        }
        
        public HashMapDict<A, B, C> AddElement(A key, B key2, C val)
        {
            this.GetOrCreate(key, out var s);
            s.Add(key2, val);
            return this;
        }
        
        public bool RemoveElement(A key, B key2)
        {
            if(!this.TryGetValue(key, out var s)) return false;
            var res = s.Remove(key2);
            if(s.Count == 0) this.Remove(key);
            return res;
        }
    }
}
