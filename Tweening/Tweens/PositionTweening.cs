using System;
using UnityEngine;
using Prota.Unity;

namespace Prota.Tween
{
    public static class PositionTweening
    {
        public static TweenHandle TweenMoveX(this Transform g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.MoveX, g, SingleMoveX)
                .SetGuard(g.LifeSpan()).SetFromTo(g.localPosition.x, to).Start(time);
        
        public static TweenHandle TweenMoveY(this Transform g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.MoveY, g, SingleMoveY)
                .SetGuard(g.LifeSpan()).SetFromTo(g.localPosition.y, to).Start(time);

        public static TweenHandle TweenMoveZ(this Transform g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.MoveZ, g, SingleMoveZ)
                .SetGuard(g.LifeSpan()).SetFromTo(g.localPosition.z, to).Start(time);
        
        public static ComposedHandle TweenMove(this Transform g, Vector3 to, float time)
        {
            return ComposedHandle.Make(
                "x", g.TweenMoveX(to.x, time),
                "y", g.TweenMoveY(to.y, time),
                "z", g.TweenMoveZ(to.z, time)
            );
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static void SingleMoveX(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localPosition = tr.localPosition.WithX(h.Evaluate(t));
        }
        
        static void SingleMoveY(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localPosition = tr.localPosition.WithY(h.Evaluate(t));
        }
        
        static void SingleMoveZ(TweenHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localPosition = tr.localPosition.WithZ(h.Evaluate(t));
        }
    }
    
    
}
