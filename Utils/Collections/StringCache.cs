using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;

namespace Prota
{
    public class StructStringCache<T> where T: struct
    {
        public readonly Func<T, string> convert;
        
        readonly Dictionary<T, string> cache = new Dictionary<T, string>();
        
        public StructStringCache(Func<T, string> convert)
        {
            this.convert = convert;
        }
        
        public StructStringCache()
        {
            this.convert = x => x.ToString();
        }
        
        public string this[T x]
        {
            get
            {
                if(cache.TryGetValue(x, out var y)) return y;
                y = convert(x);
                cache[x] = y;
                return y;
            }
        }
    }
    
    
    
    // 一个缓存过的 string => string dictionary.
    public class StringCache
    {
        public readonly Func<string, string> convert;
        
        readonly Dictionary<string, string> cache = new Dictionary<string, string>();
        
        
        public StringCache(Func<string, string> convert)
        {
            this.convert = convert;
        }
        
        public StringCache()
        {
            this.convert = x => x;
        }
        
        public string this[string x]
        {
            get
            {
                if(cache.TryGetValue(x, out var y)) return y;
                y = convert(x);
                cache[x] = y;
                return y;
            }
        }
    }
}
