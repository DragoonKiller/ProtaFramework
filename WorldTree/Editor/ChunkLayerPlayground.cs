using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using System.Collections.Generic;
using System;

using Prota.WorldTree;
using System.Linq;
using System.Threading.Tasks;
using Prota.Unity;
using System.Threading;

namespace Prota.Editor
{
    public class ChunkLayerPlaygound : UnityEditor.EditorWindow
    {
        ChunkLayer chunk;
        
        readonly List<GameObject> targets = new List<GameObject>();
        
        ListView targetList;
        
        IntegerField updateCount;
        
        bool inited = false;
        
        IntegerField activateCountField;
        IntegerField tobeDeactiveCountField;
        IntegerField toBeAddCountField;
        IntegerField updateSpeedField;
        Toggle computedField;
        IntegerField computedLoopField;
        
        int computed = 0;
        int computedLoop = 0;
        
        CancellationTokenSource cancel = new CancellationTokenSource();
        
        void OnEnable()
        {
            targets.Clear();
            var g = GameObject.Find("Target");
            if(g != null && !targets.Contains(g)) targets.Add(g);
            
            this.titleContent = new GUIContent("Chunk Layer Playground");
            
            chunk = new ChunkLayer(7, new Vector2(0, 0), new Vector2(100, 100), 10);       // 128
            
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
            rootVisualElement.AddChild(computedField = new Toggle("computed"));
            rootVisualElement.AddChild(computedLoopField = new IntegerField("computed loop"));
            inited = true;
            
            rootVisualElement.AddChild(new Button(() => {
                chunk.Clear();
            }) { text = "reset" });
            
            rootVisualElement.AddChild(updateSpeedField = new IntegerField("update speed") { value = 10 });
            
            rootVisualElement.AddChild(new Button(() => {
                UnityEngine.Debug.Log("Start!");
                computed = 0;
                cancel.Cancel(true);
                cancel = new CancellationTokenSource();
                ProtaTask.Run(async () => {
                    while(true)
                    {
                        await new BackToMainThread();
                        chunk.SetTargetPoints(targets.Where(x => x != null).Select(x => x.transform.position));
                        await new SwitchToWorkerThread();
                        await chunk.ComputeAsync();
                        computed = 1;
                        computedLoop++;
                        while(computed > 0) await new SystemTimer(0.2); // 200ms
                        
                    }
                }, cancel.Token);
            }) { text = "start" });
            
            rootVisualElement.AddChild(new Button(() => {
                UnityEngine.Debug.Log("Cancel!");
                cancel.Cancel(true);
            }) { text = "step" });
            
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        void OnDisable()
        {
            inited = false;
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        
        void Update()
        {
            activateCountField.value = chunk.activeNodes.Count;
            toBeAddCountField.value = chunk.toBeActiveNodes.Count;
            tobeDeactiveCountField.value = chunk.toBeDeactiveNodes.Count;
            computedField.value = computed != 0;
            computedLoopField.value = computedLoop;
            
            SceneView.RepaintAll();
            
        }
        
        WorldNode[] activeNodes = new WorldNode[0];
        WorldNode[] toBeRemovedNodes = new WorldNode[0];
        WorldNode[] edgeNodes = new WorldNode[0];
        WorldNode[] toBeActiveNodes = new WorldNode[0];
        WorldNode[] toBeDeactiveNodes = new WorldNode[0];
        WorldNode[] targetNodes = new WorldNode[0];
        
        void OnSceneGUI(SceneView view)
        {
            if(computed > 0)
            {
                activeNodes = chunk.activeNodes.ToArray();
                toBeRemovedNodes = chunk.toBeRemovedNodes.ToArray();
                edgeNodes = chunk.edgeNodes.ToArray();
                toBeActiveNodes = chunk.toBeActiveNodes.ToArray();
                toBeDeactiveNodes = chunk.toBeDeactiveNodes.ToArray();
                targetNodes = chunk.targetNodes.ToArray();
                computed = 0;
            }
            
            using(var c = HandleContext.Get())
            {
                try
                {
                    Handles.color = Color.black;
                    new WorldNode(0, new Vector2Int(0, 0)).Rect(chunk.rootPosition, chunk.rootSize).DrawHandles(Vector2.zero, Vector3.one * 0.8f);
                    
                    Handles.color = Color.green;
                    foreach(var node in activeNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandles(Vector2.zero, Vector3.one * 0.8f);
                    
                    Handles.color = new Color(0.8f, 0.2f, 0.8f, 1);
                    foreach(var node in toBeRemovedNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandles(Vector2.zero, Vector3.one * 0.7f);
                    
                    Handles.color = Color.green;
                    foreach(var node in edgeNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandlesCross();
                    
                    Handles.color = Color.cyan;
                    foreach(var node in toBeActiveNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandles();
                    
                    Handles.color = Color.yellow;
                    foreach(var node in toBeDeactiveNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandlesCross();
                    
                    Handles.color = Color.cyan;
                    foreach(var node in targetNodes) node.Rect(chunk.rootPosition, chunk.rootSize).DrawHandlesCross();
                    // 
                    // Handles.color = Color.red;
                    // for(int i = chunk.processedExtendCount; i < chunk.processingExtends.Count; i++) chunk.processingExtends[i].Rect(chunk.rootPosition, chunk.rootSize).DrawHandles();
                    // 
                    // Handles.color = Color.red;
                    // for(int i = chunk.processedShrinkCount; i < chunk.processingShrinks.Count; i++) chunk.processingShrinks[i].Rect(chunk.rootPosition, chunk.rootSize).DrawHandles();
                    // 
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
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