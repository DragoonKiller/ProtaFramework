using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using LiteNetLib.Utils;

namespace Prota.Net
{
    [ProtaProtocol(-6)]
    public struct S2CNtfClientId
    {
        public readonly NetId id;

        public S2CNtfClientId(NetId id) => this.id = id;
    }
}