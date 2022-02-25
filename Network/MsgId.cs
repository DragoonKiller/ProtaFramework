using UnityEngine;
using System.Collections.Generic;


namespace Prota.Net
{
    public static class BuiltinMsgId
    {
        // 告知服务器内容.
        public const int S2CNotifyInfo = -1;
        
        // 告知有客户端连接/断开服务器.
        public const int S2CAddOrRemoveClient = -2;
        
        // 告知服务器客户端的一些基础信息.
        public const int C2SReportInfo = -10;
        
        // P2P 连接 request.
        public const int C2SRequestP2PConnection = -101;
        public const int S2CResponseP2PConnection = -101;
        
        
        // 建立 P2P 连接后, 客户端向主机发送逻辑接入请求.
        public const int C2CRequestClientConnection = -102;
        // 主机受到客户端的请求, 回复是否接收成功.
        public const int C2CResponseClientConnection = -103;
        
        
        // 向服务器声明自己是/不是主机. 服务器通知所有连接.
        public const int C2SRequestHost = -110;
        public const int S2CNotifyHost = -110;
        
        // 使用服务器向另一个客户端传输数据.
        public const int C2CSendByServer = -1000;
        
    }
    
    
    
    public static partial class MsgId
    {
        public const int C2CLog = 1001;
        
        
    }
}