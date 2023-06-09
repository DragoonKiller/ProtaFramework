
using System;

namespace Prota
{
    public static class Nothing
    {
        [Serializable] public class Class { }
        static Class _nothingClass = new Class();
        public static Class nothingClass => _nothingClass;
        [Serializable] public struct Struct { }
        public static Struct nothingStruct => default;
        public static Action DoNothing = () => { };
    }
}
