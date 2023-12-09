using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

using Prota.Unity;
using UnityEditor.UIElements;
using System.Linq;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaRes), false)]
    public class ProtaResInspector : UpdateInspector
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var list = serializedObject.targetObject as ProtaRes;
            var data = list.lists;
            var d = data.ToList();
            
            // d.Select(x => $"{x.Key} :: {x.Value}").ToStringJoined().LogError();
            
            root.AddChild(new Button(() => {
                ResourceListUpdater.RefreshAllResourceList();
            }) { text = "Update All" });
            
            root.AddChild(new ListView(d, -1, MakeItem, BindItem).PassValue(out var ll).SetMaxHeight(500));
            
            return root;
            
            VisualElement MakeItem()
            {
                return new VisualElement()
                    .SetHorizontalLayout()
                    .SetGrow()
                    .AddChild(new Label() { name = "label" }.SetWidth(100))
                    .AddChild(new ObjectField() { name = "obj" }.SetNoInteraction().SetGrow());
            }
            
            void BindItem(VisualElement x, int i)
            {
                // Debug.LogError(d[i].Key + " " + d[i].Value);
                var g = x.Q<ObjectField>("obj");
                var l = x.Q<Label>("label");
                l.text = d[i].Key;
                g.value = d[i].Value;
            }
        }
        
        protected override void Update()
        {
            var binding = serializedObject.targetObject as DataBinding;
            
            
        }
    }
}
