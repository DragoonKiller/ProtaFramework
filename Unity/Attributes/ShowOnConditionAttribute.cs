using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Prota.Unity
{
    public class ShowWhenAttribute : PropertyAttribute
    {
        public string name;
        
        public ShowWhenAttribute(string name)
        {
            this.name = name;
        }
    }
 
}
