using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Prota.Animation;

namespace Prota.Editor
{
    [CustomEditor(typeof(CodeAnimationManager), false)]
    [ExecuteAlways]
    public class CodeAnimationManagerInspector : UpdateInspector
    {
        VisualElement root;
        
        public List<VisualElement> contents = new List<VisualElement>();
        
        public Dictionary<VisualElement, int> idMap = new Dictionary<VisualElement, int>();
        
        public override VisualElement CreateInspectorGUI()
        {
            contents.Clear();
            root = new VisualElement();
            return root;
        }

        protected override void Update()
        {
            var c = target as CodeAnimationManager;
            contents.SetLength(c.all.Count,
                i => {
                    var s = null as VisualElement;
                    s = new VisualElement()
                        .SetVerticalLayout()
                        .AddChild(new VisualElement()
                            .SetHorizontalLayout()
                            .AddChild(new ObjectField() { name = "guard", objectType = typeof(UnityEngine.Object) }
                                .SetMinWidth(120)
                            )
                            .AddChild(new ObjectField() { name = "target", objectType = typeof(UnityEngine.GameObject) }
                                .SetMinWidth(120)
                            )
                        )
                        .AddChild(new VisualElement()
                            .SetHorizontalLayout()
                            .AddChild(new TextField() { name = "time", label = "", isReadOnly = true }
                                .SetWidth(80)
                            )
                            .AddChild(new Slider() { name = "ratio", label = "", lowValue = 0, highValue = 1 }
                                .SetGrow()
                                .OnValueChange<Slider, float>(x => {
                                    c.all[idMap[s]].ratio = x.newValue;
                                })
                            )
                        );
                    root.AddChild(s);
                    return s;
                },
                (i, s) => {
                    idMap[s] = i;
                    s.Q<ObjectField>("guard").value = c.all[i].guard;
                    s.Q<ObjectField>("target").value = c.all[i].target;
                    s.Q<TextField>("time").value = (c.all[i].ratio * c.all[i].duration).ToString();
                    s.Q<Slider>("ratio").value = c.all[i].ratio;
                    s.SetVisible(true);
                },
                (i, s) => s.SetVisible(false)
            );
                
                
        }
    }

}