using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var labelAttribute = attribute as LabelAttribute;
            if(labelAttribute == null) return new PropertyField(property);
            return new PropertyField(property) { label = labelAttribute.name, tooltip = property.name };
        }
    }
}
