using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

using Prota.Unity;
using Prota.Editor;
using UnityEditor.UIElements;
using System.Linq;
using NUnit.Framework;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaRectmeshGenerator), false)]
    [CanEditMultipleObjects]
    public class ProtaRectmeshGeneratorInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "ProtaRectmeshGenerator");
            
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
                enterChildren = false;
            }
            serializedObject.ApplyModifiedProperties();
            bool changed = EditorGUI.EndChangeCheck();
            
            if(GUILayout.Button("Update") || changed)
            {
                UpdateAll();
            }
        }
        
        void UpdateAll()
        {
            foreach(ProtaRectmeshGenerator t in targets)
            {
                t.needUpdateMesh = true;
            }
        }
        
    }
}
