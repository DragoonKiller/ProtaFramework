using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Configuration;

namespace Prota.Editor
{
    
    public class OverworldEditor : UnityEditor.EditorWindow
    {
        [MenuItem("ProtaFramework/Window/Overworld Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<OverworldEditor>();
            window.titleContent = new GUIContent("Overworld Editor");
            window.Show();
        }
        
        bool showMode = true;
        bool editMode = false;
        
        OverworldSceneInfo info;
        
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
            var i = info.entries.FindIndex(x => newSceneName == x.name);
            var arr = info.entries as IEnumerable<SceneEntry>;
            if(i != -1) arr = arr.LeftRotate(i + 1);
            var entry = arr.FirstOrDefault(x => x.range.ContainsInclusive(p));
            if(entry == null) return;
            dragFrom = entry.range.position;
            dragTo = entry.range.position + entry.range.size;
            newSceneName = entry.name;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnEnable()
        {
            
        }
        
        void OnDisable()
        {
            
        }
        

        void OnGUI()
        {
            if(info == null) info = FindObjectOfType<OverworldSceneInfo>();
            info = EditorGUILayout.ObjectField("OverworldSceneInfo", info, typeof(OverworldSceneInfo), false) as OverworldSceneInfo;
            if(info == null)
            {
                EditorGUILayout.LabelField("OverworldSceneInfo is null.");
                return;
            }
            
            showMode = EditorGUILayout.Toggle("ShowMode", showMode);
            SceneView.duringSceneGui -= SOOnSceneGUI;
            if(showMode) SceneView.duringSceneGui += SOOnSceneGUI;
            
            editMode = EditorGUILayout.Toggle("EditMode", editMode);
            if(editMode) OpenOrCloseTilemapWindow(true);
            
            Undo.RecordObject(info, "OverworldSceneInfo");
            
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
            
            if(GUILayout.Button("Load Selected scene"))
            {
                LoadSelectedScene();
            }
            
            if(GUILayout.Button("Load Scenes By View"))
            {
                LoadSceneByView(SceneView.lastActiveSceneView);
            }
            
            if(GUILayout.Button("Clear Loaded Scenes"))
            {
                ClearLoadedScenes();
            }
            
            if(GUILayout.Button("Open/Close Tilemap Window"))
            {
                OpenOrCloseTilemapWindow();
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
            
            if(EditorGUI.EndChangeCheck())
            {
                var path = new string[] { info.scenePathRelativeToRoot };
                var scenes = AssetDatabase.FindAssets("t:scene", path)
                    .Select(x => AssetDatabase.GUIDToAssetPath(x))
                    .ToArray();
                
                var ss = EditorBuildSettings.scenes.ToList();
                ss.RemoveAll(x => scenes.Contains(x.path));
                ss.AddRange(scenes.Select(x => new EditorBuildSettingsScene(x, true)));
                EditorBuildSettings.scenes = ss.ToArray();
                
                EditorUtility.SetDirty(info);
                AssetDatabase.SaveAssetIfDirty(info);
            }
            
            Repaint();
        }

        static void OpenOrCloseTilemapWindow(bool forceClose = false)
        {
            var type = TypeCache.GetTypesDerivedFrom(typeof(EditorWindow))
                .FirstOrDefault(x => x.Name == "GridPaintPaletteWindow");
            var hasOpenInstanceMethod = typeof(EditorWindow).GetMethod("HasOpenInstances");
            var constructedMethod = hasOpenInstanceMethod.MakeGenericMethod(type);
            var hasOpenInstance = (bool)constructedMethod.Invoke(null, null);
            if (hasOpenInstance)
            {
                var window = EditorWindow.GetWindow(type);
                window.Close();
            }
            else if(!forceClose)
            {
                EditorWindow.GetWindow(type);
            }
        }

        private void ClearLoadedScenes()
        {
            EditorSceneManager.SaveOpenScenes();
            
            foreach(var entry in info.entries)
            {
                var scene = EditorSceneManager.GetSceneByName(entry.name);
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        void SOOnSceneGUI(SceneView v)
        {
            if(!editMode)
            {
                dragFrom = dragTo = null;
            }
            
            switch(Event.current.type)
            {
                case EventType.MouseDown:
                {
                    if(!editMode) break;
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
                    if(!editMode) break;
                    Event.current.Use();
                    var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if(isDragging) dragTo = ray.HitXYPlane();
                    break;
                }
                
                case EventType.MouseUp:
                {
                    if(!editMode) break;
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
                            Color.blue.WithA(0.1f),
                            Color.blue.WithA(0.8f)
                        );
                    }
                    
                    foreach(var s in info.entries)
                    {
                        var insideColor = s.state == SceneLoadingState.Loaded ? Color.green : Color.red;
                        var outlineColor = s.state == SceneLoadingState.Loaded ? Color.green : Color.red;
                        Handles.DrawSolidRectangleWithOutline(
                            s.range,
                            insideColor.WithA(0.01f),
                            outlineColor.WithA(1f)
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
                                Handles.DrawLine(s.range.center, a.range.center, 2);
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
        
        
        void LoadSelectedScene()
        {
            EditorSceneManager.SaveOpenScenes();
            
            if(!info.entries.Any(x => x.name == newSceneName))
            {
                Debug.LogWarning("Scene not found.");
                return;
            }
            
            var path = "Assets/Resources/" + info.scenePath.PathCombine(newSceneName).ToStandardPath() + ".unity";
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            
            foreach(var entry in info.entries)
            {
                if(entry.name == newSceneName) continue;
                var scene = EditorSceneManager.GetSceneByName(entry.name);
                EditorSceneManager.CloseScene(scene, true);
            }
        }
        
        void LoadSceneByView(SceneView sv)
        {
            EditorSceneManager.SaveOpenScenes();
            
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
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static void ShowSceneViewText(SceneView v)
        {
            if(v.camera.transform.forward != Vector3.forward) return;
            var view = v.camera.GetCameraWorldView();
            var minLength = view.size.MinComponent();
            var baseNum = 10;
            var l = Mathf.Pow(baseNum, Mathf.Log(minLength, baseNum).FloorToInt());
            var n = (minLength / l).FloorToInt();
            // Debug.LogWarning(n);
            if(n < 5 && l % 2 == 0) l /= 2;
            var left = (view.xMin / l).FloorToInt() * l;
            var right = (view.xMax / l).CeilToInt() * l;
            var bottom = (view.yMin / l).FloorToInt() * l;
            var top = (view.yMax / l).CeilToInt() * l;
            
            var total = (right - left) / l * (top - bottom) / l;
            if(total > 1000 || total is float.NaN) return;
            
            var style = new GUIStyle() { fontSize = 9 };
            for(var i = left; i <= right; i += l)
            for(var j = bottom; j <= top; j += l)
            {
                if((i % 1).Abs() > 1e-5f || (j % 1).Abs() > 1e-5f) continue;
                Handles.Label(new Vector3(i, j, 0), $"[{i.RoundToInt()},{j.RoundToInt()}]", style);
            }
        }
    }
}
