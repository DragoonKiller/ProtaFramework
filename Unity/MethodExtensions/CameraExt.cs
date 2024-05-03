
using System;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        
        public static Rect GetCameraWorldView(this Camera cam)
        {
            var bottomLeft = cam.ViewportPointToRay(Vector2.zero).HitXYPlane();
            var topRight = cam.ViewportPointToRay(Vector2.one).HitXYPlane();
            return new Rect(bottomLeft, topRight - bottomLeft);
        }
        
        
    }
}
