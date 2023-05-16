
using System;

namespace Prota
{
    public static class Nothing
    {
        [Serializable] public class Class { }
        [Serializable] public struct Struct { }
        public static Action DoNothing = () => { };
    }
}
