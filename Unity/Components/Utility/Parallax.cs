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
    越远的物体 factor 越小, 物件移动越慢. 比人物更远则 factor < 1.
    越近的物体 factor 越大, 物件移动越快. 比人物更近则 factor > 1.
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
            contentRoot.transform.localPosition = -deltaPos;
        }
        
    }
}
