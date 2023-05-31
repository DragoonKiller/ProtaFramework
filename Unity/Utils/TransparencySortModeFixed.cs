using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Unity
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class TransparencySortModeFixed : MonoBehaviour
    {
        public Vector3 direction = Vector3.forward;
        public TransparencySortMode mode = TransparencySortMode.CustomAxis;
        
        Camera cam;
        void Update()
        {
            cam = cam != null ? cam : this.GetComponent<Camera>();
            cam.transparencySortAxis = direction;
            cam.transparencySortMode = mode;
        }
    
    }
}
