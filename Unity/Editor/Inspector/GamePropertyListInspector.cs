using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

using Prota.Unity;
using UnityEditor.UIElements;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEditor.PackageManager;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace Prota.Editor
{
    [CustomEditor(typeof(GamePropertyList), false)]
    public class GamePropertyListInspector : UpdateInspector
    {
        VisualElement lList;
        
        Button addButton;
        
        Button remoevButton;
        
        TextField nameField;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var list = serializedObject.targetObject as GamePropertyList;
            
            
            if(lList == null) lList = new VisualElement();
            root.AddChild(lList);
            
            root.AddChild(new VisualElement()
                .SetHorizontalLayout()
                .SetGrow()
                .AddChild(nameField = new TextField() { }
                    .SetGrow()
                )
                .AddChild(addButton = new Button(Add) { text = "add" }
                    .SetMaxWidth(80)
                )
                .AddChild(remoevButton = new Button(Remove) { text = "remove" }
                    .SetMaxWidth(80)
                )
            );
            
            return root;
        }
        
        void Add()
        {
            var name = nameField.value;
            var tgt = serializedObject.targetObject as GamePropertyList;
            tgt.Add(name, 0);
        }
        
        void Remove()
        {
            var name = nameField.value;
            var tgt = serializedObject.targetObject as GamePropertyList;
            tgt.Remove(name);
        }
        
        protected override void Update()
        {
            if(lList == null) return;
            
            using var _ = TempList<int>.Get(out var list);
            
            var filter = nameField.value;
            var arrayProperty = serializedObject.FindProperty("properties._entries.data");
            for(int i = 0; i < arrayProperty.arraySize; i++)
            {
                var inUseProp = serializedObject.FindProperty($"properties._entries.data.Array.data[{i}].inuse");
                var nameProp = serializedObject.FindProperty($"properties._entries.data.Array.data[{i}].value.value.{"name".ToBackingFieldName()}");
                bool matched = filter.NullOrEmpty()
                    || nameProp.stringValue.Contains(filter, System.StringComparison.OrdinalIgnoreCase);
                if(inUseProp.boolValue && matched) list.Add(i);
            }
            
            lList.SyncData(list.Count, i => {
                return new PropertyField();
            }, (i, e) => {
                var id = list[i];
                var bindingPath = $"properties._entries.data.Array.data[{id}].value.value";
                if(e.bindingPath != bindingPath)
                {
                    e.bindingPath = bindingPath;
                    e.Bind(serializedObject);
                }
                e.SetVisible(true);
            }, (i, e) => {
                e.SetVisible(false);
            });
        }
    }
}
