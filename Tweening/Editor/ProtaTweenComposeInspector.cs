using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Tween;
using System;
using Prota.Unity;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaTweenerCompose), false)]
    [ExecuteAlways]
    public class ProtaTweenComposeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("Update"))
            {
                var compose = target as ProtaTweenerCompose;
                if(compose != null) compose.AutoMatch();
            }
            base.OnInspectorGUI();
        }
    }
    
    
    [CustomPropertyDrawer(typeof(ProtaTweenerCompose.AnimEntry), false)]
    [ExecuteAlways]
    public class ProtaTweenComposeEntryInspector : PropertyDrawer
    {
        const float lineHeight = 20;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Func<string, SerializedProperty> prop = property.FindPropertyRelative;
            return (9 + prop("tweeners").arraySize) * lineHeight + 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Func<string, SerializedProperty> prop = property.FindPropertyRelative;
            using var _ = new EditorGUI.IndentLevelScope();
            var p = position.WithHeight(lineHeight);
            EditorGUI.PropertyField(p, prop("name"));
            p = p.MoveUp(lineHeight);
            EditorGUI.PropertyField(p, prop("matchName"));
            p = p.MoveUp(lineHeight);
            EditorGUI.PropertyField(p, prop("control"));
            p = p.MoveUp(lineHeight);
            EditorGUI.PropertyField(p, prop("running"));
            p = p.MoveUp(lineHeight);
            EditorGUI.PropertyField(p, prop("progress"));
            p = p.MoveUp(lineHeight);
            EditorGUI.PropertyField(p, prop("loop"));
            p = p.MoveUp(lineHeight);
            EditorGUI.PropertyField(p, prop("reversed"));
            p = p.MoveUp(lineHeight);
            EditorGUI.PropertyField(p, prop("reverseOnLoop"));
            p = p.MoveUp(lineHeight);
            foreach(var i in prop("tweeners").EnumerateAsList())
            {
                var target = i.objectReferenceValue as ProtaTweener;
                {
                    EditorGUI.TextField(p.WithWidth(140), target?.name);
                    EditorGUI.PropertyField(p.MarginIn(140, 0, 0, 0), i, GUIContent.none);
                }
                p = p.MoveUp(lineHeight);
            }
        }
    }
}
