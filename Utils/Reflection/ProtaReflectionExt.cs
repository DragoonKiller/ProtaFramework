
using System;

namespace Prota
{

    public class ProtaReflectionFailException : Exception
    {
        public ProtaReflectionFailException() { }
        public ProtaReflectionFailException(string message) : base(message) { }
    }
        
    public static partial class MethodExtensions
    {
        public static ProtaReflectionObject ProtaReflection(this object x) => new ProtaReflectionObject(x);
        
        public static ProtaReflectionType ProtaReflection(this Type x) => new ProtaReflectionType(x);
    }

}
