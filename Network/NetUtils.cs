using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

namespace Prota.Net
{
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
    }
}
