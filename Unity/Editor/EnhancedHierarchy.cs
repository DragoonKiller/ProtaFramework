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
        
        static Texture2D barTexture = null;
        static Texture2D backTexture = null;
        
        const int maxIconCount = 10;
        
        static void OnHierarchyGUI(int instanceId, Rect area)
        {
            if(barTexture == null) barTexture = Resources.Load<Texture2D>("ProtaFramework/line_vertical_16_2");
            if(backTexture == null) backTexture = Resources.Load<Texture2D>("ProtaFramework/rect_16");
            
            var originalGUIColor = GUI.color;
            
            var target = EditorUtility.InstanceIDToObject(instanceId);
            
            var rightMargin = area.xMax - 40;
            var space = area.height - 4;                // 每个图标向左偏移多少像素.
            
            if(!(target is GameObject g)) return;
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
            comps.RemoveAll(x => x == null);
            
            // Component 图标部分.
            int n = 0;
            foreach(var c in comps)
            {
                n++;
                if(n > maxIconCount) break;
                
                var ctype = c.GetType();
                if(ctype == typeof(UnityEngine.Transform)) continue;
                GUI.Label(new Rect(rightMargin, area.yMax - area.height, 16, 16), new GUIContent(c.FindEditorIcon()));
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
            
            // 全局单例标记部分, 命名带 # 号, 或者组件带有 Singleton.
            {
                bool hasSingleton = false;
                foreach(var c in comps)
                {
                    var t = c.GetType();
                    if(!t.IsGenericType) continue;
                    t = t.GetGenericTypeDefinition();
                    if(t != typeof(Singleton<>)) continue;
                    hasSingleton = true;
                    break;
                }
                if(g.name.StartsWith("#") || hasSingleton)
                {
                    var r = new Rect(area);
                    r.xMin -= 22 + depth * pixelPerDepth;
                    r.xMax = r.xMin + r.height;
                    GUI.color = new Color(0.6f, 0.0f, 0.0f, 1f);
                    GUI.DrawTexture(r, barTexture);
                }
            }
                
            // 标记名称前缀为 >>> 或 ===, 后缀为 <<< 或 === 的物体.
            if((g.name.StartsWith(">>>") || g.name.StartsWith("===") || g.name.StartsWith("---"))
            && (g.name.EndsWith("<<<") || g.name.EndsWith("===") || g.name.EndsWith("---")))
            {
                var r = new Rect(area);
                // r.xMin -= depth * pixelPerDepth;
                r.xMin += 20;
                var oriMax = r.yMax;
                r.yMax = r.yMin + 1;
                
                GUI.color = new Color(0.6f, 0.6f, 0.6f, 1f);
                GUI.DrawTexture(r, backTexture);
                
                // GUI.color = new Color(0.4f, 0.4f, 0.4f, 1f);
                // r.xMin += depth * pixelPerDepth;
                // r.xMax -= r.width * 0.5f;
                // r.yMax = oriMax;
                // r.yMin = r.yMax - 1;
                // GUI.DrawTexture(r, backTexture);
            }
            
            // 标记当前选择的 GameObject 的 DataBinding.
            if(Selection.activeGameObject != null
                && Selection.activeGameObject.TryGetComponent<DataBinding>(out var dataBinding))
            {
                var list = (List<DataBinding.Entry>)dataBinding.ProtaReflection().Get("data");
                if(list.Any(x => x.target == g))
                {
                    var r = new Rect(area);
                    r.xMin = r.xMax;
                    r.xMax = r.xMin + 2;
                    GUI.color = new Color(1f, 0.6f, 0.6f, 1f);
                    GUI.DrawTexture(r, backTexture);
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
