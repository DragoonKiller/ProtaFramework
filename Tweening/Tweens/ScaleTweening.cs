using System;
using UnityEngine;
using Prota.Unity;

namespace Prota.Tween
{
    
    public static class ScaleTweening
    {
        public static TweenHandle TweenScaleX(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenId.ScaleX, g, ScaleX)
                .SetGuard(g.LifeSpan()).SetFromTo(g.localScale.x, to).Start(time);
        }
        
        public static TweenHandle TweenScaleY(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenId.ScaleY, g, ScaleY)
                .SetGuard(g.LifeSpan()).SetFromTo(g.localScale.y, to).Start(time);
        }

        public static TweenHandle TweenScaleZ(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenId.ScaleZ, g, ScaleZ)
                .SetGuard(g.LifeSpan()).SetFromTo(g.localScale.z, to).Start(time);
        }
        
        public static ComposedHandle TweenScale(this Transform g, Vector3 to, float time)
        {
            return ComposedHandle.Make(
                "x", g.TweenScaleX(to.x, time),
                "y", g.TweenScaleY(to.y, time),
                "z", g.TweenScaleZ(to.z, time)
            );
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static void ScaleX(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localScale = tr.localScale.WithX(h.Evaluate(t));
        }
        
        static void ScaleY(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localScale = tr.localScale.WithY(h.Evaluate(t));
        }
        
        static void ScaleZ(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localScale = tr.localScale.WithZ(h.Evaluate(t));
        }
        
    }
    
    
}
