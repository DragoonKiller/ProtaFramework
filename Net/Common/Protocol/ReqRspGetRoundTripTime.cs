using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using LiteNetLib.Utils;

namespace Prota.Net
{
    [ProtaProtocol(-7)]
    public struct C2AReqPing
    {
        public string info;
    }
    
    [ProtaProtocol(-8)]
    public struct A2CRspPing
    {
        public string info;
    }
}