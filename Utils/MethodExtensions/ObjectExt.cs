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
    } 
}