using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Prota.Net
{
    [StructLayout(LayoutKind.Sequential)]
    [ProtaSerialize]
    public struct CommonHeader
    {
        public readonly NetSequenceId seq;       // 收发包序列号. notify = 0, request > 0, response < 0, 用绝对值对应 request/response.
        public readonly NetId src;
        public readonly NetId dst;
        public readonly int protoId;
        
        [IgnoreSerialize]
        public const int size = sizeof(int) + 2 * NetId.internalSize + sizeof(int);
        
        public CommonHeader(NetSequenceId seq, NetId src, NetId dst, int protoId)
        {
            this.seq = seq;
            this.src = src;
            this.dst = dst;
            this.protoId = protoId;
        }
        
    }
}