using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    
    // 给 ProtaInspector 的组件文字提示.
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
