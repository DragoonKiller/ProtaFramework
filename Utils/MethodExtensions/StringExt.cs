using System;

namespace Prota
{
    
    public static class StringExt
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
    }
    
}