using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static IEnumerable<string> EnumeratePropertieNames(this Shader shader)
        {
            var count = shader.GetPropertyCount();
            for (int i = 0; i < count; i++)
            {
                yield return shader.GetPropertyName(i);
            }
        }
        
        public static IEnumerable<string> EnumeratePropertieNames(this Material material)
        {
            return material.shader.EnumeratePropertieNames();
        }
        
        public static ShaderPropertyType GetPropertyType(this Shader shader)
        {
            var count = shader.GetPropertyCount();
            for (int i = 0; i < count; i++)
            {
                shader.GetPropertyType(i);
            }
            return ShaderPropertyType.Float;
        }
    }
}
