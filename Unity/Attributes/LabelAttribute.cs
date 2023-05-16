using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class LabelAttribute : PropertyAttribute
    {
        public string name;

        public LabelAttribute(string name)
        {
            this.name = name;
        }
    }
 
}
