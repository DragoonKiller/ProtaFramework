using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using LiteNetLib;

namespace Prota.Net
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ProtaProtocol : ProtaSerializeAttribute
    {
        public int id;
        public DeliveryMethod method = DeliveryMethod.ReliableOrdered;
        ProtaProtocol() { }
        public ProtaProtocol(int id) => this.id = id;
        public ProtaProtocol(int id, DeliveryMethod method) => (this.id, this.method) = (id, method);
    }
    
    
}