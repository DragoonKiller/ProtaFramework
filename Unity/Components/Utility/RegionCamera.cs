using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    [ExecuteAlways]
    // 配合 RegionEnable 脚本, 用于标记相机.
    public class RegionCamera : MonoBehaviour
    {
        public static RegionCamera instance; 
        
        void Awake()
        {
            instance = this;
        }
        
        void Update()
        {
            instance = this;
        }
        
        void OnDestroy()
        {
            if(instance == this) instance = null;
        }
    }
}
