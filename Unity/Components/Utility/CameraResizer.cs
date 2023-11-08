using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    // 这个组件调整相机视野, 使其总是能够看到指定的最小区域.
    // 对应的 CanvasScaler 建议调整为 ScaleWithScreenSize,
    // 并且 Reference Resolution 的长宽比和最小区域的长宽比一致.
    [RequireComponent(typeof(Camera))]
    [ExecuteAlways]
    public class CameraResizer : MonoBehaviour
    {
        // 如果是正交投影: 表示最小视野长宽.
        // 如果是透视投影: 表示最小视野角度.
        public Vector2 minSize;
        
        // 投影面到相机的距离. 只有透视投影有用.
        public float planeDistance;
        
        void Update()
        {
            var cam = this.GetComponent<Camera>();
            var curAspect = (float)Screen.width / Screen.height;
            
            if(cam.orthographic)
            {
                var targetAspect = minSize.x / minSize.y;
                if(targetAspect > curAspect)
                {
                    var curX = minSize.x;
                    var curY = minSize.x / curAspect;
                    print($"{curX} : {curY} : {curAspect}");
                    cam.orthographicSize = curY / 2;
                }
                else
                {
                    cam.orthographicSize = minSize.y / 2;
                }
            }
            else
            {
                var targetAspect = minSize.x / minSize.y;
                if(targetAspect > curAspect)
                {
                    var curX = minSize.x;
                    var curY = minSize.x / curAspect;
                    cam.fieldOfView = Mathf.Atan2(curY / 2, planeDistance) * Mathf.Rad2Deg * 2;
                }
                else
                {
                    cam.fieldOfView = Mathf.Atan2(minSize.y / 2, planeDistance) * Mathf.Rad2Deg * 2;
                }
            }
        }
        
    }
}
