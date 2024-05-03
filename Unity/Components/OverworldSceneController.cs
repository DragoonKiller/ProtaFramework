using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Prota.Unity
{
    
    // 用于管理所有的 scene.
    public class OverworldSceneController : Singleton<OverworldSceneController>
    {
        
        public OverworldScenesInfo info;
        
        #if UNITY_EDITOR
        void OnValidate()
        {
            if(!info)
            {
                info = Resources.LoadAll<OverworldScenesInfo>("").FirstOrDefault();
            }
            
            if(!info)
            {
                Debug.LogError("OverworldSceneController: info is null, must have one.");
            }
        }
        #endif
        
        // ====================================================================================================
        // ====================================================================================================
        
        public void SetScenesActivation()
        {
            foreach(var entry in info.entries)
            {
                bool shouldActivate = false;
                foreach(var referencePoint in referencePoints)
                {
                    if(entry.ContainsPoint(referencePoint.position))
                    {
                        shouldActivate = true;
                        break;
                    }
                }
                entry.SetTargetState(shouldActivate);
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
        
        
        AsyncControl asyncControl;
        
        void Update()
        {
            asyncControl.Step();
        }
        
        
        
        
        
        
        
        
        
        
    }
}
