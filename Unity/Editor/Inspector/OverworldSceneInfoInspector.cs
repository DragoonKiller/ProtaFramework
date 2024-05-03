using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using System;

namespace Prota.Editor
{
    [CustomEditor(typeof(OverworldSceneInfo), false)]
    public class OverworldSceneInfoInspector : UnityEditor.Editor
    {
        public OverworldSceneInfo info => target as OverworldSceneInfo;
        
        
        string newSceneName;
        
        bool activated;
        bool isDragging;
        Vector2? dragFrom;
        Vector2? dragTo;
        
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
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        void ComputeAdjacents(SceneEntry[] entries)
        {
            using var _ = TempDict.Get<SceneEntry, int>(out var reverseMap);
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
            
            EditorGUILayout.LabelField(">>> New Scene <<<");
            newSceneName = EditorGUILayout.TextField("Name", newSceneName);
            if(GUILayout.Button("Create")) CreateNewScecne();
            
            EditorGUILayout.LabelField(">>> Runtime <<<");
            using(new EditorGUI.DisabledScope(true))
            {
                activated = EditorGUILayout.Toggle("Activated", activated);
                EditorGUILayout.Toggle("IsDragging", isDragging);
                if(dragFrom.HasValue) EditorGUILayout.Vector2Field("DragFrom", dragFrom.Value);
                if(dragTo.HasValue) EditorGUILayout.Vector2Field("DragTo", dragTo.Value);            
            }
            
            EditorGUILayout.LabelField(">>> OverworldSceneInfo <<<");
            base.OnInspectorGUI();
            
            serializedObject.ApplyModifiedProperties();
            
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
                        Handles.Label(s.range.TopLeft(), "  " + name, new GUIStyle() { fontSize = 14 });
                    }
                    
                    break;
                }
                
                default: break;
            }
            
            ShowSceneViewText(v);
            
            Repaint();
        }
        
        void ShowSceneViewText(SceneView v)
        {
            if(v.camera.transform.forward != Vector3.forward) return;
            var cam = v.camera;
            var bottomLeft = cam.ViewportPointToRay(Vector2.zero).HitXYPlane();
            var topRight = cam.ViewportPointToRay(Vector2.one).HitXYPlane();
            var minLength = (topRight - bottomLeft).MinComponent();
            // 找到它是几位数.
            var l = Mathf.Pow(10, Mathf.Log10(minLength).FloorToInt());
            var left = (bottomLeft.x / l).FloorToInt() * l;
            var right = (topRight.x / l).CeilToInt() * l;
            var bottom = (bottomLeft.y / l).FloorToInt() * l;
            var top = (topRight.y / l).CeilToInt() * l;
            if(((right - left) / l + 1) * ((top - bottom) / l + 1) > 500) return;
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
