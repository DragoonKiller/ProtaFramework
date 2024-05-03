using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor.SearchService;

namespace Prota.Editor
{
    [CustomEditor(typeof(OverworldSceneInfo), false)]
    public class OverworldSceneInfoInspector : UnityEditor.Editor
    {
        public OverworldSceneInfo info => target as OverworldSceneInfo;
        
        
        static string newSceneName;
        
        static bool drawAdjacents;
        
        static bool showScenesInView;
        
        static bool activated;
        static bool isDragging;
        static Vector2? dragFrom;
        static Vector2? dragTo;
        
        // ====================================================================================================
        // ====================================================================================================
        
        void CreateNewScecne()
        {
            if(dragFrom == null || dragTo == null)
            {
                Debug.LogWarning("DragFrom or DragTo is null.");
                return;
            }
            
            if(newSceneName.NullOrEmpty())
            {
                Debug.LogWarning("New scene name is null or empty.");
                return;
            }
            
            if(info == null)
            {
                Debug.LogWarning("info is null.");
                return;
            }
            
            var sceneResourcePath = info.scenePath.PathCombine(newSceneName).ToStandardPath();
            var assetPath = "Resources".PathCombine(sceneResourcePath).ToStandardPath();
            if(("Assets/" + assetPath  + ".unity").AsFileInfo().Exists)
            {
                Debug.LogWarning($"Scene file [{ newSceneName }] already exists.");
                if(info.entries.Any(x => x.name == newSceneName))
                {
                    Debug.LogWarning($"Scene [{ newSceneName }] already exists in OverworldScenesInfo.");
                }
                else
                {
                    Debug.LogWarning($"Add exist scene [{ newSceneName }] to OverworldScenesInfo.");
                    Debug.LogWarning($"You may need to set range manually.");
                }
            }
            else
            {
                var setup = NewSceneSetup.EmptyScene;
                var s = EditorSceneManager.NewScene(setup, NewSceneMode.Additive);
                EditorSceneManager.SaveScene(s, "Assets/" + assetPath + ".unity");
            }
            
            Undo.RecordObject(info, "OverworldScenesInfo");
            
            var entry = new SceneEntry();
            entry.name = newSceneName;
            entry.range = new Rect(dragFrom.Value, dragTo.Value - dragFrom.Value);
            entry.adjacentScenes = Array.Empty<int>();
            
            if(info.entries.FindIndex(x => x.name == newSceneName).PassValue(out var index) == -1)
            {
                info.entries = info.entries.Resize(info.entries.Length + 1);
                index = info.entries.Length - 1;
            }
            
            info.entries[index] = entry;
            ComputeAdjacents(info.entries);
            
            AssetDatabase.SaveAssetIfDirty(info);
            
            EditorUtility.SetDirty(info);
            AssetDatabase.SaveAssets();
            
            AssetDatabase.Refresh();
        }
        
        void ComputeAdjacents(SceneEntry[] entries)
        {
            using var _ = TempDict.Get<SceneEntry, int>(out var reverseMap);
            for(int i = 0; i < entries.Length; i++)
            {
                reverseMap[entries[i]] = i;
            }
            Parallel.ForEach(entries, (x, _, i) => {
                x.adjacentScenes = entries
                    .Where((x, j) => i != j)
                    .Where(y => x.range.Overlaps(y.range))
                    .Select(x => reverseMap[x])
                    .ToArray();
            });
        }
        
        void Select()
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var p = ray.HitXYPlane();
            var entry = info.entries.FirstOrDefault(x => x.range.ContainsInclusive(p));
            if(entry == null) return;
            dragFrom = entry.range.position;
            dragTo = entry.range.position + entry.range.size;
            newSceneName = entry.name;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnEnable()
        {
            SceneView.duringSceneGui += SOOnSceneGUI;
        }
        
