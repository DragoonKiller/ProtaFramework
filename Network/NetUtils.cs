using XLua;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace Prota.Net
{
    [LuaCallCSharp]
    public static class NetUtils
    {
        public enum DataType : byte
        {
            Integer = 1,
            Number = 2,
            String = 3,
            Table = 4,
        }
        
        /* 协议规则:
        [int] n table的个数, 至少是1
        table 1:
            [int] n: table 里的条目数量
            data 1:
                [byte] type: key 数据的类型.
                [byte] type: value 数据的类型.
                [*] key: 根据上面的类型表示的数据.
                [*] value: 根据上面的类型表示的数据.
            data 2:
                ...
            data 3:
                ...
        table 2:
        */  
        
        
        static long intKey;
        static double numberKey;
        static string strKey;
        static int tableKey;
        
        
        static long intValue;
        static double numberValue;
        static string strValue;
        static int tableValue;
        
        
        
        // 返回协议表示的第一个 LuaTable.
        public static LuaTable Decode(LuaEnv lua, NetDataReader reader)
        {
            int n = reader.GetInt();
            
            var res = new List<LuaTable>();
            for(int i = 0; i < n; i++) res.Add(lua.NewTable());
            
            for(int i = 0; i < n; i++)
            {
                int size = reader.GetInt();
                for(int j = 0; j < size; j++)
                {
                    byte typeKey = reader.GetByte();
                    byte typeValue = reader.GetByte();
                    
                    switch((DataType)typeKey)
                    {
                        case DataType.Integer: intKey = reader.GetLong(); break;
                        case DataType.Number: numberKey = reader.GetDouble(); break;
                        case DataType.String: strKey = reader.GetString(); break;
                        case DataType.Table: tableKey = reader.GetInt(); break;
                        default: UnityEngine.Debug.LogError("未知的值类型 " + typeKey); break;
                    }
                    
                    switch((DataType)typeValue)
                    {
                        case DataType.Integer: intValue = reader.GetLong(); break;
                        case DataType.Number: numberValue = reader.GetDouble(); break;
                        case DataType.String: strValue = reader.GetString(); break;
                        case DataType.Table: tableValue = reader.GetInt(); break;
                        default: UnityEngine.Debug.LogError("未知的值类型 " + typeValue); break;
                    }
                    
                    switch((typeKey * 10) + typeValue)
                    {
                        case (byte)DataType.Integer * 10 +  (byte)DataType.Integer: res[i].Set(intKey, intValue); break;
                        case (byte)DataType.Number * 10 +   (byte)DataType.Integer: res[i].Set(numberKey, intValue); break;
                        case (byte)DataType.String * 10 +   (byte)DataType.Integer: res[i].Set(strKey, intValue); break;
                        case (byte)DataType.Table * 10 +    (byte)DataType.Integer: res[i].Set(res[tableKey], intValue); break;
                        case (byte)DataType.Integer * 10 +  (byte)DataType.Number:  res[i].Set(intKey, numberValue); break;
                        case (byte)DataType.Number * 10 +   (byte)DataType.Number:  res[i].Set(numberKey, numberValue); break;
                        case (byte)DataType.String * 10 +   (byte)DataType.Number:  res[i].Set(strKey, numberValue); break;
                        case (byte)DataType.Table * 10 +    (byte)DataType.Number:  res[i].Set(res[tableKey], numberValue); break;
                        case (byte)DataType.Integer * 10 +  (byte)DataType.String:  res[i].Set(intKey, strValue); break;
                        case (byte)DataType.Number * 10 +   (byte)DataType.String:  res[i].Set(numberKey, strValue); break;
                        case (byte)DataType.String * 10 +   (byte)DataType.String:  res[i].Set(strKey, strValue); break;
                        case (byte)DataType.Table * 10 +    (byte)DataType.String:  res[i].Set(res[tableKey], strValue); break;
                        case (byte)DataType.Integer * 10 +  (byte)DataType.Table:   res[i].Set(intKey, res[tableValue]); break;
                        case (byte)DataType.Number * 10 +   (byte)DataType.Table:   res[i].Set(numberKey, res[tableValue]); break;
                        case (byte)DataType.String * 10 +   (byte)DataType.Table:   res[i].Set(strKey, res[tableValue]); break;
                        case (byte)DataType.Table * 10 +    (byte)DataType.Table:   res[i].Set(res[tableKey], res[tableValue]); break;
                        default: UnityEngine.Debug.LogError("未知的组合类型 " + ((typeKey * 10) + typeValue)); break;
                    }
                }
            }
            
            return res[0];
        }
        
        public static void Encode(LuaEnv lua, NetDataWriter writer, LuaTable data)
        {
            
        }
        
    }
}