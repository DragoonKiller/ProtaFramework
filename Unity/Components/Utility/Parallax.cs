using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    /*
    视差模拟, 算法如下:
    令 anchorPos 是物件的初始位置.
    设它和相机 camera.pos 的相对位置是 d = object.pos - camera.pos.
    这个相对位置 d 会乘以一个数字 factor, 作为物件的最终相对相机的位置.
    factor = -∞ : 物体和相机处于同一平面.
    factor < 0 : 物体在人物和相机之间.
    factor = 0 : 物体在人物所处平面.
    0 < factor < 1 : 物体比人物更远.
    factor = 1: 物体在无穷远处.
    公式为 factor = 1 - cameraDistance / distance.
    distance 是相机与物体的距离.
    cameraDistance 是相机与人物的距离.
    */
    
    [ExecuteAlways]
    public class Parallax : MonoBehaviour
    {
        public float factor = 1f;
        
        GameObject contentRoot;
        
        void Update()
        {
            if(ParallaxCamera.instance == null) return;
            if(contentRoot == null) contentRoot = this.GetBinding("ContentRoot");
            var myPos = this.transform.position.ToVec2();
            var camPos = ParallaxCamera.instance.transform.position.ToVec2();
            var deltaPos = camPos - myPos;
            deltaPos *= factor;
            contentRoot.transform.localPosition = deltaPos;
        }
        
    }
}
