using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    [ExecuteAlways]
    // 配合 Parallax 脚本, 用于标记相机.
    public class ParallaxCamera : MonoBehaviour
    {
        public static ParallaxCamera instance; 
        
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
