using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

using Prota.Unity;
using UnityEditor.UIElements;
using System.Linq;

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
            
            root.AddChild(new ListView(d, -1, MakeItem, BindItem).PassValue(out var ll).SetMaxHeight(500));
            
            return root;
            
            VisualElement MakeItem()
            {
                return new ObjectField().SetNoInteraction();
            }
            
            void BindItem(VisualElement x, int i)
            {
                var g = x as ObjectField;
                var entry = d[i].Key;
                g.value = d[i].Value;
            }
        }
        
        protected override void Update()
        {
            
        }
    }
}
