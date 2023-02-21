using UnityEngine;
using UnityEditor;
using Prota.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Prota.Tween;
using System.Collections.Generic;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaTweenManager), false)]
    [ExecuteAlways]
    public class ProtaTweeningManagerInspector : UpdateInspector
    {
        VisualElement root;
        VisualElement running;
        
        Label countLabel;
        
        List<VisualElement> runningList = new List<VisualElement>();
        
        ProtaTweenManager mgr => target as ProtaTweenManager;
        
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
            
            runningList.SetEnumList<List<VisualElement>, ArrayLinkedList<TweenData>.IndexEnumerable, VisualElement, ArrayLinkedListKey>(mgr.data.EnumerateKey(),
                (i, handle) => {
                    VisualElement x = null;
                    bool detailVisible = false;
                    x = new VisualElement()
                        .AddChild(new VisualElement()
                            .SetHorizontalLayout()
                            .AddChild(new Button(() => {
                                    x.Q("detail").SetVisible(detailVisible = !detailVisible);
                                }) { text = "+" }
                            )
                            .AddChild(new ObjectField("") { name = "target" })
                            .AddChild(new Label() { name = "id" })
                            .AddChild(new Label() { name = "tid" })
                        )
                        .AddChild(new VisualElement() { name = "detail" }
                            .SetVisible(false)
                            .AddChild(new TextField("Current Ratio") { name = "curRatio" }.SetGrow().SetNoInteraction())
                            .AddChild(new TextField("Current Value") { name = "curValue" }.SetGrow().SetNoInteraction())
                            .AddChild(new TextField("Current Time") { name = "curTime" }.SetGrow().SetNoInteraction())
                            .AddChild(new Toggle("Valid") { name = "valid" }.SetGrow())
                            .AddChild(new VisualElement().SetHorizontalLayout()
                                .AddChild(new Label("Value").SetWidth(100))
                                .AddChild(new TextField() { name = "valueFrom" }.SetWidth(200))
                                .AddChild(new TextField() { name = "valueTo" }.SetWidth(200))
                            )
                            .AddChild(new VisualElement().SetHorizontalLayout()
                                .AddChild(new Label("Time").SetWidth(100))
                                .AddChild(new TextField() { name = "timeFrom" }.SetWidth(200))
                                .AddChild(new TextField() { name = "timeTo" }.SetWidth(200))
                            )
                            // .AddChild(new ObjectField("Guard") { name = "guard" })
                            .AddChild(new CurveField("Curve") { name = "curve" }.SetGrow())
                        )
                        .AddChild(new VisualElement().AsHorizontalSeperator(1))
                        ;
                    running.Add(x);
                    return x;
                },
                (i, element, key) => {
                    var data = mgr.data[key];
                    element.SetVisible(data.valid);
                    if(!data.valid) return;
                    element.Q<Label>("id").text = key.ToString();
                    element.Q<Label>("tid").text = data.tid.ToString();
                    element.Q<ObjectField>("target").value = data.target;
                    if(element.Q("detail").visible)
                    {
                        element.Q<TextField>("curRatio").value = data.EvaluateRatio(data.GetTimeLerp()).ToString();
                        element.Q<TextField>("curValue").value = data.Evaluate(data.GetTimeLerp()).ToString();
                        element.Q<TextField>("curTime").value = data.GetTimeLerp().ToString();
                        element.Q<TextField>("valueFrom").value = data.from.ToString();
                        element.Q<TextField>("valueTo").value = data.to.ToString();
                        element.Q<TextField>("timeFrom").value = data.timeFrom.ToString();
                        element.Q<TextField>("timeTo").value = data.timeTo.ToString();
                        element.Q<CurveField>("curve").value = data.curve;
                        // element.Q<ObjectField>("guard").value = data.guard;
                        element.Q<Toggle>("valid").value = data.valid;
                    }
                },
                (i, element) => {
                    element.SetVisible(false);
                }
            );
        }
        
        
    }
    
    
    

}
