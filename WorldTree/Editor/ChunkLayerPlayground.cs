using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using System.Collections.Generic;
using System;

using Prota.WorldTree;
using System.Linq;

namespace Prota.Editor
{
    public class ChunkLayerPlaygound : UnityEditor.EditorWindow
    {
        ChunkLayer chunk;
        IEnumerator<ChunkLayer> stepHandle;
        
        readonly List<GameObject> targets = new List<GameObject>();
        
        ListView targetList;
        
        IntegerField updateCount;
        
        bool inited = false;
        
        IntegerField activateCountField;
        IntegerField tobeDeactiveCountField;
        IntegerField toBeAddCountField;
        IntegerField updateSpeedField;
            
        void OnEnable()
        {
            targets.Clear();
            var g = GameObject.Find("Target");
            if(g != null && !targets.Contains(g)) targets.Add(g);
            
            this.titleContent = new GUIContent("Chunk Layer Playground");
            
            chunk = new ChunkLayer(7, new Vector2(0, 0), new Vector2(100, 100), 10);       // 128
            stepHandle = chunk.CreateStepHandle(100, chunk => {
                chunk.SetTargetPoints(targets.Where(x => x != null).Select(x => x.transform.position.ToVec2()));
            });
            
            rootVisualElement.AddChild(updateCount = new IntegerField("update count") { value = 0 });
            
            targetList = new ListView();
            targetList.Clear();
            
            targetList.itemsSource = targets;
            targetList.makeItem = () => new ObjectField() { objectType = typeof(GameObject) }.SetHeight(18);
            targetList.bindItem = (e, i) => {
                var x = e as ObjectField;
                x.value = targets[i];
                x.OnValueChange<ObjectField, UnityEngine.Object>(e => {
                    targets[i] = e.newValue as GameObject;
                });
            };
            
            rootVisualElement.AddChild(new VisualElement()
                .SetHorizontalLayout()
                .AddChild(new Button(() => {
                        targets.Add(null);
                        targetList.RefreshItems();
                    }) { text = "+" }
                )
                .AddChild(new Button(() => {
                        if(targets.Count == 0) return;
                        targets.RemoveLast();
                        targetList.RefreshItems();
                    }) { text = "-" })
                );
                
            rootVisualElement.AddChild(targetList);
            
            rootVisualElement.AddChild(new Vector2Field("root half size") { value = chunk.rootSize }
                .OnValueChange<Vector2Field, Vector2>(e => {
                    chunk.rootSize = e.newValue;
                })
            );
            
            rootVisualElement.AddChild(new Vector2Field("root position") { value = chunk.rootPosition }
                .OnValueChange<Vector2Field, Vector2>(e => {
                    chunk.rootPosition = e.newValue;
                })
            );
            
            rootVisualElement.AddChild(activateCountField = new IntegerField("active count"));
            rootVisualElement.AddChild(toBeAddCountField = new IntegerField("to be add count"));
            rootVisualElement.AddChild(tobeDeactiveCountField = new IntegerField("to be disable count"));
            inited = true;
            
            rootVisualElement.AddChild(new Button(() => {
                chunk.Clear();
            }) { text = "reset" });
            
            rootVisualElement.AddChild(updateSpeedField = new IntegerField("update speed") { value = 10 });
            
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        void OnDisable()
        {
            inited = false;
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        
        void Update()
        {
            if(!inited) return;
            updateCount.value = updateCount.value + 1;
            
            
            if(updateCount.value % (updateSpeedField.value * 10) == 0)
            {
                chunk.ApplyRemove();
            }
            
            if(updateCount.value % updateSpeedField.value != 0) return;
            stepHandle.MoveNext();
            
            activateCountField.value = chunk.activeNodes.Count;
            toBeAddCountField.value = chunk.toBeActiveNodes.Count;
            tobeDeactiveCountField.value = chunk.toBeDeactiveNodes.Count;
            
            SceneView.RepaintAll();
            
        }
        
        void OnSceneGUI(SceneView view)
        {
            using(var c = HandleContext.Get())
            {
                Handles.color = Color.black;
                new WorldNode(0, new Vector2Int(0, 0)).Rect(chunk.rootPosition, chunk.rootSize).DrawHandles(Vector2.zero, Vector3.one * 0.8f);
                
                Handles.color = Color.green;
                foreach(var node in chunk.activeNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandles(Vector2.zero, Vector3.one * 0.8f);
                
                Handles.color = new Color(0.8f, 0.2f, 0.8f, 1);
                foreach(var node in chunk.toBeRemovedNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandles(Vector2.zero, Vector3.one * 0.7f);
                
                Handles.color = Color.green;
                foreach(var node in chunk.edgeNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandlesCross();
                
                Handles.color = Color.cyan;
                foreach(var node in chunk.toBeActiveNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandles();
                
                Handles.color = Color.yellow;
                foreach(var node in chunk.toBeDeactiveNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandlesCross();
                
                Handles.color = Color.cyan;
                foreach(var node in chunk.targetNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandlesCross();
                
                Handles.color = Color.red;
                for(int i = chunk.processedExtendCount; i < chunk.processingExtends.Count; i++) chunk.processingExtends[i].Rect(chunk.rootPosition, chunk.rootSize).DrawHandles();
                
                Handles.color = Color.red;
                for(int i = chunk.processedShrinkCount; i < chunk.processingShrinks.Count; i++) chunk.processingShrinks[i].Rect(chunk.rootPosition, chunk.rootSize).DrawHandles();
            }
            
        }
        
        
        
        
        // [MenuItem("Kartoga/Chunk Layer Playground")]
        // public static void ChunkLayerPlaygound()
        // {
        //     var window = EditorWindow.GetWindow<Prota.Editor.ChunkLayerPlaygound>();
        //     window.Show();
        // }
    }
    
}