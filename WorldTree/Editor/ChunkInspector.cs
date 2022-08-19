using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.UIElements;
using Prota.Unity;
using Prota.WorldTree;

namespace Prota.Editor
{
    [CustomEditor(typeof(Chunk), false)]
    public class MaterialHandlerInspector : UpdateInspector
    {
        Chunk chunk => target as Chunk;
        
        VisualElement root;
        
        Toggle initedView;
        
        public static bool showGizmos = true;
        
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            root.AddChild(initedView = new Toggle("inited"));
            root.AddChild(new IntegerField("max depth") { value = chunk.maxDepth }
                .OnValueChange<IntegerField, int>(e => {
                    chunk.Reset();
                    chunk.maxDepth = e.newValue;
                    chunk.UpdateAll();
                })
            );
            root.AddChild(new FloatField("half size") { value = chunk.rootHalfSize }
                .OnValueChange<FloatField, float>(e => {
                    chunk.Reset();
                    chunk.rootHalfSize = e.newValue;
                    chunk.UpdateAll();
                })
            );
            root.AddChild(new Toggle("multithread update") { value = chunk.multithreadUpdate }
                .OnValueChange<Toggle, bool>(e => {
                    chunk.multithreadUpdate = e.newValue;
                })
            );
            root.AddChild(new IntegerField("frame per tick") { value = chunk.framePerTick }
                .OnValueChange<IntegerField, int>(e => {
                    chunk.framePerTick = e.newValue;
                })
            );
            root.AddChild(new IntegerField("update per tick") { value = chunk.updatePerTickLimit }
                .OnValueChange<IntegerField, int>(e => {
                    chunk.updatePerTickLimit = e.newValue;
                })
            );
            root.AddChild(new Button(() => chunk?.Reset()) { text = "reset" });
            
            // ====================================================================================================
            // ====================================================================================================
            
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            root.AddChild(new PropertyField(serializedObject.FindProperty("transformTargetList")));
            root.AddChild(new PropertyField(serializedObject.FindProperty("pointTargetList")));
            
            // ====================================================================================================
            // ====================================================================================================
            
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            root.AddChild(new Toggle("Show gizmos").OnValueChange<Toggle, bool>(e => showGizmos = e.newValue));
            
            return root;
        }

        protected override void Update()
        {
            if(chunk == null) return;
            if(root == null) return;
            initedView.value = chunk.inited;
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
    
        static Color[] colorArray = new Color[] {
            Color.red,
            Color.yellow,
            Color.green,
            Color.blue,
            Color.cyan,
        };

        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        static void DrawColliderRange(Chunk chunk, GizmoType type)
        {
            if(!showGizmos) return;
            if(chunk.activateList == null) return;
            for(int i = 0; i < chunk.activateList.Length; i++)
            {
                using(var g = new GizmosContext())
                {
                    Gizmos.color = colorArray[i % colorArray.Length];
                    foreach(var node in chunk.activateList[i])
                    {
                        var rect = node.rect;
                        Gizmos.DrawLine(rect.BottomLeft(), rect.BottomRight());
                        Gizmos.DrawLine(rect.BottomLeft(), rect.TopLeft());
                        Gizmos.DrawLine(rect.BottomRight(), rect.TopRight());
                        Gizmos.DrawLine(rect.TopLeft(), rect.TopRight());
                    }
                }
            }
        }
    
    }
    
    
    
    
}