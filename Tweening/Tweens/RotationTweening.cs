using System;
using UnityEngine;
using Prota.Unity;

namespace Prota.Tween
{
    
    public static class RotationTweening
    {
        public static TweenHandle TweenRotateX(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenId.RotateX, g, RotateX)
                .SetGuard(g.LifeSpan()).SetFromTo(g.rotation.eulerAngles.x, to).Start(time);
        }
        
        public static TweenHandle TweenRotateY(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenId.RotateY, g, RotateY)
                .SetGuard(g.LifeSpan()).SetFromTo(g.rotation.eulerAngles.y, to).Start(time);
        }

        public static TweenHandle TweenRotateZ(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenId.RotateZ, g, RotateZ)
                .SetGuard(g.LifeSpan()).SetFromTo(g.rotation.eulerAngles.z, to).Start(time);
        }
        
        public static ComposedHandle TweenRotate(this Transform g, Vector3 to, float time)
        {
            return ComposedHandle.Make(
                "x", g.TweenRotateX(to.x, time),
                "y", g.TweenRotateY(to.y, time),
                "z", g.TweenRotateZ(to.z, time)
            );
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static void RotateX(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.rotation = Quaternion.Euler(tr.rotation.eulerAngles.WithX(h.Evaluate(t)));
        }
        
        static void RotateY(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.rotation = Quaternion.Euler(tr.rotation.eulerAngles.WithY(h.Evaluate(t)));
        }
        
        static void RotateZ(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.rotation = Quaternion.Euler(tr.rotation.eulerAngles.WithZ(h.Evaluate(t)));
        }
        
    }
    
    
}
