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
        
        Dictionary<long, VisualElement> runningList = new Dictionary<long, VisualElement>();
        
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
            countLabel.text = UnityEngine.Application.isPlaying ? mgr.idMap.Count.ToString() : "Not Running.";
            
            runningList.SetSync(mgr.idMap,
                (id, handle) => {
                    var x = new VisualElement()
                        .AddChild(new VisualElement() { name = "L1" }
                            .SetHorizontalLayout()
                            .AddChild(new Label() { name = "id" })
                            .AddChild(new EnumField() { name = "type" })
                        )
                        .AddChild(new VisualElement() { name = "L2" }
                            .SetHorizontalLayout()
                        )
                        .AddChild(new VisualElement().AsHorizontalSeperator(1));
                    running.Add(x);
                    return x;
                },
                (id, element, handle) => {
                    element.Q<Label>("id").text = handle.id.ToString();
                    element.Q<EnumField>("type").value = handle.type;
                },
                (id, element) => {
                    running.Remove(element);
                }
            );
        }
        
        
    }
    
    
    

}
