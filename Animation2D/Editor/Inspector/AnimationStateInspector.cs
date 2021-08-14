using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Prota.Animation
{
    /// <summary>
    /// Track 的基础类型.
    /// </summary>
    [CustomEditor(typeof(ProtaAnimationState))]
    [CanEditMultipleObjects]
    public sealed class ProtaAnimationStateInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var asset = serializedObject.FindProperty("_asset");
            EditorGUILayout.PropertyField(asset);
            EditorGUILayout.LabelField("当前状态", EditorStyles.boldLabel);
            var time = serializedObject.FindProperty("_time");
            EditorGUILayout.PropertyField(time);
            if(GUILayout.Button("打开编辑器")) EditorWindow.GetWindow<AnimationEditorWindow>().Set(serializedObject.targetObject as ProtaAnimationState);
        }
    }
}