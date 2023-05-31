using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;

namespace Prota
{
    // 一个缓存过的 string => string dictionary.
    public class StringCache
    {
        public readonly Func<string, string> convert;
        
        readonly Dictionary<string, string> cache = new Dictionary<string, string>();
        
        
        public StringCache(Func<string, string> convert)
        {
            this.convert = convert;
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
