using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public class AssertionFailedException : Exception
        {
            public AssertionFailedException() { }

            public AssertionFailedException(string message) : base(message) { }

            public AssertionFailedException(string message, Exception innerException) : base(message, innerException) { }
        }

        public static T AssertNotNull<T>(this T value, string message = null)
            where T: class
        {
            if(value == null) throw new AssertionFailedException(message);
            return value;
        }
        
        public static bool Assert(this bool value, string message = null)
        {
            if(!value) throw new AssertionFailedException(message);
            return value;
        }
    }
}