        void OnDisable()
        {
            SceneView.duringSceneGui -= SOOnSceneGUI;
        }
        

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "OverworldSceneInfo");
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.LabelField(">>> New Scene <<<");
            newSceneName = EditorGUILayout.TextField("Name", newSceneName);
            
            var buttonName = info.entries.Any(x => x.name == newSceneName) ? "Change" : "Create";
            if(GUILayout.Button(buttonName)) CreateNewScecne();
            
            EditorGUILayout.LabelField(">>> Editor <<<");
            
            if(GUILayout.Button("Focus"))
            {
                Focus(SceneView.lastActiveSceneView);
            }
            
            if(GUILayout.Button("ShowAll"))
            {
                ShowAll(SceneView.lastActiveSceneView);
            }
            
            if(GUILayout.Button("Load Scenes By View"))
            {
                LoadSceneByView(SceneView.lastActiveSceneView);
            }
            
            showScenesInView = EditorGUILayout.Toggle("Show Scenes In View", showScenesInView);
            
            drawAdjacents = EditorGUILayout.Toggle("Draw Adjacents", drawAdjacents);
            
            using(new EditorGUI.DisabledScope(true))
            {
                activated = EditorGUILayout.Toggle("Activated", activated);
                EditorGUILayout.Toggle("IsDragging", isDragging);
                if(dragFrom.HasValue) EditorGUILayout.Vector2Field("DragFrom", dragFrom.Value);
                if(dragTo.HasValue) EditorGUILayout.Vector2Field("DragTo", dragTo.Value);            
            }
            
            if(showScenesInView)
            {
                ShowScenesInViewInInspector(SceneView.lastActiveSceneView.camera);
            }
            
