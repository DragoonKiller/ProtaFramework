using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prota
{
    
    public static partial class MethodExtensions
    {
        public static bool NullOrEmpty(this string s) => string.IsNullOrEmpty(s);
        
        public static string Log(this string x)
        {
            Console.WriteLine(x);
            return x;
        }
        
        public static string LogError(this string x)
        {
            Console.Error.WriteLine(x);
            return x;
        }
        
        public static StringBuilder ToStringBuilder(this string x)
        {
            var s = new StringBuilder();
            s.Append(x);
            return s;
        }
        
        [ThreadStatic] static StringBuilder _stringBuilder;
        public static string Join(this IEnumerable<string> list, string separator)
        {
            if(_stringBuilder == null) _stringBuilder = new StringBuilder();
            _stringBuilder.Clear();
            var first = true;
            foreach(var s in list)
            {
                if(first) first = false;
                else _stringBuilder.Append(separator);
                _stringBuilder.Append(s);
            }
            return _stringBuilder.ToString();
        }
        
        public static string ToByteSeqString(this string s)
        {
            return s.Select(x => (int)x).ToStringJoined(",");
        }
    }
    
}
