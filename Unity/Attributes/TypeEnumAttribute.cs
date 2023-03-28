using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    public class TypeEnum : PropertyAttribute
    {
        public Type type;
        
        public bool allowNull = true;

        public TypeEnum(Type type)
        {
            this.type = type;
        }
    }
 
}