            EditorGUILayout.LabelField(">>> OverworldSceneInfo <<<");
            base.OnInspectorGUI();
            
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssetIfDirty(target);
            }
            serializedObject.UpdateIfRequiredOrScript();
            
            Repaint();
        }
        
        void SOOnSceneGUI(SceneView v)
        {
            switch(Event.current.type)
            {
                case EventType.MouseDown:
                {
                    if(Event.current.button == 0)
                    {
                        Event.current.Use();
                        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        dragTo = dragFrom = ray.HitXYPlane();
                        isDragging = true;
                    }
                    else
                    {
                        dragFrom = null;
                        dragTo = null;
                        isDragging = false;
                    }
                    break;
                }
                
                case EventType.MouseDrag:
                {
                    Event.current.Use();
                    var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if(isDragging) dragTo = ray.HitXYPlane();
                    break;
                }
                
                case EventType.MouseUp:
                {
                    if(Event.current.button != 0) break;
                    Event.current.Use();
                    isDragging = false;
                    SwapDrag();
                    if(dragFrom.Value == dragTo.Value)
                    {
                        // 双击选择.
                        Select();
                    }
                    break;
                }
                
                case EventType.Repaint:
                {
                    if(dragFrom.HasValue && dragTo.HasValue)
                    {
                        var rect = new Rect(dragFrom.Value, dragTo.Value - dragFrom.Value);
                        Handles.DrawSolidRectangleWithOutline(
                            rect,
                            Color.blue.WithA(0.3f),
                            Color.blue.WithA(0.8f)
                        );
                    }
                    
                    foreach(var s in info.entries)
                    {
                        Handles.DrawSolidRectangleWithOutline(
                            s.range,
                            Color.green.WithA(0.0f),
                            Color.red.WithA(1f)
                        );
                    }
                    
                    foreach(var s in info.entries)
                    {
                        Handles.Label(s.range.TopLeft(), "  " + s.name, new GUIStyle() { fontSize = 14 });
                    }
                    
                    if(drawAdjacents)
                    {
                        foreach(var s in info.entries)
                        {
                            foreach(var a in s.GetAdjacent(info.entries))
                            {
                                Handles.DrawLine(s.range.center, a.range.center);
                            }
                        }
                    }
                    
                    break;
                }
                
                default: break;
            }
            
            ShowSceneViewText(v);
            
            Repaint();
        }
        
        void Focus(SceneView view)
        {
            if(dragFrom == null || dragTo == null) return;
            var select = new Rect(dragFrom.Value, dragTo.Value - dragFrom.Value);
            var center = select.center;
            view.pivot = view.pivot.WithXY(center);
            var aspect = view.camera.aspect;
            var y = select.size.y.Max(select.size.x / aspect);
            view.size = y / 2 + 1f;
        }
        
        void ShowAll(SceneView view)
        {
            Rect? all = null;
            foreach(var s in info.entries)
            {
                if(all == null) all = s.range;
                else all = all.Value.BoundingBox(s.range);
            }
            
            view.pivot = view.pivot.WithXY(all.Value.center);
            var aspect = view.camera.aspect;
            var y = all.Value.size.y.Max(all.Value.size.x / aspect);
            view.size = y / 2 + 1f;
        }
        
        void ShowScenesInViewInInspector(Camera camera)
        {
            var view = camera.GetCameraWorldView();
            var scenesInView = info.entries.Where(x => view.Overlaps(x.range)).ToArray();
            EditorGUILayout.LabelField("Scenes In View");
            EditorGUILayout.LabelField("Count: " + scenesInView.Length);
            foreach(var s in scenesInView) EditorGUILayout.LabelField(s.name);
        }
        
        
        void LoadSceneByView(SceneView sv)
        {
            // 加载状态由 EditorSceneManager 获取.
            var view = sv.camera.GetCameraWorldView();
            foreach(var entry in info.entries)
            {
                if(view.Overlaps(entry.range))
                {
                    var path = "Assets/Resources/" + info.scenePath.PathCombine(entry.name).ToStandardPath() + ".unity";
                    EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                }
                else
                {
                    var scene = EditorSceneManager.GetSceneByName(entry.name);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }
        
        void ShowSceneViewText(SceneView v)
        {
            if(v.camera.transform.forward != Vector3.forward) return;
            var view = v.camera.GetCameraWorldView();
            var minLength = view.size.MinComponent();
            var baseNum = 10;
            var l = Mathf.Pow(baseNum, Mathf.Log(minLength, baseNum).FloorToInt());
            var left = (view.xMin / l).FloorToInt() * l;
            var right = (view.xMax / l).CeilToInt() * l;
            var bottom = (view.yMin / l).FloorToInt() * l;
            var top = (view.yMax / l).CeilToInt() * l;
            if(((right - left) / l + 1) * ((top - bottom) / l + 1) > 400) return;
            for(var i = left; i <= right; i += l)
            for(var j = bottom; j <= top; j += l)
            {
                Handles.Label(new Vector3(i, j, 0), $"[{i},{j}]", new GUIStyle() { fontSize = 8 });
            }
        }
        
        // void ShowAreaText(string name, Rect rect)
        // {
        //     using(new HandleColorScope(Color.red))
        //     {
        //         Handles.Label(rect.LeftCenter(), rect.xMin.ToString(), new GUIStyle() { fontSize = 10 });
        //         Handles.Label(rect.RightCenter(), rect.xMax.ToString(), new GUIStyle() { fontSize = 10 });
        //         Handles.Label(rect.BottomCenter(), rect.yMin.ToString(), new GUIStyle() { fontSize = 10 });
        //         Handles.Label(rect.TopCenter(), rect.yMax.ToString(), new GUIStyle() { fontSize = 10 });
        //     }
        // }
        
        void SwapDrag()
        {
            if(dragFrom == null || dragTo == null) return;
            
            var from = dragFrom.Value;
            var to = dragTo.Value;
            
            if(from.x > to.x)
            {
                var t = from.x;
                from.x = to.x;
                to.x = t;
            }
            
            if(from.y > to.y)
            {
                var t = from.y;
                from.y = to.y;
                to.y = t;
            }
            
            dragFrom = from;
            dragTo = to;
        }
        
    }
}
