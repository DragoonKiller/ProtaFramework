using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Prota.Unity;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Prota.Unity
{
    
    
    // 表列定义.
    public class DataEntry
    {
        public string name;
        
        public DataType type;
        
        public bool isIndex;

        public DataEntry(string name, DataType type, bool isIndex = false)
        {
            this.name = name;
            this.type = type;
            this.isIndex = isIndex;
        }
    }
    
    
    // 表结构定义.
    public class DataSchema : IReadOnlyList<DataEntry>
    {
        public readonly List<DataEntry> entries = new List<DataEntry>();
        
        public readonly Dictionary<string, int> nameToEntryId = new Dictionary<string, int>();
        
        public DataSchema(List<DataEntry> entires)
        {
            this.entries = new List<DataEntry>(entires);
            SetupEntries();
        }
        
        void SetupEntries()
        {
            for(int i = 0; i < entries.Count; i++) nameToEntryId[entries[i].name] = i;
        }

        public DataEntry this[int index] => entries[index];

        public int Count => entries.Count;

        public IEnumerator<DataEntry> GetEnumerator() => entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
    
    
    
    public class DataColumn
    {
        public event Action<int> addCallbacks;
        public event Action<int, int> removeCallbacks;
        public event Action<int, DataValue, DataValue> modifyCallbacks;

        readonly List<RawDataValue> dataList = new List<RawDataValue>();
        readonly Dictionary<DataValue, int> index = null;
        
        public readonly string columnName;
        public readonly string tableName;
        public readonly int columnId;
        public readonly DataType type;
        public readonly bool isIndex;
        
        public int Count => dataList.Count;
        
        public DataValue this[int i]
        {
            get
            {
                if(!IndexValid(i)) return default;
                return new DataValue(this.type, dataList[i]);
            }
            
            set
            {
                if(!IndexValid(i)) return;
                if(!TestDuplicatedIndexValue(i, value)) return;
                var original = dataList[i];
                dataList[i] = value.rawValue;
                modifyCallbacks?.Invoke(i, new DataValue(this.type, original), value);
            }
        }
        
        internal DataColumn(string tableName, string columnName, bool isIndex, int columnId, DataType type)
        {
            this.tableName = tableName;
            this.columnName = columnName;
            this.type = type;
            this.columnId = columnId;
            this.isIndex = isIndex;
            
            if(isIndex)
            {
                index = new Dictionary<DataValue, int>();
                
                addCallbacks += i => {
                    index.Add(this[i], i);
                };
                
                removeCallbacks += (i, swap) => {
                    index.Remove(this[i]);
                };
                
                modifyCallbacks += (i, from, to) => {
                    index.Remove(from);
                    index.Add(to, i);
                };
            }
        }

        internal void InvokeAddCallbacks(int i) => addCallbacks?.Invoke(i);
        
        internal void InvokeRemoveCallbacks(int i, int swap) => removeCallbacks?.Invoke(i, swap);
        
        public int IndexOfValue(DataValue value)
        {
            if(index != null)
            {
                if(index.TryGetValue(value, out var i)) return i;
            }
            return dataList.IndexOf(value.rawValue);
        }
        
        internal bool Add(DataValue value)
        {
            if(!TypeValid(value.type)) return false;
            if(!TestDuplicatedIndexValue(-1, value)) return false;
            dataList.Add(value.rawValue);
            return true;
        }
        
        internal DataValue RemoveBySwap(int i)
        {
            if(!IndexValid(i)) return default;
            var value = dataList[i];
            dataList.RemoveBySwap(i);
            return new DataValue(this.type, value);
        }
        
        bool TestDuplicatedIndexValue(int i, DataValue value)
        {
            if(index == null) return true;
            if(index.TryGetValue(value, out var line))
            {
                if(line == i) return true;  // 重复赋相同的值.
                Log.Error($"{ this }: 索引的值重复了: 值[{ value }] 原有行[{ line }]");
                return false;
            }
            return true;
        }
        
        bool IndexValid(int i)
        {
            var valid = 0 <= i && i < dataList.Count;
            if(!valid) Log.Error($"{ this }: 索引越界 { i } n: { dataList.Count }");
            return valid;
        }
        
        bool TypeValid(DataType type)
        {
            if(type != this.type)
            {
                Log.Error($"{ this }: 添加值的类型错误, 应为[{ this.type }] 实为[{ type }]");
                return false;
            }
            return true;
        }
        
        public override string ToString() => $"DataColumn[ table[{ tableName }] column({ columnId })[{ columnName }] ]";
        
    }
        
        
    
    // 数据表.
    public class DataTable
    {
        // 添加行的逻辑.
        //会绕过 modify***Callback.
        public sealed class AddAccessor
        {
            internal readonly DataTable table;
            int column = 0;      // 列号.
            bool valid = true;
            
            internal AddAccessor(DataTable table)
            {
                this.table = table;
            }
            
            internal AddAccessor Start()
            {
                column = 0;
                valid = true;
                return this;
            }
            
            public AddAccessor Add(DataValue value)
            {
                if(!valid) return this;
                if(!TestColumnCount()) return this;
                if(!table.data[column].Add(value)) return Interrupt();
                column += 1;
                return this;
            }
            
            // 返回行号.
            internal int Complete()
            {
                if(!valid) return -1;
                
                if(column != table.columnCount)
                {
                    Log.Error($"{this}: 填写的数据数量错误! 赋值了 { column } 个, 实际上需要 { table.columnCount } 个.");
                    Interrupt();
                    return -1;
                }
                
                var newRow = table.Count;
                table.Count += 1;
                
                for(int i = 0; i < column; i++)
                {
                    table.data[i].InvokeAddCallbacks(newRow);
                }
                
                return table.Count - 1;
            }
            
            bool TestColumnCount()
            {
                if(column >= table.columnCount)
                {
                    Log.Error($"{this}: 添加了过多的数据, 只能接收 { table.columnCount } 条数据.");
                    Interrupt();
                    return false;
                }
                return true;
            }
            
            AddAccessor Interrupt()
            {
                if(!valid) return this;
                valid = false;
                Log.Error($"{this}: 添加数据行发生错误, 回退修改.");
                for(int i = 0; i < column; i++)
                {
                    table.data[i].RemoveBySwap(i);
                }
                return this;
            }
            
            
        }
        
        
        public readonly DataSchema schema;
        
        public readonly string name;
        
        public readonly List<DataColumn> data = new List<DataColumn>();
        
        readonly AddAccessor add;
        
        // 有多少数据行.
        public int Count { get; private set; }
        
        public int lineCount => Count;
        
        public int columnCount => schema.Count;
        
        public DataColumn Data(int i) => data[i];
        
        public DataColumn DataByName(string name) => data[ColumnNameToId(name)];
        
        public DataTable(string name, DataSchema schema)
        {
            this.name = name;
            this.schema = schema;
            add = new AddAccessor(this);
            Count = 0;
            
            for(int i = 0; i < schema.entries.Count; i++)
            {
                var entry = schema.entries[i];
                data.Add(new DataColumn(name, entry.name, entry.isIndex, i, entry.type));
            }
            
        }
        
        // 添加一个条目.
        // 只需要获取 AddAccessor 然后填数据即可.
        public int AddRecord(Action<AddAccessor> f)
        {
            var s = add.Start();
            f(s);
            return add.Complete();
        }
        
        public int AddRecord(List<DataValue> values)
        {
            var s = add.Start();
            foreach(var v in values) s.Add(v);
            return add.Complete();
        }
        
        public List<DataValue> RemoveRecord(int x)
        {
            // 删除只有索引越界一种情况, 而这种情况可以直接被这个地方的代码挡住, 这样删除操作总是会成功.
            if(!(0 <= x && x < Count))
            {
                Log.Error($"尝试删除表中一行时, 索引越界: 表[{ name }] 总行数[{ Count }] 删除行[{ x }]");
                return null;
            }
            
            var list = new List<DataValue>();
            
            var swap = Count - 1;
            for(int i = 0; i < columnCount; i++)
            {
                data[i].InvokeRemoveCallbacks(x, swap);
            }
            
            for(int i = 0; i < columnCount; i++)
            {
                list.Add(data[i].RemoveBySwap(x));
            }
            
            Count -= 1;
            return list;
        }
        
        public int ColumnNameToId(string name) => schema.nameToEntryId[name];
        
        public string ColumnIdToName(int columnId) => schema[columnId].name;
        
        
        
        public override string ToString() => $"DataTable[{ name }]";
    }
    
    

}