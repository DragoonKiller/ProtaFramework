using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.EditorTools;

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
        bool moveMode = false;
        bool loadSceneDynamically = false;
        
        OverworldSceneInfo info;
        
        static string selectSceneName;
        
        GameObject[] gameObjectsInSelect;
        
        bool drawAdjacents;
        
        bool showScenesInView;
        
        Vector2? cachedSnapTo;
        Vector2 snap
        {
            get
            {
                if(cachedSnapTo == null)
                {
                    var s = PlayerPrefs.GetString("ProtaFramework::OverworldEditor.SnapTo", "0,0");
                    var ss = s.Split(',');
                    cachedSnapTo = new Vector2(float.Parse(ss[0]), float.Parse(ss[1]));   
                }
                return cachedSnapTo.Value;
            }
            set
            {
                if(value == cachedSnapTo) return;
                PlayerPrefs.SetString("ProtaFramework::OverworldEditor.SnapTo", $"{value.x},{value.y}");
                cachedSnapTo = value;
            }
        }
        
        static bool activated;
        static Vector2? dragFrom;
        static Vector2? dragTo;
        
        Vector2 scrollPos;
        
        Rect selectOriginalRange;
        Vector2[] selectOriginalPos;
        
        SceneEntry selectedScene => info.entries.FirstOrDefault(x => x.name == selectSceneName);
        
        // ====================================================================================================
        // ====================================================================================================
        
        void CreateNewScecne()
        {
            if(dragFrom == null || dragTo == null)
            {
                Debug.LogWarning("DragFrom or DragTo is null.");
                return;
            }
            
            if(selectSceneName.NullOrEmpty())
            {
                Debug.LogWarning("New scene name is null or empty.");
                return;
            }
            
            if(info == null)
            {
                Debug.LogWarning("info is null.");
                return;
            }
            
            var sceneResourcePath = info.scenePath.PathCombine(selectSceneName).ToStandardPath();
            var assetPath = "Resources".PathCombine(sceneResourcePath).ToStandardPath();
            if(("Assets/" + assetPath  + ".unity").AsFileInfo().Exists)
            {
                Debug.LogWarning($"Scene file [{ selectSceneName }] already exists.");
                if(selectedScene == null)
                {
                    Debug.LogWarning($"Scene [{ selectSceneName }] already exists in OverworldScenesInfo.");
                }
                else
                {
                    Debug.LogWarning($"Add exist scene [{ selectSceneName }] to OverworldScenesInfo.");
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
            entry.name = selectSceneName;
            entry.range = new Rect(dragFrom.Value, dragTo.Value - dragFrom.Value);
            entry.adjacentScenes = Array.Empty<int>();
            
            if(info.entries.FindIndex(x => x.name == selectSceneName).PassValue(out var index) == -1)
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
            var i = info.entries.FindIndex(x => selectSceneName == x.name);
            var arr = info.entries as IEnumerable<SceneEntry>;
            if(i != -1) arr = arr.LeftRotate(i + 1);
            var entry = arr.FirstOrDefault(x => x.range.ContainsInclusive(p));
            if(entry == null) return;
            dragFrom = entry.range.position;
            dragTo = entry.range.position + entry.range.size;
            selectSceneName = entry.name;
            var scene = EditorSceneManager.GetSceneByName(selectSceneName);
            gameObjectsInSelect = scene.GetRootGameObjects();
            selectOriginalPos = gameObjectsInSelect.Select(x => x.transform.position.ToVec2()).ToArray();
            selectOriginalRange = entry.range;
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
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            if(info == null) info = FindObjectOfType<OverworldSceneInfo>();
            info = EditorGUILayout.ObjectField("OverworldSceneInfo", info, typeof(OverworldSceneInfo), false) as OverworldSceneInfo;
            if(info == null)
            {
                EditorGUILayout.LabelField("OverworldSceneInfo is null.");
                return;
            }
            
            Undo.RecordObject(info, "OverworldSceneInfo");
            
            EditorGUI.BeginChangeCheck();
            
            selectSceneName = EditorGUILayout.TextField("Select Name", selectSceneName);
            
            var buttonName = info.entries.Any(x => x.name == selectSceneName) ? "Change" : "Create";
            if(GUILayout.Button(buttonName)) CreateNewScecne();
            
            EditorGUILayout.LabelField(">>> Editor <<<");
            
            showMode = EditorGUILayout.Toggle("Show Mode", showMode);
            SceneView.duringSceneGui -= SOOnSceneGUI;
            if(showMode) SceneView.duringSceneGui += SOOnSceneGUI;
            
            editMode = EditorGUILayout.Toggle("Edit Mode", editMode);
            if(editMode) OpenOrCloseTilemapWindow(true);
            
            loadSceneDynamically = EditorGUILayout.Toggle("Load Scene Dynamically", loadSceneDynamically);
            if(loadSceneDynamically) LoadSceneByView(SceneView.lastActiveSceneView);
            
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
            
            snap = EditorGUILayout.Vector2Field("Snap", snap);

            showScenesInView = EditorGUILayout.Toggle("Show Scenes In View", showScenesInView);
            
            drawAdjacents = EditorGUILayout.Toggle("Draw Adjacents", drawAdjacents);
            
            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Toggle("Move Mode", moveMode);
                EditorGUILayout.Toggle("Activated", activated);
                if(dragFrom.HasValue) EditorGUILayout.Vector2Field("DragFrom", dragFrom.Value);
                if(dragTo.HasValue) EditorGUILayout.Vector2Field("DragTo", dragTo.Value);            
            }
            
            EditorGUILayout.EndScrollView();
            
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
                case EventType.KeyDown:
                {
                    if(Event.current.keyCode == KeyCode.X)
                    {
                        editMode = !editMode;
                        Event.current.Use();
                    }
                    if(Event.current.keyCode == KeyCode.C)
                    {
                        dragFrom = null;
                        dragTo = null;
                        Event.current.Use();
                    }
                    break;
                }
                
                case EventType.KeyUp:
                {
                    break;
                }
                
                case EventType.MouseDown:
                {
                    if(!editMode) break;
                    
                    if(Event.current.button == 0)       // 左键按下
                    {
                        Event.current.Use();
                        dragTo = dragFrom = GetPointerPos().SnapTo(snap);
                    }
                    else if(Event.current.button == 1)  // 右键按下
                    {
                        Event.current.Use();
                        dragTo = dragFrom = GetPointerPos().SnapTo(snap);
                    }
                    break;
                }
                
                case EventType.MouseDrag:
                {
                    if(!editMode) break;
                    if(Event.current.button == 0)       // 左键拖拽, 进入 moveMode.
                    {
                        Event.current.Use();
                        if(selectedScene != null)
                        {
                            if(selectedScene.range.ContainsInclusive(dragFrom.Value)) moveMode = true;
                            if(!moveMode) break;
                            dragTo = GetPointerPos().SnapTo(snap);
                            var delta = (dragTo.Value - dragFrom.Value).SnapTo(snap);
                            for(int i = 0; i < gameObjectsInSelect.Length; i++)
                            {
                                gameObjectsInSelect[i].transform.position = selectOriginalPos[i] + delta;
                                selectedScene.range = selectOriginalRange.Move(delta);
                            }
                        }
                    }
                    else if(Event.current.button == 1)  // 右键拖拽, 修改 dragTo.
                    {
                        Event.current.Use();
                        dragTo = GetPointerPos().SnapTo(snap);
                    }
                    
                    break;
                }
                
                case EventType.MouseUp:
                {
                    if(!editMode) break;
                    if(Event.current.button == 0)       // 左键释放, 退出 moveMode, 如果没有进入 moveMode, 则选择区域.
                    {
                        Event.current.Use();
                        SwapDrag();
                        if(moveMode)
                        {
                            moveMode = false;
                            selectOriginalPos = gameObjectsInSelect.Select(x => x.transform.position.ToVec2()).ToArray();
                            selectOriginalRange = selectedScene.range;
                            dragFrom = selectedScene.range.min;
                            dragTo = selectedScene.range.max;
                            ComputeAdjacents(info.entries);
                        }
                        else
                        {
                            Select(); // 左键单击, 选择区域.
                        }
                    }
                    else if(Event.current.button == 1)  // 右键释放, 修改 dragTo.
                    {
                        Event.current.Use();
                        dragTo = GetPointerPos().SnapTo(snap);
                    }
                    
                    break;
                }
                
                case EventType.Repaint:
                {
                    if(moveMode)
                    {
                        Handles.DrawSolidRectangleWithOutline(
                            selectedScene.range,
                            Color.blue.WithG(0.4f).WithA(0.1f),
                            Color.blue.WithA(0.8f)
                        );
                    }
                    else if(dragFrom.HasValue && dragTo.HasValue)
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
                                using var _ = new HandleColorScope(Color.green);
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
            
            if(!info.entries.Any(x => x.name == selectSceneName))
            {
                Debug.LogWarning("Scene not found.");
                return;
            }
            
            var path = "Assets/Resources/" + info.scenePath.PathCombine(selectSceneName).ToStandardPath() + ".unity";
            EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            
            foreach(var entry in info.entries)
            {
                if(entry.name == selectSceneName) continue;
                var scene = EditorSceneManager.GetSceneByName(entry.name);
                EditorSceneManager.CloseScene(scene, true);
            }
        }
        
        Dictionary<string, Scene> loadedScenes = new();
        void LoadSceneByView(SceneView sv)
        {
            EditorSceneManager.SaveOpenScenes();
            
            loadedScenes.Clear();
            for(int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                loadedScenes.Add(scene.name, scene);
            }
            
            // 加载状态由 EditorSceneManager 获取.
            var view = sv.camera.GetCameraWorldView();
            foreach(var entry in info.entries)
            {
                if(view.Overlaps(entry.range))
                {
                    if(loadedScenes.ContainsKey(entry.name)) continue;
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
        
        // ====================================================================================================
        // Utils
        // ====================================================================================================
        
        Vector2 GetPointerPos()
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            return ray.HitXYPlane();
        }
    }
}
