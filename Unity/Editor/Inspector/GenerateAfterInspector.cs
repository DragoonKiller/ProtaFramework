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
    [CustomEditor(typeof(GenerateAfter), false)]
    public class GenerateInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var o = serializedObject;
            var generateAfterEvent = o.FindProperty("generateAfterEvent");
            var generateMode = o.FindProperty("generateMode");
            var targetToGenerate = o.FindProperty("targetToGenerate");
            var referenceTransform = o.FindProperty("referenceTransform");
            var position = o.FindProperty("position");
            var localRotation = o.FindProperty("localRotation");
            var loop = o.FindProperty("loop");
            var delay = o.FindProperty("delay");
            var loopDelay = o.FindProperty("loopDelay");
            var g = o.FindProperty("g");
            var generateStarted = o.FindProperty("generateStarted");
            
            var generateAfterEventValue = (GenerateAfter.GenerateAfterEvent)generateAfterEvent.enumValueIndex;
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(targetToGenerate);
            EditorGUILayout.PropertyField(generateAfterEvent);
            EditorGUILayout.PropertyField(generateMode);
            EditorGUILayout.PropertyField(loop);
            EditorGUILayout.PropertyField(position);
            EditorGUILayout.PropertyField(localRotation);
            EditorGUILayout.FloatField("Delay", delay.floatValue);
            
            var modeValue = (GenerateAfter.GenerateMode)generateMode.enumValueIndex;
            if(modeValue == GenerateAfter.GenerateMode.LocalToSpecific)
            {
                EditorGUILayout.PropertyField(referenceTransform);
            }
            else if(modeValue == GenerateAfter.GenerateMode.LocalToThis)
            {
                
            }
            else if(modeValue == GenerateAfter.GenerateMode.World)
            {
                
            }
            else throw new System.Exception("Unknown GenerateMode: " + modeValue);
            
            var loopValue = loop.boolValue;
            if(loopValue)
            {
                EditorGUILayout.FloatField("Loop Delay", loopDelay.floatValue);
            }
            
            if(EditorGUI.EndChangeCheck())
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                o.ApplyModifiedProperties();
            }
            
            EditorGUILayout.LabelField("=== Runtime Info ===");
            GUI.enabled = false;
            EditorGUILayout.PropertyField(g);
            EditorGUILayout.PropertyField(generateStarted);
            GUI.enabled = true;
        }
    }
}
