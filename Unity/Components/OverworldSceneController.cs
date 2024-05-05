using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Codice.CM.SEIDInfo;
using Codice.LogWrapper;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prota.Unity
{
    
    // 用于管理所有的 scene.
    public class OverworldSceneController : Singleton<OverworldSceneController>
    {
        
        public OverworldSceneInfo info;
        
        #if UNITY_EDITOR
        void OnValidate()
        {
            if(!info)
            {
                info = Resources.LoadAll<OverworldSceneInfo>("").FirstOrDefault();
            }
            
            if(!info)
            {
                Debug.LogError("OverworldSceneController: info is null, must have one.");
            }
        }
        #endif
        
        // ====================================================================================================
        // ====================================================================================================
        
        bool proceeding = false;
        [SerializeField] bool[] shouldActive;
        [SerializeField] int[] distance;
        [SerializeField] Vector2[] recordPositions;
        [SerializeField] bool[] reached;
        
        Dictionary<string, SceneEntry> name2entry = new();
        Dictionary<SceneEntry, int> entry2id = new();
        
        void CheckAllActivateImmediately()
        {
            _ = CheckAllActivate();
            asyncControl.StepUntilClear();
        }
        
        Queue<int> activeQueue = new();
        
        
        async Task CheckAllActivate()
        {
            try
            {
                var n = info.entries.Length;
                if(shouldActive.IsNullOrEmpty())
                {
                    shouldActive = new bool[n];
                    distance = new int[n];
                    reached = new bool[n];
                    name2entry = info.entries.ToDictionary(x => x.name, x => x);
                    entry2id = info.entries.Select((x, i) => (x, i)).ToDictionary(x => x.x, x => x.i);
                }
                
                proceeding = true;
                
                shouldActive.Fill(x => false);
                distance.Fill(x => (int)1e6);
                reached.Fill(x => false);
                
                var m = referencePoints.Count;
                if(recordPositions.IsNullOrEmpty() || recordPositions.Length != m)
                {
                    recordPositions = new Vector2[m];
                }
                for(int i = 0; i < m; i++)
                {
                    recordPositions[i] = referencePoints[i].position;
                }
                
                await asyncControl;
                
                void Enqueue(int id, int d)
                {
                    reached[id] = true;
                    shouldActive[id] = true;
                    distance[id] = d;
                    activeQueue.Enqueue(id);
                }
                
                activeQueue.Clear();
                for(int i = 0; i < n; i++)
                {
                    if(info.entries[i].ContainsAny(recordPositions))
                    {
                        Enqueue(i, 0);
                    }
                }
                
                await asyncControl;
                
                for(int i = 0; i < 1e4 && activeQueue.Count != 0; i++)
                {
                    var id = activeQueue.Dequeue();
                    var entry = info.entries[id];
                    if(distance[id] >= info.adjacentStep) continue;     // 不能再走了.
                    
                    foreach(var adj in entry.GetAdjacent(info.entries))
                    {
                        var adjId = entry2id[adj];
                        if(reached[adjId]) continue;
                        Enqueue(adjId, distance[id] + 1);
                    }
                    await asyncControl;
                }
                
                proceeding = false;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
        void ApplyActivation()
        {
            if(proceeding) throw new Exception("Cannot apply activation while proceeding.");
            foreach(var e in info.entries)
            {
                var id = entry2id[e];
                e.SetTargetState(shouldActive[id]);
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public List<Transform> referencePoints = new List<Transform>();
        
        public void AddReferencePoint(Transform referencePoint)
        {
            referencePoints.Add(referencePoint);
        }
        
        public void RemoveReferencePoint(Transform referencePoint)
        {
            referencePoints.Remove(referencePoint);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        AsyncControl asyncControl = new AsyncControl();
        
        void Start()
        {
            CheckAllActivateImmediately();
            ApplyActivation();
        }
        
        void Update()
        {
            asyncControl.Step();
            if(asyncControl.isClear)
            {
                ApplyActivation();
                _ = CheckAllActivate();
            }
        }
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        // 
        // #if UNITY_EDITOR
        // void OnDrawGizmos()
        // {
        //     var sstyle = new GUIStyle() { fontSize = 14 };
        //     // if(!Application.isPlaying) return;
        //     foreach(var entry in info.entries)
        //     {
        //         Gizmos.color = entry.targetState ? Color.green : Color.red;
        //         Gizmos.DrawWireCube(entry.range.center, entry.range.size);
        //         var c = GUI.color;
        //         GUI.color = Color.blue;
        //         Handles.Label(entry.range.TopLeft(), "  " + entry.name, sstyle);
        //         GUI.color = c;
        //     }
        // }
        // #endif
    }
}
