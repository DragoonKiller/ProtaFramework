using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(NameAttribute))]
    public class LabelDrawer : PropertyDrawer
    {
        // public override VisualElement CreatePropertyGUI(SerializedProperty property)
        // {
        //     var labelAttribute = attribute as NameAttribute;
        //     if(labelAttribute == null) return new PropertyField(property);
        //     return new PropertyField(property) { label = labelAttribute.name, tooltip = property.name };
        // }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelAttribute = attribute as NameAttribute;
            base.OnGUI(position, property, new GUIContent(labelAttribute.name));
        }
    }
}
