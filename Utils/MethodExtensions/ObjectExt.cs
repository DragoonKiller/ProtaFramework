using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static bool DiffModify<T>(this ref T x, T value) where T : struct
        {
            var res = x.Equals(value);
            x = value;
            return res;
        }
        
        public static T PassValue<T>(this T x, out T value)
        {
            return value = x;
        }
        
        public static object CreateInstanceOfNonInitializedType(this Type x, params object[] args)
        {
            return Activator.CreateInstance(x, args);
        }
    }
}
