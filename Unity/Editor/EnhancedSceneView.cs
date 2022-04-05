using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using Prota.Unity;

namespace Prota.Editor
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
        
        
        public static bool showColliderRange
        {
            get => EditorPrefs.GetBool("ProtaFramework.ShowColliderRange", true);
            set => EditorPrefs.SetBool("ProtaFramework.ShowColliderRange", value);
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
            showColliderRange = EditorGUILayout.ToggleLeft("Show Collider Range", showColliderRange);
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
        
        
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        static void DrawTransformConnnection(Transform t, GizmoType type)
        {
            if(!showTransformConnection) return;
            var p = t.parent;
            if(p == null) return;
            var from = p.position;
            var to = t.position;
            var c = Color.green.PushToGizmos();
            Gizmos.DrawLine(from, to);
            c.PopFromGizmos();
        }
        
        
        
        static readonly Color darkRed = Color.red - new Color(.3f, .3f, .3f, 0);
        static readonly Color darkGreen = Color.green - new Color(.3f, .3f, .2f, 0);
        
        [DrawGizmo(GizmoType.NonSelected)]
        static void DrawColliderRange(BoxCollider2D c, GizmoType type)
        {
            if(!showColliderRange) return;
            if(c == null) return;
            var color = c.attachedRigidbody == null ? darkRed : darkGreen;
            var min = c.bounds.min;
            var max = c.bounds.max;
            color.PushToGizmos();
            Gizmos.DrawLine(min, max.X(min.x));
            Gizmos.DrawLine(min, max.Y(min.y));
            Gizmos.DrawLine(min.X(max.x), max);
            Gizmos.DrawLine(min.Y(max.y), max);
            color.PopFromGizmos();
        }
        
        
        
        
    }
}