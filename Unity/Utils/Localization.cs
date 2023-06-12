using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Prota;
using Prota.Unity;
using UnityEngine;

namespace Prota.Unity
{
    public static class Localization
    {
        static string TranslateInternal(this string x)
        {
            // TODO
            return x;
        }
        
        public static string T(this string x, params object[] values)
        {
            if(values.Length == 0) return x.TranslateInternal();
            
            x = string.Format(x, values.Select(y => y.ToString()).ToArray());
            
            return x;
        }
    }
}
