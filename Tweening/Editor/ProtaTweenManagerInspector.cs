using UnityEngine;
using UnityEditor;
using Prota.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Prota.Tweening;
using System.Collections.Generic;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaTweeningManager), false)]
    [ExecuteAlways]
    public class ProtaTweeningManagerInspector : UpdateInspector
    {
        VisualElement root;
        VisualElement running;
        
        Label countLabel;
        
        List<VisualElement> runningList = new List<VisualElement>();
        
        ProtaTweeningManager mgr => target as ProtaTweeningManager;
        
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            root.AddChild(new VisualElement()
                .SetHorizontalLayout()
                .AddChild(new Label() { text = "total: " })
                .AddChild(countLabel = new Label() { })
            );
            root.Add(running = new ScrollView());
            
            return root;
        }
        
        protected override void Update()
        {
            countLabel.text = UnityEngine.Application.isPlaying ? mgr.data.Count.ToString() : "Not Running.";
            
            runningList.SetEnumList<List<VisualElement>, ArrayLinkedList<TweenData>, VisualElement, TweenData>(mgr.data,
                (id, handle) => {
                    var x = new VisualElement()
                        .AddChild(new VisualElement() { name = "L1" }
                            .SetHorizontalLayout()
                            .AddChild(new Label() { name = "id" })
                            .AddChild(new EnumField(TweeningType.Custom) { name = "type" })
                        )
                        .AddChild(new VisualElement() { name = "L2" }
                            .SetHorizontalLayout()
                        )
                        .AddChild(new VisualElement().AsHorizontalSeperator(1));
                    running.Add(x);
                    return x;
                },
                (id, element, data) => {
                    element.SetVisible(data.valid);
                    if(!data.valid) return;
                    element.Q<Label>("id").text = data.ToString();
                    element.Q<EnumField>().value = data.type;
                },
                (id, element) => {
                    element.SetVisible(false);
                }
            );
        }
        
        
    }
    
    
    

}
