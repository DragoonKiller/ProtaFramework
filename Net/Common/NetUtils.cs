using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

using LiteNetLib;
using LiteNetLib.Utils;

namespace Prota.Net
{
    public static class NetUtils
    {
        public static void Error(this CommonHeader header, string desc)
        {
            Console.WriteLine($"[Error] [{ header.seq } : { header.src } => { header.dst } : { header.protoId }] { desc }");
        }
    }
}