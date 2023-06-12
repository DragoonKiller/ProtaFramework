
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine.Pool;

namespace Prota
{
    public class DataTableAccessor<T> where T: new()
    {
        public readonly DataTable dataTable;
        
        public static readonly ProtaReflectionType p = typeof(T).ProtaReflection();
        
        public readonly Dictionary<string, T> nameDict = new Dictionary<string, T>();
        public readonly Dictionary<int, T> idDict = new Dictionary<int, T>();
        
        public DataTableAccessor(DataTable dataTable)
        {
            this.dataTable = dataTable;
        }
        
        public T this[string key]
        {
            get
            {
                if(nameDict.TryGetValue(key, out T value)) return value;
                if(!dataTable.hasKey) throw new Exception($"DataTable [{ dataTable.name }] has no key column.");
                var obj = new T();
                var id = dataTable.LineNumOfEntry(key);
                nameDict[key] = obj;
                idDict[id] = obj;
                GetObject(obj, id, dataTable);
                return obj;
            }
        }
        
        public T this[int lineNum]
        {
            get
            {
                if(idDict.TryGetValue(lineNum, out T value)) return value;
                var obj = new T();
                var name = dataTable.hasKey ? dataTable.GetString(lineNum, "id") : null;
                if(name != null) nameDict[name] = obj;
                idDict[lineNum] = obj;
                GetObject(obj, lineNum, dataTable);
                return obj;
            }
        }
        
        static void GetObject(T obj, int columnNum, DataTable dataTable)
        {
            foreach(var field in p.allPublicFields)
            {
                if(!dataTable.HasKey(field.Name)) continue;
                
                if(field.FieldType == typeof(float))
                {
                    field.SetValue(obj, dataTable.GetFloat(columnNum, field.Name));
                }
                else if(field.FieldType == typeof(int))
                {
                    field.SetValue(obj, dataTable.GetInt(columnNum, field.Name));
                }
                else if(field.FieldType == typeof(double))
                {
                    field.SetValue(obj, (double)dataTable.GetFloat(columnNum, field.Name));
                }
                else if(field.FieldType == typeof(string))
                {
                    field.SetValue(obj, dataTable.GetString(columnNum, field.Name));
                }
                else throw new Exception($"DataTable [{ dataTable.name }] field [{ field.Name }] type [{ field.FieldType }] not supported.");
            }
        }
        
        public void ClearCache()
        {
            nameDict.Clear();
            idDict.Clear();
        }
        
    }
    
    public class DataTable : CsvParser
    {
        public static Dictionary<string, DataTable> cache = new Dictionary<string, DataTable>();
        
        public static void InvalidateCache()
        {
            cache.Clear();
        }
        
        public static DataTable GetTable(string name)
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
        
        // 属性.
        readonly PairDictionary<string, int> _propertiesColumnNum = new PairDictionary<string, int>();
        
        // 每一行的名称, 由 id 列指定.
        // 注意 id 列必须不重名, 且不为空, 否则最好不要使用 id 列.
        // 保存的行数是考虑Header的数据表行数.
        readonly PairDictionary<string, int> _entriesLineNum = new PairDictionary<string, int>();
        
        public IEnumerable<string> properties => _propertiesColumnNum.Keys;
        
        // 有没有 id 这一列.
        public bool hasKey { get; private set; }
        
        // id 这一列是第几行.
        public int keyColumnNum
        {
            get
            {
                if(!hasKey) throw new Exception($"DataTable [{ name }] has no key.");
                return _propertiesColumnNum["id"];
            }
        }
        
        public DataTable(string originalContent, int headerLine) : this("Unknown", originalContent, headerLine)
        {
            
        }
        
        public DataTable(string name, string originalContent, int headerLine) : base(name, originalContent, headerLine)
        {
            if(headerLine == 0) throw new Exception($"DataTable [{ name }] must have header.");
            
            for(int i = 0; i < headerInfo.properties.Count; i++)
            {
                _propertiesColumnNum[headerInfo.properties[i]] = i;
            }
            
            hasKey = _propertiesColumnNum.TryGetValue("id", out int keyIndex);
            if(hasKey)
            {
                _entriesLineNum = new PairDictionary<string, int>();
                for(int i = 0; i < dataRowN; i++)
                {
                    var key = GetString(i, keyIndex);
                    _entriesLineNum[key] = i;
                }
            }
            
            foreach(var key in properties)
            {
                if(key.Trim() != key) throw new Exception($"DataTable [{ name }] Key [{ key }] has space or invisible chars.");
            }
        }
        
        // ====================================================================================================
        // Get
        // ====================================================================================================
        
        public float GetFloat(string entry, string propName)
        {
            var line = GetLineOfEntry(entry);
            var column = GetColumnFromPropName(propName);
            return GetFloat(line, column);
        }
        
        public int GetInt(string entry, string propName)
        {
            var line = GetLineOfEntry(entry);
            var column = GetColumnFromPropName(propName);
            return GetInt(line, column);
        }
        
        public string GetString(string entry, string propName)
        {
            var line = GetLineOfEntry(entry);
            var column = GetColumnFromPropName(propName);
            return GetString(line, column);
        }
        
        public float GetFloat(int dataLine, string propName)
        {
            var column = GetColumnFromPropName(propName);
            return GetFloat(dataLine, column);
        }
        
        public int GetInt(int dataLine, string propName)
        {
            var column = GetColumnFromPropName(propName);
            return GetInt(dataLine, column);
        }
        
        public string GetString(int dataLine, string propName)
        {
            var column = GetColumnFromPropName(propName);
            return GetString(dataLine, column);
        }
        
        
        int GetLineOfEntry(string entry)
        {
            if(_entriesLineNum == null) throw new Exception($"DataTable [{ name }] has no key, so there's no entry.");
            // _entriesLineNum.Select(x => x.ToString()).ToStringJoined().LogError();
            if(!_entriesLineNum.TryGetValue(entry, out int line))
                throw new Exception($"DataTable [{ name }] Entry [{ entry }] not found.");
            return line;
        }
        
        int GetColumnFromPropName(string propName)
        {
            if(!_propertiesColumnNum.TryGetValue(propName, out int index))
                throw new Exception($"DataTable [{ name }] Key [{ propName }] not found.");
            return index;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public bool HasKey(string name) => _propertiesColumnNum.ContainsKey(name);
        public bool HasEntry(string name) => _entriesLineNum.ContainsKey(name);
        public int LineNumOfEntry(string name)
        {
            if(!_entriesLineNum.TryGetValue(name, out int value))
                throw new Exception($"DataTable [{ name }] Entry { name } not found.");
            return value;
        }
        
        public int ColumnNumOfProperty(string name)
        {
            if(!_propertiesColumnNum.TryGetValue(name, out int value))
                throw new Exception($"DataTable [{ name }] Property { name } not found.");
            return value;
        }
        public string EntryOfIndex(int index) => _entriesLineNum.GetKeyByValue(index);
        public string KeyOfIndex(int index) => _propertiesColumnNum.GetKeyByValue(index);
        
    }
}
