using System;
using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.IO;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        [MenuItem("ProtaFramework/Tools/Move Scene Camera to Main Camera")]
        public static void MoveSceneCameraToMainCamera()
        {
            var scene = SceneView.lastActiveSceneView;
            var cam = Selection.activeObject switch {
                GameObject g => g.GetComponent<Camera>(),
                Camera c => c,
                _ => Camera.main
            };
            if(cam == null) return;
            
            // scene.camera.transform.position = cam.transform.position;
            // scene.camera.transform.rotation = cam.transform.rotation;
            
            scene.AlignViewToObject(cam.transform);
        }
    }
}
