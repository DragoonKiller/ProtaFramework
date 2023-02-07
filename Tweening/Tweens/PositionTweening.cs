using System;
using UnityEngine;
using Prota.Unity;

namespace Prota.Tween
{
    public struct TweenComposedMove
    {
        public readonly TweenHandle x;
        public readonly TweenHandle y;
        public readonly TweenHandle z;

        public TweenComposedMove(TweenHandle x, TweenHandle y, TweenHandle z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    
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
        
        public static TweenComposedMove TweenMove(this Transform g, Vector3 to, float time)
        {
            return new TweenComposedMove(
                TweenMoveX(g, to.x, time),
                TweenMoveY(g, to.y, time),
                TweenMoveZ(g, to.z, time)
            );
        }
        
        
        public static Transform ClearTweenMoveX(this Transform g)
        {
            ProtaTweenManager.instance.Remove(g, TweenId.MoveX);
            return g;
        }
        
        public static Transform ClearTweenMoveY(this Transform g)
        {
            ProtaTweenManager.instance.Remove(g, TweenId.MoveY);
            return g;
        }
        
        public static Transform ClearTweenMoveZ(this Transform g)
        {
            ProtaTweenManager.instance.Remove(g, TweenId.MoveZ);
            return g;
        }
        
        public static Transform ClearTweenMove(this Transform g)
        {
            return g.ClearTweenMoveX().ClearTweenMoveY().ClearTweenMoveZ();
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        public static ref TweenComposedMove SetFromTo(ref this TweenComposedMove m, Vector3 from, Vector3 to)
        {
            m.x.SetFromTo(from.x, to.x);
            m.y.SetFromTo(from.y, to.y);
            m.z.SetFromTo(from.z, to.z);
            return ref m;
        }
        
        public static ref TweenComposedMove Start(ref this TweenComposedMove m, float duration, bool realtime = false)
        {
            m.Start(duration, realtime);
            m.Start(duration, realtime);
            m.Start(duration, realtime);
            return ref m;
        }
        
        public static ref TweenComposedMove SetCurve(ref this TweenComposedMove m, AnimationCurve curve = null)
        {
            m.x.SetCurve(curve);
            m.y.SetCurve(curve);
            m.z.SetCurve(curve);
            return ref m;
        }
        
        public static ref TweenComposedMove SetEase(ref this TweenComposedMove m, TweenEase ease)
        {
            m.x.SetEase(ease);
            m.y.SetEase(ease);
            m.z.SetEase(ease);
            return ref m;
        }
        
        public static ref TweenComposedMove SetGuard(ref this TweenComposedMove m, LifeSpan guard)
        {
            m.x.SetGuard(guard);
            m.y.SetGuard(guard);
            m.z.SetGuard(guard);
            return ref m;
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
