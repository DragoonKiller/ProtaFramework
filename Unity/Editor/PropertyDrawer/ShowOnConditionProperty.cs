using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(ShowWhenAttribute))]
    public class ShowWhenDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = fieldInfo.GetCustomAttribute<Readonly>();
            if(ShouldDraw(property)) return EditorGUI.GetPropertyHeight(property, label, true);
            return 0;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool shouldDraw = ShouldDraw(property);
            if (shouldDraw) EditorGUI.PropertyField(position, property, label, true);
        }

        bool ShouldDraw(SerializedProperty property)
        {
            var attr = fieldInfo.GetCustomAttribute<ShowWhenAttribute>();
            var target = property.serializedObject.targetObject;    // 这个 object 是一个 Component.
            var refTarget = target.ProtaReflection();
            bool hasValue = refTarget.TryGet(attr.name, out object value);
            if(hasValue && value is bool b && b) return true;
            if(hasValue && value is object o && o != null) return true;
            bool hasMethod = refTarget.type.HasMethod(attr.name);
            if(hasMethod && refTarget.Call(attr.name).PassValue(out var st) != null && st is bool k && k) return true;
            return false;
        }
    }
}
