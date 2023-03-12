using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

namespace Prota.Editor
{
    // https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
    // https://docs.unity3d.com/ScriptReference/PropertyDrawer.html <<== Unity 2022.2+ 才默认 UIElement 为主.
    [CustomPropertyDrawer(typeof(Readonly))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isReadonly = false;
            isReadonly |= fieldInfo.GetCustomAttribute<Readonly>().whenPlaying && Application.isPlaying;
            isReadonly |= fieldInfo.GetCustomAttribute<Readonly>().whenEditing && !Application.isPlaying;
            if(isReadonly)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
    }
}
