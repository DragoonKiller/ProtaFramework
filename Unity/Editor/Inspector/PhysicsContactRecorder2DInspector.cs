using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

using Prota.Unity;
using Prota.Editor;
using UnityEditor.UIElements;
using System.Linq;
using UnityEngine.UI;

namespace Prota.Editor
{
    [CustomEditor(typeof(PhysicsContactRecorder2D), false)]
    public class PhysicsContact2DRecorderInspector : UpdateInspector
    {
        VisualElement list;
        PhysicsContactRecorder2D t => serializedObject.targetObject as PhysicsContactRecorder2D;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement() { name = "root" };
            root.AddChild(new PropertyField(serializedObject.FindProperty("layerMask")));
            root.AddChild(list = new VisualElement() { name = "visList" });
            return root;
        }

        protected override void Update()
        {
            if(list == null) return;
            if(t == null) return;
            if(t.colliders == null) return;
            
            var v = t.colliders.ToList();
            list.SyncData(v.Count,
                i => {
                    var e = new VisualElement()
                        .SetHorizontalLayout()
                        .AddChild(new LayerMaskField() { name = "layer" }
                            .SetWidth(80)
                        )
                        .AddChild(new Label() { name = "type" })
                        .AddChild(new ObjectField() { name = "obj" }
                            .SetGrow()
                        );
                    list.AddChild(e);
                    return e;
                },
                (i, e) => {
                    e.Q<LayerMaskField>("layer").value = v[i].gameObject.layer;
                    e.Q<ObjectField>("obj").value = v[i];
                    e.Q<Label>("type").text = v[i].isTrigger ? "T" : "C";
                    e.SetVisible(true);
                },
                (i, e) => {
                    e.SetVisible(false);
                }
            );
        }
    }
}
