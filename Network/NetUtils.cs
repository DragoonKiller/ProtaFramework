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
        }
        
        /* 协议规则:
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
        */  
        
        
        static long intKey;
        static double numberKey;
        static string strKey;
        
        static long intValue;
        static double numberValue;
        static string strValue;
        
        
        // 返回协议表示的第一个 LuaTable.
        public static void Decode(LuaTable result, NetDataReader reader)
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
                    default: UnityEngine.Debug.LogError("未知的值类型 " + typeKey); break;
                }
                
                switch((DataType)typeValue)
                {
                    case DataType.Integer: intValue = reader.GetLong(); break;
                    case DataType.Number: numberValue = reader.GetDouble(); break;
                    case DataType.String: strValue = reader.GetString(); break;
                    default: UnityEngine.Debug.LogError("未知的值类型 " + typeValue); break;
                }
                
                switch((typeKey * 10) + typeValue)
                {
                    case (byte)DataType.Integer * 10 +  (byte)DataType.Integer: result.Set(intKey, intValue); break;
                    case (byte)DataType.Number * 10 +   (byte)DataType.Integer: result.Set(numberKey, intValue); break;
                    case (byte)DataType.String * 10 +   (byte)DataType.Integer: result.Set(strKey, intValue); break;
                    case (byte)DataType.Integer * 10 +  (byte)DataType.Number:  result.Set(intKey, numberValue); break;
                    case (byte)DataType.Number * 10 +   (byte)DataType.Number:  result.Set(numberKey, numberValue); break;
                    case (byte)DataType.String * 10 +   (byte)DataType.Number:  result.Set(strKey, numberValue); break;
                    case (byte)DataType.Integer * 10 +  (byte)DataType.String:  result.Set(intKey, strValue); break;
                    case (byte)DataType.Number * 10 +   (byte)DataType.String:  result.Set(numberKey, strValue); break;
                    case (byte)DataType.String * 10 +   (byte)DataType.String:  result.Set(strKey, strValue); break;
                    default: UnityEngine.Debug.LogError("未知的组合类型 " + ((typeKey * 10) + typeValue)); break;
                }
            }
        }
        
        static NetDataWriter writerCache;
        public static void Encode(NetDataWriter writer, LuaTable data)
        {
            writerCache = writer;
            data.ForEach<long, long>((k, v) => {
                writerCache.Put((byte)DataType.Integer);
                writerCache.Put((byte)DataType.Integer);
                writerCache.Put(k);
                writerCache.Put(v);
            });
            data.ForEach<long, double>((k, v) => {
                writerCache.Put((byte)DataType.Integer);
                writerCache.Put((byte)DataType.Number);
                writerCache.Put(k);
                writerCache.Put(v);
            });
            data.ForEach<long, string>((k, v) => {
                writerCache.Put((byte)DataType.Integer);
                writerCache.Put((byte)DataType.String);
                writerCache.Put(k);
                writerCache.Put(v);
            });
            data.ForEach<double, long>((k, v) => {
                writerCache.Put((byte)DataType.Number);
                writerCache.Put((byte)DataType.Integer);
                writerCache.Put(k);
                writerCache.Put(v);
            });
            data.ForEach<double, double>((k, v) => {
                writerCache.Put((byte)DataType.Number);
                writerCache.Put((byte)DataType.Number);
                writerCache.Put(k);
                writerCache.Put(v);
            });
            data.ForEach<double, string>((k, v) => {
                writerCache.Put((byte)DataType.Number);
                writerCache.Put((byte)DataType.String);
                writerCache.Put(k);
                writerCache.Put(v);
            });
            data.ForEach<string, long>((k, v) => {
                writerCache.Put((byte)DataType.String);
                writerCache.Put((byte)DataType.Integer);
                writerCache.Put(k);
                writerCache.Put(v);
            });
            data.ForEach<string, double>((k, v) => {
                writerCache.Put((byte)DataType.String);
                writerCache.Put((byte)DataType.Number);
                writerCache.Put(k);
                writerCache.Put(v);
            });
            data.ForEach<string, string>((k, v) => {
                writerCache.Put((byte)DataType.String);
                writerCache.Put((byte)DataType.String);
                writerCache.Put(k);
                writerCache.Put(v);
            });
        }
    }
}