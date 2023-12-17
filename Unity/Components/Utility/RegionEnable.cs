using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    
    [ExecuteAlways]
    public class RegionEnable : MonoBehaviour
    {
        #if UNITY_EDITOR
        public bool enableInEditor = true;
        #endif
        
        public GameObject[] targets;
        
        public Collider2D reference;
        
        void Update()
        {
            #if UNITY_EDITOR
            if(!enableInEditor) return;
            #endif
            
            if(targets.IsNullOrEmpty()) return;
            if(reference == null) return;
            
            var camera = RegionCamera.instance;
            if(camera == null) return;
            
            var cameraInRegion = reference.OverlapPoint(camera.transform.position.ToVec2());
            targets.SetActiveAll(cameraInRegion);
        }
        
    }
}
