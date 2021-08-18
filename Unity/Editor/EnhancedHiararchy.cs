using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using Prota.CommonResources;

namespace Prota.Unity
{
    [InitializeOnLoad]
    public class EnhancedHiararchy
    {
        
        static EnhancedHiararchy()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHiararchyGUI;
        }

        private const int pixelPerDepth = 14;
        static readonly List<Component> comps = new List<Component>();
        
        static void OnHiararchyGUI(int instanceId, Rect area)
        {
            var originalGUIColor = GUI.color;
            
            var target = EditorUtility.InstanceIDToObject(instanceId);
            
            var rightMargin = area.xMax - 20;
            
            if(target is GameObject g)
            {
                var depth = g.transform.GetDepth();
                
                // SetActive 部分.
                var active = EditorGUI.Toggle(new Rect(rightMargin, area.yMax - area.height, 16, 16), g.activeSelf);
                rightMargin -= area.height;
                if(active != g.activeSelf) g.SetActive(active);
                
                g.GetComponents<Component>(comps);
                // comps.Sort(Compare);
                
                // Component 图标部分.
                foreach(var c in comps)
                {
                    if(c == null) continue; // 脚本丢失时, Component 为 null.
                    if(c.GetType() == typeof(UnityEngine.Transform)) continue;
                    var content = EditorGUIUtility.ObjectContent(c, c.GetType());
                    if(content.image != null)
                    {
                        GUI.Label(new Rect(rightMargin, area.yMax - area.height, 16, 16), new GUIContent(content.image));
                        rightMargin -= area.height;
                    }
                }
                
                // 空物体标记部分.
                if(comps.Count == 1 && comps[0].GetType() == typeof(Transform))
                {
                    var r = new Rect(area);
                    r.xMin -= 20 + depth * pixelPerDepth;
                    r.xMax = r.xMin + r.height;
                    GUI.color = new Color(1, 1, 1, 0.4f);
                    GUI.DrawTexture(r, ResourcesDatabase.inst["Common"]["line_vertical_16_2"] as Texture2D);
                    
                }
                
            }
            
            GUI.color = originalGUIColor;
        }
        
        static int Compare(Component a, Component b)
        {
            return a.GetType().Name.CompareTo(b.GetType().Name);
        }
    }
}