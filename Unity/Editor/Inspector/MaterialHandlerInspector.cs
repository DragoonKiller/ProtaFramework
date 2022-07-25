using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.UIElements;

namespace Prota.Editor
{
    [CustomEditor(typeof(MaterialHandler), false)]
    public class MaterialHandlerInspector : UpdateInspector
    {
        
        MaterialHandler t => target as MaterialHandler;
            
        VisualElement matInstanceRoot;
        
        List<ObjectField> matInstances = new List<ObjectField>();
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            root.AddChild(new PropertyField(serializedObject.FindProperty("isShared")));
            root.AddChild(new PropertyField(serializedObject.FindProperty("materialTemplates")));
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            root.AddChild(matInstanceRoot = new VisualElement()
                .SetMaxHeight(400)
            );
            
            return root;
        }
        
        protected override void Update()
        {
            matInstances.SetLength(t.mats?.Length ?? 0,
                i => {
                    var s = new ObjectField() { pickingMode = PickingMode.Ignore };
                    s.SetParent(matInstanceRoot);
                    return s;
                },
                (i, v) => {
                    v.SetVisible(true);
                    v.SetEnabled(false);
                    v.value = t.mats[i];
                },
                (i, v) => {
                    v.SetVisible(false);
                }
            );
        }
    }
}
        