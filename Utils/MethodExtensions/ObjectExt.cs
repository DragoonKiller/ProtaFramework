using System;
using System.Collections.Generic;
using System.Text;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static ref T Swap<T>(this ref T t, ref T value) where T: struct
        {
            var original = t;
            t = value;
            value = original;
            return ref t;
        }
        
        public static T SwapSet<T>(this ref T t, T value) where T: struct
        {
            var original = t;
            t = value;
            return original;
        }
        
        public static bool SetAndCompare<T>(this ref T x, T value) where T : struct
        {
            var res = x.Equals(value);
            x = value;
            return res;
        }
        
        public static T PassValue<T>(this T x, out T value)
        {
            return value = x;
        }
        
        public static IEnumerable<string> ToStrings(this IEnumerable<object> x)
        {
            foreach(var i in x)
            {
                yield return i.ToString();
            }
        }
        
        public static string ToStringJoined(this IEnumerable<object> x, string separator = "\n")
        {
            return string.Join(separator, x.ToStrings());
        }
        
        
        public static bool IsAllOfType(this IEnumerable<object> x, Type type)
        {
            foreach(var i in x)
            {
                if(i == null) continue;
                if(!type.IsAssignableFrom(i.GetType())) return false;
            }
            return true;
        }
        
        public static bool IsAllOfType<T>(this IEnumerable<object> x)
        {
            return x.IsAllOfType(typeof(T));
        }
        
        public static bool ConvertAllToType(this IEnumerable<object> x, Type type, out List<object> result)
        {
            result = new List<object>();
            foreach(var i in x)
            {
                if(i == null) continue;
                if(!type.IsAssignableFrom(i.GetType())) return false;
                result.Add(i);
            }
            return true;
        }
        
        public static bool ConvertAllToType<T>(this IEnumerable<object> x, out List<T> result)
        {
            result = new List<T>();
            foreach(var i in x)
            {
                if(i == null) continue;
                if(!(i is T)) return false;
                result.Add((T)i);
            }
            return true;
        }
    }
}
