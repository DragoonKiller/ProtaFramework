using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Prota.Net
{
    
    
    public static partial class ProtoId
    {
        // 客户端向服务器建立连接. 服务器可以认为是一个大厅(loddy).
        // 和大厅建立连接后, 由客户端指定开房. 广播消息以"房"为单位; 房内的客户端可以相互通信.
        
        
        // 客户端向服务器请求连接上的客户端列表.
        public static int C2SReqClientList = -1;
        public static int S2CRspClientList = -2;
        
        // 客户端进入和退出房间.
        public static int C2SReqEnterRoom = -3;
        public static int S2CRspEnterRoom = -4;
        public static int S2CNtfOtherEnterRoom = -5;
        public static int C2SReqExitRoom = -6;
        public static int S2CRspExitRoom = -7;
    }
}