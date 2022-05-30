using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Prota.Input;

namespace Prota.Editor
{
    [CustomEditor(typeof(GameInput), false)]
    [ExecuteAlways]
    public class GameInputInspector : UnityEditor.Editor
    {
        GameInput script => target as GameInput;
        
        class ActionRecord : VisualElement
        {
            public string actionName;
            public readonly List<VisualElement> callbacks = new List<VisualElement>();
        }
        
        VisualElement root;
        
        VisualElement contentRoot;
        
        readonly Dictionary<string, ActionRecord> actions = new Dictionary<string, ActionRecord>();
        
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement()
                .AddChild(contentRoot = new ScrollView(ScrollViewMode.Vertical)
                    .SetMaxHeight(600)
                    .SetGrow()
                )
            ;
            
            return root;
        }
        
        void OnEnable()
        {
            EditorApplication.update += Update;
        }
        
        void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        
        void Update()
        {
            if(!Application.isPlaying)
            {
                actions.Clear();
                return;
            }
            
            if(Selection.objects.Count() != 1) return;
            if(this.root == null) return;
            
            actions.SetSync(GameInput.callbacks, (k, v) => {
                var action = new ActionRecord() { actionName = k }
                    .SetGrow()
                    .AddChild(new Label() { name = "title", text = k })
                    .AddChild(new VisualElement() { name = "sub" })
                ;
                action.actionName = k;
                contentRoot.AddChild(action);
                return action;
                
            }, (k, g, v) => {
                g.callbacks.SetLength(v.Count, i =>
                {
                    var g = new VisualElement()
                        .SetHorizontalLayout()
                        .SetGrow()
                        .AddChild(new VisualElement().AsVerticalSeperator(22, new Color(0f, 0f, 0f, 0f)))
                        .AddChild(new Label() { name = "info" });
                    g.SetParent(actions[k].Q("sub"));
                    return g;
                    
                }, (i, g) => {
                    g.SetVisible(true);
                    
                    string content = string.Intern("Unknown");
                    var tt = v[i].Target;
                    if(tt is UnityEngine.Object gg)
                    {
                        content = gg.ToString() + " : " + gg.GetInstanceID() + " " + v[i].ToString();
                    }
                    else
                    {
                        content = v[i].ToString();
                    }
                    
                    g.Q<Label>("info").text = content;
                    
                }, (i, g) => {
                    g.SetVisible(false);
                });
                
            }, (k, v) => {
                contentRoot.Remove(actions[k]);
                actions.Remove(k);
            });
        }
        
        
        
    }
    
}