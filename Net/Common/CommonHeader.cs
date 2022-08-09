using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Prota.Net
{
    public struct CommonHeader
    {
        public readonly NetSequenceId seq;       // 收发包序列号. notify = 0, request > 0, response < 0, 用绝对值对应 request/response.
        public readonly int protoId;
        public readonly NetId src;
        public readonly NetId dst;
        
        public const int size = sizeof(int) + 2 * NetId.internalSize + sizeof(int);
        
        public CommonHeader(NetSequenceId seq, NetId src, NetId dst, int protoId)
        {
            this.seq = seq;
            this.src = src;
            this.dst = dst;
            this.protoId = protoId;
        }
        
    }
    
    public static class CommonHeaderExt
    {
        public static CommonHeader GetCommonHeader(this NetDataReader reader, bool reset = false)
        {
            var seq = new NetSequenceId(reader.GetUShort());
            var src = new NetId(reader.GetUShort());
            var dst = new NetId(reader.GetUShort());
            var protoType = reader.GetInt();
            if(reset)
            {
                reader.SkipBytes(-CommonHeader.size);
            }
            return new CommonHeader(seq, src, dst, protoType);
        }
        
        public static NetDataWriter WriteHeader(this NetDataWriter writer, CommonHeader header, bool reset = false)
        {
            var curPos = writer.SetPosition(0);     // get original position by Set.
            writer.SetPosition(curPos);             // recover original position.
            
            writer.Put(header.seq);
            writer.Put(header.src.id);
            writer.Put(header.dst.id);
            writer.Put(header.protoId);
            if(reset)
            {
                writer.SetPosition(curPos);         // set position back.
            }
            return writer;
        }

    }
}