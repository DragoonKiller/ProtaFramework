
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prota
{
    public class DataTable : CsvParser
    {
        public static Dictionary<string, DataTable> cache = new Dictionary<string, DataTable>();
        
        public static void InvalidateCache()
        {
            cache.Clear();
        }
        
        public static DataTable Get(string name)
        {
            if(cache.TryGetValue(name, out DataTable value)) return value;
            if(onFind != null)
            {
                value = onFind(name);
                if(value != null)
                {
                    cache[name] = value;
                    return value;
                }
            }
            throw new Exception($"DataTable [{ name }] not found.");
        }
        
        public static Func<string, DataTable> onFind;
        
        // ====================================================================================================
        // ====================================================================================================
        
        readonly PairDictionary<string, int> _keys = new PairDictionary<string, int>();
        
        readonly PairDictionary<string, int> _entries = new PairDictionary<string, int>();
        
        public IEnumerable<string> keys => _keys.Keys;
        
        public IEnumerable<string> entries
        {
            get
            {
                if(_entries == null) throw new Exception($"DataTable [{ name }] has no key, so there's no entry.");
                return _entries.Keys;
            }
        }
        
        public DataTable(string originalContent) : this("Unknown", originalContent)
        {
            
        }
        
        public DataTable(string name, string originalContent) : base(name, originalContent)
        {
            for(int i = 0; i < columnN; i++)
            {
                bool success = Get(0, i, out string key);
                if(!success || key.NullOrEmpty()) continue;
                _keys[key] = i;
            }
            
            var hasKey = _keys.TryGetValue("id", out int keyIndex);
            if(hasKey)
            {
                _entries = new PairDictionary<string, int>();
                for(int i = 1; i < rowN; i++)
                {
                    Get(i, keyIndex, out string key);
                    if(key.NullOrEmpty()) continue;
                    _entries[key] = i;
                }
            }
            
            foreach(var key in keys)
            {
                if(key.Trim() != key) throw new Exception($"DataTable [{ name }] Key [{ key }] has space or invisible chars.");
            }
        }
        
        
        public bool Get(int line, string key, out string value)
        {
            if(!_keys.TryGetValue(key, out int index)) throw new Exception($"DataTable [{ name }] Key { key } not found.");
            return Get(line, index, out value);
        }
        
        public bool Get(string entry, string key, out string value)
        {
            if(_entries == null) throw new Exception($"DataTable [{ name }] has no key, so there's no entry.");
            if(!_keys.TryGetValue(key, out int index))
            {
                value = null;
                return false;
            }
            if(!_entries.TryGetValue(entry, out int line))
            {
                value = null;
                return false;
            }
            return Get(line, index, out value);
        }
        
        public bool Get(int line, string key, out float value)
        {
            if(!_keys.TryGetValue(key, out int index)) throw new Exception($"DataTable [{ name }] Key { key } not found.");
            return Get(line, index, out value);
        }
        
        public bool Get(string entry, string key, out float value)
        {
            if(_entries == null) throw new Exception($"DataTable [{ name }] has no key, so there's no entry.");
            if(!_keys.TryGetValue(key, out int index))
            {
                value = 0;
                return false;
            }
            if(!_entries.TryGetValue(entry, out int line))
            {
                value = 0;
                return false;
            }
            return Get(line, index, out value);
        }
        
        public bool Get(int line, string key, out int value)
        {
            if(!_keys.TryGetValue(key, out int index)) throw new Exception($"DataTable [{ name }] Key { key } not found.");
            return Get(line, index, out value);
        }
        
        public bool Get(string entry, string key, out int value)
        {
            if(_entries == null) throw new Exception($"DataTable [{ name }] has no key, so there's no entry.");
            if(!_keys.TryGetValue(key, out int index))
            {
                value = 0;
                return false;
            }
            if(!_entries.TryGetValue(entry, out int line))
            {
                value = 0;
                return false;
            }
            return Get(line, index, out value);
        }
        
        public bool HasKey(string name) => _keys.ContainsKey(name);
        public bool HasEntry(string name) => _entries.ContainsKey(name);
        public int IndexOfEntry(string name) => _entries[name];
        public int IndexOfKey(string name) => _keys[name];
        public string EntryOfIndex(int index) => _entries.GetKeyByValue(index);
        public string KeyOfIndex(int index) => _keys.GetKeyByValue(index);
        
    }
}
