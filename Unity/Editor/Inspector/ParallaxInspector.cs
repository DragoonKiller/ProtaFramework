using UnityEngine;
using UnityEditor;
using Prota.Unity;

namespace Prota.Editor
{
    [CustomEditor(typeof(Parallax), false)]
    [CanEditMultipleObjects]
    public class ParallaxInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if(ParallaxCamera.instance == null)
            {
                EditorGUILayout.HelpBox("No [ParallaxCamera] found in scene.", MessageType.Error);
                return;
            }
            
            var cameraDistance = -ParallaxCamera.instance.transform.position.z;
            
            if(cameraDistance <= 0)
            {
                EditorGUILayout.HelpBox($"Camera distance must be positive. now is [{ cameraDistance }]", MessageType.Error);
                return;
            }
            
            var distance = serializedObject.FindProperty("distance");
            EditorGUILayout.PropertyField(distance);
            
            Undo.RecordObject(target, "ParallaxComponent");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
