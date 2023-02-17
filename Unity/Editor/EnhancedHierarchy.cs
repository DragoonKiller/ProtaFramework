using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota.Editor
{
    [InitializeOnLoad]
    public class EnhancedHierarchy : UnityEditor.Editor
    {
        static EnhancedHierarchy()
        {
            UpdateSettings();
        }
        
        static bool registered
        {
            get => EditorPrefs.GetBool("Prota:EnhancedHierarchyEnabled", true);
            set => EditorPrefs.SetBool("Prota:EnhancedHierarchyEnabled", value);
        }
        
        [MenuItem("ProtaFramework/Editor/Toggle Enhanced Hierarchy")]
        static void SwitchEnhancedHierarchy()
        {
            registered = !registered;
            UpdateSettings();
        }
        
        [MenuItem("ProtaFramework/Editor/GC Unused Assets")]
        static void GCUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
        
        
        static void UpdateSettings()
        {
            if(!registered) 
            {
                UnityEngine.Debug.Log($"Enhanced Hierarchy disabled.");
                EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
            }
            else
            {
                UnityEngine.Debug.Log($"Enhanced Hierarchy enabled.");
                EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
            }
        }
        
        private const int pixelPerDepth = 14;
        static readonly List<Component> comps = new List<Component>();
        static readonly List<GameObject> parents = new List<GameObject>();
        
        static Dictionary<Type, GUIContent> thumbnailMap = new Dictionary<Type, GUIContent>();
        
        static Texture2D barTexture = null;
        
        const int maxIconCount = 10;
        
        static void OnHierarchyGUI(int instanceId, Rect area)
        {
            if(barTexture == null) barTexture = Resources.Load<Texture2D>("ProtaFramework/line_vertical_16_2");
            
            var originalGUIColor = GUI.color;
            
            var target = EditorUtility.InstanceIDToObject(instanceId);
            
            var rightMargin = area.xMax - 40;
            var space = area.height - 4;                // 每个图标向左偏移多少像素.
            
            if(target is GameObject g)
            {
                var depth = g.transform.GetDepth();
                
                // SetActive 部分.
                var active = EditorGUI.Toggle(new Rect(rightMargin, area.yMax - area.height, 16, 16), g.activeSelf);
                rightMargin -= space + 2;
                if(active != g.activeSelf)
                {
                    Undo.RecordObject(g, "Activation");
                    g.SetActive(active);
                    Selection.activeObject = g;
                }
                
                // 这个 gameobject 下属的 gameobject.
                g.GetComponents<Component>(comps);
                // comps.Sort(Compare);
                comps.Reverse();
                
                // Component 图标部分.
                int n = 0;
                foreach(var c in comps)
                {
                    if(c == null) continue; // 脚本丢失时, Component 为 null.
                    
                    n++;
                    if(n > maxIconCount) break;
                    
                    var ctype = c.GetType();
                    if(ctype == typeof(UnityEngine.Transform)) continue;
                    
                    if(!thumbnailMap.TryGetValue(ctype, out var content))
                    {
                        var cc = EditorGUIUtility.ObjectContent(c, ctype);
                        var image = cc.image;
                        if(image == null) continue;     // 没有图标.
                        thumbnailMap[ctype] = content = new GUIContent(image);
                    }
                    
                    GUI.Label(new Rect(rightMargin, area.yMax - area.height, 16, 16), content);
                    rightMargin -= space;
                }
                
                // Canvas 标记部分. 如果一个物体被挂在 canvas 下方则有一个蓝色竖线标记.
                if(g.GetComponentInParent<Canvas>() != null)
                {
                    var r = new Rect(area);
                    r.xMin -= 20 + depth * pixelPerDepth;
                    r.xMax = r.xMin + r.height;
                    GUI.color = new Color(0.6f, 0.65f, 1, 1f);
                    GUI.DrawTexture(r, barTexture);
                }
                
                // 空物体标记部分.
                // if(comps.Count == 1 && comps[0].GetType() == typeof(Transform))
                // {
                //     var r = new Rect(area);
                //     r.xMin -= 22 + depth * pixelPerDepth;
                //     r.xMax = r.xMin + r.height;
                //     GUI.color = new Color(0.4f, 0.4f, 0.4f, 1f);
                //     GUI.DrawTexture(r, barTexture);
                // }
                
            }
            
            GUI.color = originalGUIColor;
        }
        
        static int Compare(Component a, Component b)
        {
            return a.GetType().Name.CompareTo(b.GetType().Name);
        }
    }
}