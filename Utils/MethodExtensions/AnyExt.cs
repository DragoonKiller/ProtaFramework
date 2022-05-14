using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static T SwapSet<T>(this ref T t, T value) where T: struct
        {
            var original = t;
            t = value;
            return original;
        }
        
    }
}