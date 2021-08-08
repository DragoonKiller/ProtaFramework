using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace Prota.Unity
{
    public class EnhancedSceneView : EditorWindow
    {
        
        // ============================================================================================================
        // 配置项
        // ============================================================================================================
        
        
        public static bool showTransformConnection
        {
            get => EditorPrefs.GetBool("ProtaFramework.ShowTransformConnection", true);
            set => EditorPrefs.SetBool("ProtaFramework.ShowTransformConnection", value);
        }
        
        
        
        
        
        // ============================================================================================================
        // 配置界面
        // ============================================================================================================
        
        
        [MenuItem("ProtaFramework/Scene/Settings %4")]
        static void ShowWindow()
        {
            var window = EditorWindow.GetWindow<EnhancedSceneView>();
            window.name = "Scene Settings";
            window.titleContent = new GUIContent("Scene Settings");
        }
        
        void OnGUI()
        {
            showTransformConnection = EditorGUILayout.ToggleLeft("Show Transform Connection", showTransformConnection);
            SceneView.lastActiveSceneView.Repaint();
        }
        
        
        // ============================================================================================================
        // 功能
        // ============================================================================================================
        
        static EnhancedSceneView()
        {
            EditorApplication.update += Update;
        }
        
        static void Update()
        {
            foreach(var camera in SceneView.GetAllSceneCameras())
            {
                camera.clearFlags = CameraClearFlags.Color;
                camera.backgroundColor = Color.black;
            }
        }
        
        
        static List<GameObject> rootGameObjectsCache = new List<GameObject>();
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawTransformConnnection(Transform t, GizmoType type)
        {
            if(!showTransformConnection) return;
            var p = t.parent;
            if(p == null) return;
            var from = p.position;
            var to = t.position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, to);
        }
        
    }
}