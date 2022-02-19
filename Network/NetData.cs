using System.Collections.Generic;
using LiteNetLib.Utils;
using Prota.Unity;
using UnityEngine;
using XLua;

namespace Prota.Net
{
    public sealed class NetDataSchema : DataSchema
    {
        public NetDataSchema(List<DataEntry> entires) : base(entires)
        {
            
        }
    }

    public sealed class NetDataTable : DataTable
    {
        public NetDataTable(string name, DataSchema schema) : base(name, schema)
        {
            
        }
    }
    
    [LuaCallCSharp]
    [RequireComponent(typeof(Client))]
    public sealed class NetData : MonoBehaviour
    {
        public Client client => GetComponent<Client>();
        
        public readonly Dictionary<string, NetDataTable> data = new Dictionary<string, NetDataTable>();
        
        void AddTable(string name, List<DataEntry> entries)
        {
            data.Add(name, new NetDataTable(name, new DataSchema(entries)));
        }
        
        public void Start()
        {
            client.AddCallback(BuiltinMsgId.C2CAddRecord, (peer, reader, method) => {
                
            });
            
            client.AddCallback(BuiltinMsgId.C2CRemoveRecord, (peer, reader, method) => {
                // 结构:
                // [string] 表名称.
                // [1: *] 第一列数据.
                // [2: *] 第二列数据.
                // [3: *] 第三列数据.
                // ...
                var tableName = reader.GetString();
                var table = data[tableName];
                for(int i = 0; i < table.columnCount; i++)
                {
                }
            });
            
            client.AddCallback(BuiltinMsgId.C2CModifyData, (peer, reader, method) => {
                // 结构:
                // [string] 表名称.
                // [n: int] 有多少个修改条目.
                // [key: int] 使用哪个列数据作为行的索引.
                //   [1: *] 行索引值.
                //   [1: int] 列.
                //   [1: *] 数据.
                //   [2: *] 行索引值.
                //   [2: int] 列.
                //   [2: *] 数据.
                // ...
                var tableName = reader.GetString();
                var n = reader.GetInt();
                var table = data[tableName];
                for(int i = 0; i < n; i++)
                {
                    var row = reader.GetString();
                    var m = reader.GetInt();
                    for(int j = 0; j < m; j++)
                    {
                        var column = reader.GetInt();
                        switch(table.schema[column].type)
                        {
                            case DataType.Int32:
                            
                            break;
                            
                            case DataType.Int64:
                            
                            break;
                            
                            case DataType.Float32:
                            
                            break;
                            
                            case DataType.Float64:
                            
                            break;
                            
                            case DataType.String:
                            
                            break;
                        }
                    }
                }
            });
            
            client.AddCallback(BuiltinMsgId.C2CRequestModifyData, (peer, reader, method) => {
                
            });
        }
        
    }
    
    
    

}