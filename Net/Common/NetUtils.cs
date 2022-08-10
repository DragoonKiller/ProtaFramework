using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Prota.Net
{
    public class SerializePrioritryAttribute : Attribute
    {
        public int priority;

        public SerializePrioritryAttribute(int priority) => this.priority = priority;
    }
    
    public static class NetUtils
    {
        public static void Error(this CommonHeader header, string desc)
        {
            Console.WriteLine($"[Error] [{ header.seq } : { header.src } => { header.dst } : { header.protoId }] { desc }");
        }
        
        public static ArraySegment<byte> SegmentToTheEnd(this NetDataReader reader) => reader.RawData.AsSegment(reader.Position, reader.UserDataSize - (reader.Position - reader.UserDataOffset));
        public static int GetPosition(this NetDataWriter writer)
        {
            var ori = writer.SetPosition(0);
            writer.SetPosition(ori);
            return ori;
        }
        
        
        [ThreadStatic]
        static byte[] _tempBuffer;
        static byte[] tempBuffer => _tempBuffer == null ? _tempBuffer = new byte[64 * 1024] : _tempBuffer; // 64KB cache per thread.
        public static NetDataWriter PutProtaSerialize<T>(this NetDataWriter writer, T data)
        {
            var cnt = data.Serialize(tempBuffer);
            writer.Put(tempBuffer, 0, cnt);
            return writer;
        }
    }
}