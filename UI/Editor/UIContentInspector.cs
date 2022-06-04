using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Concurrent;
using System.IO;

using Prota.UI;

namespace Prota.Editor
{
    [CustomEditor(typeof(UIContent), false)]
    public class UIContentInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            return new VisualElement();
        }
    
        [MenuItem("Component/ProtaFramework/UI/Add UIElement, _p")]
        static void AddComponent()
        {
            foreach(var g in Selection.objects)
            {
                if(g is GameObject gg) gg.GetOrCreate<UIContent>();
            }
        }
    }
}