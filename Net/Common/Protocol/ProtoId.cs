using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Prota.Net
{
    
    
    public static partial class ProtoId
    {
        // 客户端向服务器建立连接. 服务器可以认为是一个大厅(loddy).
        // 和大厅建立连接后, 由客户端指定开房. 广播消息以"房"为单位; 房内的客户端可以相互通信.
        
        
        // 客户端向服务器请求连接上的客户端列表.
        public const int C2SReqClientList = -1;
        public const int S2CRspClientList = -2;
        
        
        static ProtoId()
        {
            var s = new Dictionary<int, FieldInfo>();
            foreach(var i in typeof(ProtoId).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                if(!(i.IsLiteral && !i.IsInitOnly)) continue;
                var v = (int)i.GetRawConstantValue();
                if(s.ContainsKey(v))
                {
                    Console.WriteLine($"[Error] enum conflict with { i.Name } and { s[v].Name }");
                }
                s.Add(v, i);
            }
        }
        
        public static void UnitTest()
        {
            
        }
    }
}