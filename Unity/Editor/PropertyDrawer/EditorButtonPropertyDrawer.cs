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
        // public override VisualElement CreatePropertyGUI(SerializedProperty property)
        // {
        //     var button = new Button(() => {
        //         property.boolValue = property.boolValue;
        //         property.serializedObject.ApplyModifiedProperties();
        //     });
        //     button.text = property.displayName;
        //     return button;
        // }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(GUILayout.Button(label))
            {
                property.boolValue = true;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
