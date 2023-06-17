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
using static Prota.Unity.GameProperty;
using System.Linq.Expressions;

namespace Prota.Editor
{
    [CustomEditor(typeof(GamePropertyList), false)]
    [CanEditMultipleObjects]
    public class GamePropertyListInspector : UpdateInspector
    {
        VisualElement lList;
        
        Button addButton;
        
        Button remoevButton;
        
        TextField nameField;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
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
            foreach(var s in serializedObject.targetObjects)
            {
                var tgt = s as GamePropertyList;
                tgt.Add(name, 0);
            }
        }
        
        void Remove()
        {
            var name = nameField.value;
            foreach(var s in serializedObject.targetObjects)
            {
                var tgt = s as GamePropertyList;
                tgt.Remove(name);
            }
        }
        
        protected override void Update()
        {
            if(lList == null) return;
            if(serializedObject.targetObject == null) return;
            
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
                return new VisualElement()
                    .AddChild(new PropertyField() { name = "g" })
                    .AddChild(new VisualElement() { name = "m" });
            }, (i, e) => {
                var id = list[i];
                
                var g = e.Q<PropertyField>("g");
                var bindingPath = $"properties._entries.data.Array.data[{id}].value.value";
                if(g.bindingPath != bindingPath)
                {
                    g.bindingPath = bindingPath;
                    g.Bind(serializedObject);
                }
                
                bindingPath = $"properties._entries.data.Array.data[{id}].value.value.{"name".ToBackingFieldName()}";
                var name = serializedObject.FindProperty(bindingPath).stringValue;
                var tgt = serializedObject.targetObject as GamePropertyList;
                var actualObject = tgt[name];
                var modifiers = actualObject.ProtaReflection().Get<ArrayLinkedList<Modifier>>("modifiers");
                var m = e.Q<VisualElement>("m");
                m.SyncData(modifiers, j => {
                    return new TextField();
                }, (modifier, e2) => {
                    var addBase = modifier.addBase == 0 ? "" : $"+base[{ modifier.addBase }]";
                    var mulBase = modifier.mulBase == 0 ? "" : $"*base[{ modifier.mulBase }]";
                    var addFinal = modifier.addFinal == 0 ? "" : $"+final[{ modifier.addFinal }]";
                    var mulFinal = modifier.mulFinal == 1 ? "" : $"*final[{ modifier.mulFinal }]";
                    e2.value = $"{addBase} {mulBase} {addFinal} {mulFinal}";
                    e2.SetNoInteraction();
                });
                
                e.SetVisible(true);
            }, (i, e) => {
                e.SetVisible(false);
            });
        }
    }
}
