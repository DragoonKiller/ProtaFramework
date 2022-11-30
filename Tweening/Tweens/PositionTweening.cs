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
            => ProtaTweenManager.instance.New(TweenType.MoveX, g, SingleMoveX)
                .SetGuard(g.LifeSpan()).SetFrom(g.localPosition.x).SetTo(to).Start(time);
        
        public static TweenHandle TweenMoveY(this Transform g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.MoveY, g, SingleMoveY)
                .SetGuard(g.LifeSpan()).SetFrom(g.localPosition.y).SetTo(to).Start(time);

        public static TweenHandle TweenMoveZ(this Transform g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.MoveZ, g, SingleMoveZ)
                .SetGuard(g.LifeSpan()).SetFrom(g.localPosition.z).SetTo(to).Start(time);
        
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
            ProtaTweenManager.instance.Remove(g, TweenType.MoveX);
            return g;
        }
        
        public static Transform ClearTweenMoveY(this Transform g)
        {
            ProtaTweenManager.instance.Remove(g, TweenType.MoveY);
            return g;
        }
        
        public static Transform ClearTweenMoveZ(this Transform g)
        {
            ProtaTweenManager.instance.Remove(g, TweenType.MoveZ);
            return g;
        }
        
        public static Transform ClearTweenMove(this Transform g)
        {
            g.ClearTweenMoveX();
            g.ClearTweenMoveY();
            g.ClearTweenMoveZ();
            return g;
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        public static ref TweenComposedMove SetFrom(ref this TweenComposedMove m, Vector3 from)
        {
            m.x.SetFrom(from.x);
            m.y.SetFrom(from.y);
            m.z.SetFrom(from.z);
            return ref m;
        }
        
        public static ref TweenComposedMove SetTo(ref this TweenComposedMove m, Vector3 to)
        {
            m.x.SetTo(to.x);
            m.y.SetTo(to.y);
            m.z.SetTo(to.z);
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
        
        
        static void TrackingMoveX(TweenHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = (Transform)h.target;
            h.SetTo((h.customData as Transform).transform.position.x);
            tr.localPosition = tr.localPosition.WithX(h.Evaluate(t));
        }
        
        static void TrackingMoveY(TweenHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = (Transform)h.target;
            h.SetTo((h.customData as Transform).transform.position.y);
            tr.localPosition = tr.localPosition.WithY(h.Evaluate(t));
        }
        
        static void TrackingMoveZ(TweenHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = (Transform)h.target;
            h.SetTo((h.customData as Transform).transform.position.z);
            tr.localPosition = tr.localPosition.WithZ(h.Evaluate(t));
        }
        
    }
    
    
}
