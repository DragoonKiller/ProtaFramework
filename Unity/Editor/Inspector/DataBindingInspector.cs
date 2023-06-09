using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

using Prota.Unity;
using Prota.Editor;
using UnityEditor.UIElements;
using System.Linq;
using NUnit.Framework;

namespace Prota.Editor
{
    [CustomEditor(typeof(DataBinding), false)]
    public class DataBindingInspector : UpdateInspector
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var binding = serializedObject.targetObject as DataBinding;
            var data = binding.ProtaReflection().Get("data") as List<DataBinding.Entry>;
            
            
            root.AddChild(new PropertyField(serializedObject.FindProperty("includeSelf")));
            root.AddChild(new PropertyField(serializedObject.FindProperty("featureCharacter")));
            root.AddChild(new ListView(data, -1, MakeItem, BindItem).PassValue(out var list).SetMaxHeight(500));
            
            return root;
            
            VisualElement MakeItem()
            {
                return new ObjectField().SetNoInteraction();
            }
            
            void BindItem(VisualElement x, int i)
            {
                var g = x as ObjectField;
                var entry = data[i];
                g.value = entry.target;
            }
        }
        
        protected override void Update()
        {
            var binding = serializedObject.targetObject as DataBinding;
            
            
        }
    }
}
