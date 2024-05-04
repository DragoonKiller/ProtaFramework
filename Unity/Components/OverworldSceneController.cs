using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codice.LogWrapper;
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
        
        public void CheckAllActivation()
        {
            foreach(var entry in info.entries)
            {
                SetSceneActivation(entry);
            }
        }
        
        void SetSceneActivation(SceneEntry entry)
        {
            entry.SetTargetState(ShouldActive(entry));
        }
        
        bool ShouldActive(SceneEntry entry)
        {
            foreach(var referencePoint in referencePoints)
            {
                if(entry.ContainsPoint(referencePoint.position)) return true;
            }
            return false;
        }
        
        void UnloadAllUnrelatedScenes()
        {
            foreach(var entry in info.entries)
            {
                var shouldActive = ShouldActive(entry);
                var n = SceneManager.sceneCount;
                for(int i = 0; i < n; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if(scene.name == entry.name)
                    {
                        SceneManager.UnloadSceneAsync(scene);
                    }
                }
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
        
        [field: SerializeField] public int cur { get; private set; } = 0;
        
        void Start()
        {
            UnloadAllUnrelatedScenes();
        }
        
        void Update()
        {
            for(int i = 0; i < info.checkPerFrame; i++)
            {
                cur = (cur + 1).Repeat(info.entries.Length);
                var entry = info.entries[cur];
                SetSceneActivation(entry);
            }
        }
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnDrawGizmos()
        {
            if(!Application.isPlaying) return;
            foreach(var entry in info.entries)
            {
                Gizmos.color = entry.targetState ? Color.green : Color.red;
                Gizmos.DrawWireCube(entry.range.center, entry.range.size);
            }
        }
    }
}
