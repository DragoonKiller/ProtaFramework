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
        
        public const GizmoType AllTypeOfGizmo = GizmoType.Active | GizmoType.Pickable | GizmoType.Selected | GizmoType.NonSelected | GizmoType.NotInSelectionHierarchy | GizmoType.InSelectionHierarchy;
        
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
        
        public static bool showSpriteRendererRange
        {
            get => EditorPrefs.GetBool("ProtaFramework.ShowSpriteRendererRange", true);
            set => EditorPrefs.SetBool("ProtaFramework.ShowSpriteRendererRange", value);
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
            showSpriteRendererRange = EditorGUILayout.ToggleLeft("Show Sprite Renderer Range", showSpriteRendererRange);
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
        
        
        [DrawGizmo(AllTypeOfGizmo)]
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
        
        [DrawGizmo(AllTypeOfGizmo)]
        static void DrawColliderRange(BoxCollider2D c, GizmoType type)
        {
            if(!showColliderRange) return;
            if(c == null) return;
            var color = c.attachedRigidbody == null ? darkRed : darkGreen;
            var min = c.bounds.min;
            var max = c.bounds.max;
            color.PushToGizmos();
            Gizmos.DrawLine(min, max.WithX(min.x));
            Gizmos.DrawLine(min, max.WithY(min.y));
            Gizmos.DrawLine(min.WithX(max.x), max);
            Gizmos.DrawLine(min.WithY(max.y), max);
            color.PopFromGizmos();
        }
        
        
        static readonly Color darkBlue = Color.blue - new Color(.3f, .3f, .4f, 0);
        
        
        [DrawGizmo(AllTypeOfGizmo)]
        static void DrawSpriteRenderer(SpriteRenderer c, GizmoType type)
        {
            if(!showSpriteRendererRange) return;
            if(c == null) return;
            var min = c.bounds.min;
            var max = c.bounds.max;
            var color = darkBlue.PushToGizmos();
            Gizmos.DrawLine(min, max.WithX(min.x));
            Gizmos.DrawLine(min, max.WithY(min.y));
            Gizmos.DrawLine(min.WithX(max.x), max);
            Gizmos.DrawLine(min.WithY(max.y), max);
            color.PopFromGizmos();
        }
        
        
        
    }
}