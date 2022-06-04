using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Concurrent;
using System.IO;

using Prota.UI;
using System.Collections.Generic;

namespace Prota.Editor
{
    [CustomEditor(typeof(UIBinding), false)]
    public class UIBindingInspector : UpdateInspector
    {
        UIBinding x => target as UIBinding;
        
        VisualElement root;
        
        ScrollView elementList;
        
        readonly Dictionary<string, VisualElement> elements = new Dictionary<string, VisualElement>();
        
        public override VisualElement CreateInspectorGUI()
        {
            return root = new VisualElement()
                .AddChild(new Button(() => { SetData(null, null); this.Update(); }) { text = "刷新数据" })
                .AddChild(elementList = new ScrollView(ScrollViewMode.Vertical)
                    .SetGrow()
                    .SetMaxHeight(400)
                );
        }
        
        
        protected override void Update()
        {
            if(root == null) return;
            
            elements.SetSync(x.rawContent,
                (key, element) => {
                    var v = new VisualElement()
                        .SetGrow()
                        .SetHorizontalLayout()    
                        .AddChild(new Label() { name = "label", text = key }
                            .SetWidth(120)
                        )
                        .AddChild(new ObjectField() { name = "target", value = element, objectType = typeof(UIContent) }
                            .SetGrow()
                        );
                    elementList.Add(v);
                    return v;
                },
                (key, v, element) => {
                    v.SetVisible(true);
                    v.Q<Label>("label").text = key;
                    v.Q<ObjectField>("target").value = element;
                },
                (key, v) => {
                    v.SetVisible(false);
                }
            );
        }
        
        
        
        // ========================================================================================
        // 操作
        // ========================================================================================
        
        void SetData(bool clear = false)
        {
            var target = x;
            SetData(target, null, clear);
            
        }
        
        static void SetData(UIBinding target, Transform root, bool clear = true)
        {
            EditorUtility.SetDirty(target);
            root = root ?? target.transform;
            
            if(clear) target.ClearBindings();
            
            if(root.TryGetComponent<UIContent>(out var content))
            {
                target[content.gameObject.name] = content;
            }
            
            root.transform.ForeachChild(sub => {
                if(sub.TryGetComponent<UIBinding>(out var nextLevel))
                {
                    SetData(nextLevel, null, true);
                }
                else
                {
                    SetData(target, sub, false);
                }
            });
        }
        
        
        
        
        [MenuItem("Component/ProtaFramework/UI/Refresh Window, _o")]
        static void RefreshComponent()
        {
            foreach(var g in Selection.objects)
            {
                if(g is GameObject gg) SetData(gg.GetOrCreate<UIBinding>(), null, true);
            }
        }
    }
}