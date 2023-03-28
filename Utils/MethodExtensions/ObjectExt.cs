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
        
        public static int? NullableCompare(this object a, object b)
        {
            if(a == null && b == null) return 0;
            if(a != null) return 1;
            if(b != null) return -1;
            return null;
        }
    }
}
