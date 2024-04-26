using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

using Prota.Unity;
using UnityEditor.UIElements;
using System.Linq;
using System.Dynamic;
using NUnit.Framework.Internal;

namespace Prota.Editor
{
    [CustomEditor(typeof(ResourceList), false)]
    public class ResourceListInspector : UpdateInspector
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var list = serializedObject.targetObject as ResourceList;
            var data = list.resources;
            var d = data.ToList();
            
            var ignoreSubAsset = new PropertyField(serializedObject.FindProperty("ignoreSubAsset"));
            root.AddChild(ignoreSubAsset);
            var ignoreDuplicateAsset = new PropertyField(serializedObject.FindProperty("ignoreDuplicateAsset"));
            root.AddChild(ignoreDuplicateAsset);
            var regexMatcher = new PropertyField(serializedObject.FindProperty("regexMatcher"));
            root.AddChild(regexMatcher);
            var subFormat = new PropertyField(serializedObject.FindProperty("subAssetNamingFormat"));
            
            root.AddChild(subFormat);
            ignoreSubAsset.RegisterValueChangeCallback(e => {
                subFormat.SetVisible(!e.changedProperty.boolValue);
                ResourceListUpdater.UpdateResourceList(list);
            });
            ignoreDuplicateAsset.RegisterValueChangeCallback(e => {
                ResourceListUpdater.UpdateResourceList(list);
            });
            subFormat.RegisterValueChangeCallback(e => {
                ResourceListUpdater.UpdateResourceList(list);
            });
            subFormat.SetVisible(!serializedObject.FindProperty("ignoreSubAsset").boolValue);
            
            
            root.AddChild(new Button(() => {
                ResourceListUpdater.UpdateResourceList(list);
            }){ text = "refresh" });
            
            root.AddChild(new ListView(d, -1, MakeItem, BindItem).PassValue(out var ll).SetMaxHeight(700));
            
            return root;
            
            VisualElement MakeItem()
            {
                return new VisualElement()
                    .SetHorizontalLayout()
                    .SetGrow()
                    .AddChild(new TextField(){ name = "text" }.SetMaxWidth(200).SetGrow())
                    .AddChild(new ObjectField(){ name = "obj" }.SetNoInteraction().SetGrow());
            }
            
            void BindItem(VisualElement x, int i)
            {
                var text = x.Q<TextField>("text");
                var obj = x.Q<ObjectField>("obj");
                text.textEdition.isReadOnly = true;
                
                text.value = d[i].Key;
                obj.value = d[i].Value;
            }
        }
        
        protected override void Update()
        {
            
        }
    }
}
