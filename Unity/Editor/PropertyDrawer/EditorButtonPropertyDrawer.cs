using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(EditorButton))]
    public class EditorButtonDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var button = new Button(() => property.boolValue = true);
            button.text = property.displayName;
            return button;
        }
    }
}
