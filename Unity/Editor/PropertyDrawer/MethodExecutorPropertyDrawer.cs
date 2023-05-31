using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(MethodExecutor))]
    public class MethodExecutorDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var t = fieldInfo.DeclaringType.ProtaReflection();
            var res = new VisualElement();
            foreach(var f in t.allMethods)
            {
                if(f.GetCustomAttribute<MethodExecutor>().PassValue(out var attr) == null) continue;
                var button = new Button(() => f.Invoke(property.serializedObject.targetObject, null));
                button.text = f.Name;
                res.Add(button);
            }
            return res;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;
    }
}
