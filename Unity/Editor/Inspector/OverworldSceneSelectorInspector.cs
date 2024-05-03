using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Linq;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using System;

namespace Prota.Editor
{
    [CustomEditor(typeof(OverworldSceneController), false)]
    public class OverworldSceneSelectorInspector : UnityEditor.Editor
    {
        public OverworldSceneController controller => target as OverworldSceneController;
        public OverworldScenesInfo info => controller.info;
        
        
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
                Debug.LogWarning("OverworldSceneController.info is null.");
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
            entry.adjacentScenes = Array.Empty<SceneEntry>();
            
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
            Parallel.ForEach(entries, (x, _, i) => {
                x.adjacentScenes = entries
                    .Where((x, j) => i != j)
                    .Where(y => x.range.Overlaps(y.range))
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
        
        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, "OverworldSceneController");
            
            EditorGUILayout.LabelField(">>> OverworldSceneController <<<");
            base.OnInspectorGUI();
            
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
            
            serializedObject.ApplyModifiedProperties();
            
            Repaint();
        }
        
        void OnSceneGUI()
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
                    
                    var style = new GUIStyle();
                    style.fontSize = 20;
                    var sstyle = new GUIStyle();
                    sstyle.fontSize = 10;
                    
                    foreach(var s in info.entries)
                    {
                        Handles.DrawSolidRectangleWithOutline(
                            s.range,
                            Color.green.WithA(0.0f),
                            Color.red.WithA(1f)
                        );
                        
                        using(new HandleColorScope(Color.red))
                        {
                            Handles.Label(s.range.TopLeft(), "  " + s.name, style);
                            Handles.Label(s.range.LeftCenter(), s.range.xMin.ToString(), sstyle);
                            Handles.Label(s.range.RightCenter(), s.range.xMax.ToString(), sstyle);
                            Handles.Label(s.range.BottomCenter(), s.range.yMin.ToString(), sstyle);
                            Handles.Label(s.range.TopCenter(), s.range.yMax.ToString(), sstyle);
                        }
                    }
                    
                    break;
                }
                
                default: break;
            }
            
            Repaint();
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
        
    }
}
