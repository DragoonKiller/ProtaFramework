using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(CompactEditorAttribute))]
    public class CompactEditorDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.type == "Vector4")
            {
                Undo.RecordObject(property.serializedObject.targetObject, "Change Field");
                using var _ = new EditorGUILayout.HorizontalScope();
                var x = property.FindPropertyRelative("x");
                var y = property.FindPropertyRelative("y");
                var z = property.FindPropertyRelative("z");
                var w = property.FindPropertyRelative("w");
                EditorGUILayout.PropertyField(x, label);
                EditorGUILayout.PropertyField(y, GUIContent.none);
                EditorGUILayout.PropertyField(z, GUIContent.none);
                EditorGUILayout.PropertyField(w, GUIContent.none);
                property.serializedObject.ApplyModifiedProperties();
                return;
            }
        }
    }
}
