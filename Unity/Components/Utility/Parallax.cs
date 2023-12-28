using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    /*
    视差模拟.
    cameraDistance: 相机到焦平面(一般是人物所在平面)的距离.
    distance: 物件到焦平面的距离.
    factor: 物件在画面上的移动乘数. 即物件在焦平面上的投影的移动速度比.
    当焦平面的人物移动了距离 relativeDistance 时, 距离为 distance 的物体在焦平面上的投影位移为
    h = relativeDistance / distance * cameraDistance
    此时只需要设置 relativeDistance 为物体的实际位置和人物的距离, 即可算出投影距离h.
    */
    
    [ExecuteAlways]
    public class Parallax : MonoBehaviour
    {
        // 与焦平面的距离. 正数代表远离相机, 负数代表靠近相机.
        public float distance = 0f;
        
        GameObject contentRoot;
        
        void LateUpdate()
        {
            if(ParallaxCamera.instance == null) return;
            if(contentRoot == null) contentRoot = this.GetBinding("ContentRoot");
            
            var myPos = this.transform.position;
            var camPos = ParallaxCamera.instance.transform.position;
            var cameraDistance = myPos.z - camPos.z;
            // var objectDistance = distance - camPos.z;
            var relativeMove =  myPos.ToVec2().To(camPos.ToVec2());
            relativeMove *= 1 - cameraDistance / (distance + cameraDistance);
            contentRoot.transform.localPosition = relativeMove;
        }
        
    }
}
