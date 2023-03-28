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
            if(ShouldDraw(property))
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
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
            var t = target.GetType();
            var field = t.GetField(attr.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var prop = t.GetProperty(attr.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            bool shouldDraw = true;
            if (field != null)
            {
                var value = field.GetValue(target);
                if (value is bool b) shouldDraw = b;
                else if (value is object o) shouldDraw = o != null;
            }
            else if (prop != null)
            {
                var value = prop.GetValue(target);
                if (value is bool b) shouldDraw = b;
                else if (value is object o) shouldDraw = o != null;
            }

            return shouldDraw;
        }
    }
}
