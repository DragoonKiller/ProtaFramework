using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Prota.Editor
{
    public static partial class UnityMethodExtensions
    {
        static Dictionary<Type, Texture> cache = new Dictionary<Type, Texture>();
        
        public static Texture FindEditorIcon(this UnityEngine.Object x)
        {
            if(x == null) return null;
            if(cache.ContainsKey(x.GetType())) return cache[x.GetType()];
            
            UnityEngine.Profiling.Profiler.BeginSample("FindEditorIcon");
            var guiContent = EditorGUIUtility.ObjectContent(x, x.GetType());
            var image = guiContent.image;
            UnityEngine.Profiling.Profiler.EndSample();
            return cache[x.GetType()] = image;
        }
    }
}
