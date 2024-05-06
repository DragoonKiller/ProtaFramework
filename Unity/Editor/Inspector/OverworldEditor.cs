using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

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
        
        GameObject[] gameObjectsInSelect = Array.Empty<GameObject>();
        
        bool drawAdjacents;
        
        static bool activated;
        static Vector2? dragFrom;
        static Vector2? dragTo;
        
        Vector2 scrollPos;
        
        Rect selectOriginalRange;
        Vector2[] selectOriginalPos;
        
        SceneEntry selectedScene => info.entries.FirstOrDefault(x => x.name == selectSceneName);
        
        EditorPrefEntryBool showScenesInView;
        EditorPrefEntryVec snap;
        EditorPrefEntryString<SceneAsset> templateSceneAsset;
        
        Type tilemapWindowClass;
        
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
                }
            }
            else
            {
                var path = $"Assets/{ assetPath }.unity";
                if(templateSceneAsset.value)
                {
                    AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(templateSceneAsset.value), path);
                    EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                }
                else
                {
                    var setup = NewSceneSetup.EmptyScene;
                    var s = EditorSceneManager.NewScene(setup, NewSceneMode.Additive);
                    EditorSceneManager.SaveScene(s, path);
                }
            }
            
            Undo.RecordObject(info, "OverworldScenesInfo");
            
            SceneEntry entry = null;
            if(info.entries.FindIndex(x => x.name == selectSceneName).PassValue(out var index) == -1)
            {
                info.entries = info.entries.Resize(info.entries.Length + 1);
                index = info.entries.Length - 1;
                entry = new SceneEntry(selectSceneName, info);
                entry.adjacentScenes = Array.Empty<int>();
                info.entries[index] = entry;
            }
            
            entry = info.entries[index];
            var originalRange = entry.range;
            entry.range = new Rect(dragFrom.Value, dragTo.Value - dragFrom.Value);
            ComputeAdjacents(info.entries);
            
            if(IsSceneOpened(entry.name))
            {
                var param = new Rect[] { originalRange, entry.range };
                var gs = EditorSceneManager.GetSceneByName(entry.name).GetRootGameObjects();
                foreach(var g in gs) g.BroadcastMessage("OnOverworldUpdate", param, SendMessageOptions.DontRequireReceiver);
            }
            
            ApplySelect(entry);
            
            EditorUtility.SetDirty(info);
            AssetDatabase.SaveAssetIfDirty(info);
            
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
        
        void SelectByPointer()
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var p = ray.HitXYPlane();
            var i = info.entries.FindIndex(x => selectSceneName == x.name);
            var arr = info.entries as IEnumerable<SceneEntry>;
            if(i != -1) arr = arr.LeftRotate(i + 1);
            var entry = arr.FirstOrDefault(x => x.range.ContainsInclusive(p));
            if(entry == null) return;
            ApplySelect(entry);
        }
        
        void ApplySelect(SceneEntry entry)
        {
            dragFrom = entry.range.position;
            dragTo = entry.range.position + entry.range.size;
            selectSceneName = entry.name;
            
            // 选择了就要加载.....
            EditorSceneManager.OpenScene(entry.assetPath, OpenSceneMode.Additive);
            var scene = EditorSceneManager.GetSceneByName(selectSceneName);
            gameObjectsInSelect = scene.GetRootGameObjects();
            selectOriginalPos = gameObjectsInSelect.Select(x => x.transform.position.ToVec2()).ToArray();
            selectOriginalRange = entry.range;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnEnable()
        {
            showScenesInView = new EditorPrefEntryBool("ProtaFramework::OverworldEditor.ShowScenesInView", false);
            
            snap = new EditorPrefEntryVec("ProtaFramework::OverworldEditor.Snap");
            
            templateSceneAsset = new EditorPrefEntryString<SceneAsset>(
                "ProtaFramework::OverworldEditor.TemplateSceneAsset",
                path => AssetDatabase.LoadAssetAtPath<SceneAsset>(path),
                asset => AssetDatabase.GetAssetPath(asset)
            );
            
            tilemapWindowClass = TypeCache.GetTypesDerivedFrom(typeof(EditorWindow))
                .FirstOrDefault(x => x.Name == "GridPaintPaletteWindow");
        }
        
        void OnDisable()
        {
            ClearLoadedScenes();
        }
        

        void OnGUI()
        {
            using var _ = new EditorGUILayout.ScrollViewScope(scrollPos);
            scrollPos = _.scrollPosition;
            
            if(info == null) info = Resources.LoadAll<OverworldSceneInfo>("").FirstOrDefault();
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
            if(editMode && tilemapWindowClass != null) OpenOrCloseTilemapWindow(true);
            
            loadSceneDynamically = EditorGUILayout.Toggle("Load Scene Dynamically", loadSceneDynamically);
            if(loadSceneDynamically) LoadSceneByView(SceneView.lastActiveSceneView);
            
            if(GUILayout.Button("Focus")) Focus(SceneView.lastActiveSceneView);
            
            if(GUILayout.Button("ShowAll"))ShowAll(SceneView.lastActiveSceneView);
            
            if(GUILayout.Button("Load Selected scene")) LoadSelectedScene();
            
            if(GUILayout.Button("Load Scenes By View")) LoadSceneByView(SceneView.lastActiveSceneView);
            
            if(GUILayout.Button("Clear Loaded Scenes")) ClearLoadedScenes();
            
            if(tilemapWindowClass != null && GUILayout.Button("Open/Close Tilemap Window")) OpenOrCloseTilemapWindow();
            
            snap.vec2 = EditorGUILayout.Vector2Field("Snap", snap.vec2);
            
            templateSceneAsset.value = EditorGUILayout.ObjectField("Template Scene", templateSceneAsset.value, typeof(SceneAsset), false) as SceneAsset;
            
            showScenesInView.value = EditorGUILayout.Toggle("Show Scenes In View", showScenesInView.value);
            
            drawAdjacents = EditorGUILayout.Toggle("Draw Adjacents", drawAdjacents);
            
            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Toggle("Move Mode", moveMode);
                EditorGUILayout.Toggle("Activated", activated);
                if(dragFrom.HasValue) EditorGUILayout.Vector2Field("DragFrom", dragFrom.Value);
                if(dragTo.HasValue) EditorGUILayout.Vector2Field("DragTo", dragTo.Value);            
            }
            
            if(showScenesInView.value) ShowScenesInViewInInspector(SceneView.lastActiveSceneView.camera);
            
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

        void OpenOrCloseTilemapWindow(bool forceClose = false)
        {
            var hasOpenInstanceMethod = typeof(EditorWindow).GetMethod("HasOpenInstances");
            var constructedMethod = hasOpenInstanceMethod.MakeGenericMethod(tilemapWindowClass);
            var hasOpenInstance = (bool)constructedMethod.Invoke(null, null);
            if (hasOpenInstance)
            {
                var window = EditorWindow.GetWindow(tilemapWindowClass);
                window.Close();
            }
            else if(!forceClose)
            {
                editMode = false;
                EditorWindow.GetWindow(tilemapWindowClass);
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
            
            gameObjectsInSelect = Array.Empty<GameObject>();
            selectOriginalPos = Array.Empty<Vector2>();
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
                    if(Event.current.keyCode == KeyCode.F)
                    {
                        Focus(v);
                        Event.current.Use();
                    }
                    if(Event.current.keyCode == KeyCode.V)
                    {
                        LoadSceneByView(v);
                        Event.current.Use();
                    }
                    if(Event.current.keyCode == KeyCode.A)
                    {
                        ClearLoadedScenes();
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
                        dragTo = dragFrom = GetPointerPos().SnapTo(snap.vec2);
                        Undo.RecordObjects(gameObjectsInSelect.Select(x => x.transform).ToArray(), "Move GameObjects");
                        Undo.RecordObject(info, "OverworldScenesInfo");
                    }
                    else if(Event.current.button == 1)  // 右键按下
                    {
                        Event.current.Use();
                        dragTo = dragFrom = GetPointerPos().SnapTo(snap.vec2);
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
                            dragTo = GetPointerPos().SnapTo(snap.vec2);
                            var delta = (dragTo.Value - dragFrom.Value).SnapTo(snap.vec2);
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
                        dragTo = GetPointerPos().SnapTo(snap.vec2);
                    }
                    
                    break;
                }
                
                case EventType.MouseUp:
                {
                    if(!editMode) break;
                    if(Event.current.button == 0)       // 左键释放, 退出 moveMode, 如果没有进入 moveMode, 则选择区域.
                    {
                        Event.current.Use();
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
                            SelectByPointer(); // 左键单击, 选择区域.
                        }
                    }
                    else if(Event.current.button == 1)  // 右键释放, 修改 dragTo.
                    {
                        Event.current.Use();
                        dragTo = GetPointerPos().SnapTo(snap.vec2);
                        SwapDrag();
                    }
                    
                    break;
                }
                
                case EventType.Repaint:
                {
                    ValidateOverworldInfo();
                    
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

        void ValidateOverworldInfo()
        {
            if(info == null) return;
            if(info.entries == null) info.entries = Array.Empty<SceneEntry>();
            foreach(var e in info.entries) e.info = info;
            var n = info.entries.Length;
            info.entries = info.entries.Remove(x => !File.Exists(x.assetPath));
            if(n != info.entries.Length) ComputeAdjacents(info.entries);
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
            
            var path = info.GetAssetPathOfName(selectSceneName);
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
                    EditorSceneManager.OpenScene(entry.assetPath, OpenSceneMode.Additive);
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
            var min = dragFrom.Value.Min(dragTo.Value);
            var max = dragFrom.Value.Max(dragTo.Value);
            dragFrom = min;
            dragTo = max;
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
        
        static bool IsSceneOpened(string name)
        {
            for(int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if(scene.name == name) return true;
            }
            return false;
        }
        
        Vector2 GetPointerPos()
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            return ray.HitXYPlane();
        }
    }
}
