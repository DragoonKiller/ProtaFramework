using System;
using UnityEngine;

namespace Prota.Tweening
{
    public struct TweenComposedScale
    {
        public readonly TweenHandle x;
        public readonly TweenHandle y;
        public readonly TweenHandle z;

        public TweenComposedScale(TweenHandle x, TweenHandle y, TweenHandle z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    
    
    
    
    public static class ScaleTweening
    {
        public static TweenHandle TweenScaleX(this Transform g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleX, g, ScaleX).SetFrom(g.localScale.x).SetTo(to).Start(time);
        }
        
        public static TweenHandle TweenScaleY(this Transform g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleY, g, ScaleY).SetFrom(g.localScale.y).SetTo(to).Start(time);
        }

        public static TweenHandle TweenScaleZ(this Transform g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleZ, g, ScaleZ).SetFrom(g.localScale.z).SetTo(to).Start(time);
        }
        
        public static TweenComposedScale TweenScale(this Transform g, Vector3 to, float time)
        {
            return new TweenComposedScale(
                TweenScaleX(g, to.x, time),
                TweenScaleY(g, to.y, time),
                TweenScaleZ(g, to.z, time)
            );
        }
        
        
        
        
        public static Transform ClearTweenScaleX(this Transform g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ScaleX);
            return g;
        }
        
        public static Transform ClearTweenScaleY(this Transform g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ScaleY);
            return g;
        }
        
        public static Transform ClearTweenScaleZ(this Transform g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ScaleZ);
            return g;
        }
        
        public static Transform ClearTweenScale(this Transform g)
        {
            g.ClearTweenScaleX();
            g.ClearTweenScaleY();
            g.ClearTweenScaleZ();
            return g;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        public static ref TweenComposedScale SetFrom(ref this TweenComposedScale m, Vector3 from)
        {
            m.x.SetFrom(from.x);
            m.y.SetFrom(from.y);
            m.z.SetFrom(from.z);
            return ref m;
        }
        
        public static ref TweenComposedScale SetTo(ref this TweenComposedScale m, Vector3 to)
        {
            m.x.SetTo(to.x);
            m.y.SetTo(to.y);
            m.z.SetTo(to.z);
            return ref m;
        }
        
        public static ref TweenComposedScale SetDuration(ref this TweenComposedScale m, float duration, bool realtime = false)
        {
            m.x.Start(duration, realtime);
            m.y.Start(duration, realtime);
            m.z.Start(duration, realtime);
            return ref m;
        }
        
        public static ref TweenComposedScale SetCurve(ref this TweenComposedScale m, AnimationCurve curve = null)
        {
            m.x.SetCurve(curve);
            m.y.SetCurve(curve);
            m.z.SetCurve(curve);
            return ref m;
        }
        
        public static ref TweenComposedScale SetGuard(ref this TweenComposedScale m, UnityEngine.Object guard)
        {
            m.x.SetGuard(guard);
            m.y.SetGuard(guard);
            m.z.SetGuard(guard);
            return ref m;
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
