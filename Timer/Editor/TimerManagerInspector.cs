using UnityEngine;
using UnityEditor;
using Prota.Unity;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Prota.Timer
{
    [CustomEditor(typeof(TimerManager), false)]
    public class TimerManagerInspector : UpdateInspector
    {
        Label normalCur;
        ScrollView normalList;
        Label normalCount;
        Label realtimeCur;
        ScrollView realtimeList;
        Label realtimeCount;
        
        public Dictionary<TimeKey, VisualElement> normalLoaded = new Dictionary<TimeKey, VisualElement>();
        
        public Dictionary<TimeKey, VisualElement> realtimeLoaded = new Dictionary<TimeKey, VisualElement>();
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement()
                .AddChild(normalCur = new Label())
                .AddChild(normalCount = new Label())
                .AddChild(normalList = new ScrollView(ScrollViewMode.Vertical)
                    .SetMaxHeight(600)
                )
                .AddChild(new VisualElement().AsHorizontalSeperator(3))
                .AddChild(realtimeCur = new Label())
                .AddChild(realtimeCount = new Label())
                .AddChild(realtimeList = new ScrollView(ScrollViewMode.Vertical)
                    .SetMaxHeight(600)
                );
            return root;
        }
        
        void Update(Dictionary<TimeKey, VisualElement> loaded, TimerQueue q, ScrollView scroll, Label count)
        {
            count.text = q.timers.Count.ToString();
            loaded.SetSync(q.timers, (k, t) => {
                return new VisualElement()
                    .SetParent(scroll)
                    .SetHorizontalLayout()
                    .AddChild(new Label() { name = "name" }
                        .SetWidth(100)
                    )
                    .AddChild(new TextField() { name = "type", isReadOnly = true }
                        .SetMaxWidth(60)
                        .SetGrow()
                    )
                    .AddChild(new TextField() { name = "time", isReadOnly = true }
                        .SetGrow()
                    )
                ;
            }, (k, v, t) => {
                v.SetVisible(true);
                v.Q<Label>("name").text = t.name;
                v.Q<TextField>("type").value = t.repeat ? "repeat" : "normal";
                v.Q<TextField>("time").value = k.time.ToString("0.000000");
            }, (k, v) => {
                v.SetVisible(false);
            });
        }
        
        protected override void Update()
        {
            realtimeCur.text = $"realtime: { Time.realtimeSinceStartup.ToString("0.000000") }";
            normalCur.text = $"time: { Time.time.ToString("0.000000") }";
            Update(normalLoaded, Timer.normalTimer, normalList, normalCount);
            Update(realtimeLoaded, Timer.realtimeTimer, realtimeList, realtimeCount);
        }
    }
}