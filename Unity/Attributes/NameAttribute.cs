using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class NameAttribute : PropertyAttribute
    {
        public string name;

        public NameAttribute(string name)
        {
            this.name = name;
        }
    }
 
}
