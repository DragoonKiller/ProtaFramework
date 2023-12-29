using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Prota.Unity;

namespace Prota.VisualEffect
{
    [ExecuteAlways]
    public class GodLightSource : MonoBehaviour
    {
        public static GodLightSource instance;
        
        public float radius = 5f;
        
        public float sampleRadius = 1f;
        
        public float intensity = 1;
        
        public Vector3 worldPos => transform.position;
        
        public Vector3 radiusRefPos => worldPos + Vector3.right * radius;
        
        void OnEnable()
        {
            if(instance != null) Debug.LogWarning("Multiple GodLightSource in scene");
            instance = this;
        }
        
        void OnDisable()
        {
            if(instance == this) instance = null;
        }
        
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(worldPos, radius);
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(worldPos, sampleRadius);
        }
        
        
    }
}
