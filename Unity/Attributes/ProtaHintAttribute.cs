using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ProtaHint : Attribute
    {
        public string content;

        public ProtaHint(string content)
        {
            this.content = content;
        }
    }
 
}
