using System;
using UnityEngine;

namespace Prota.Tweening
{
    public struct TweenComposedScale
    {
        public readonly TweeningHandle x;
        public readonly TweeningHandle y;
        public readonly TweeningHandle z;

        public TweenComposedScale(TweeningHandle x, TweeningHandle y, TweeningHandle z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    
    
    
    
    public static class ScaleTweening
    {
        public static TweeningHandle TweenScaleX(this Transform g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleX, g, ScaleX).SetDuration(time).RecordTime();
        }
        
        public static TweeningHandle TweenScaleY(this Transform g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleY, g, ScaleY).SetDuration(time).RecordTime();
        }

        public static TweeningHandle TweenScaleZ(this Transform g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleZ, g, ScaleZ).SetDuration(time).RecordTime();
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
        
        public static ref TweenComposedScale RecordTime(ref this TweenComposedScale m, bool isRealtime)
        {
            m.x.RecordTime(isRealtime);
            m.y.RecordTime(isRealtime);
            m.z.RecordTime(isRealtime);
            return ref m;
        }
        
        public static ref TweenComposedScale SetDuration(ref this TweenComposedScale m, float duration)
        {
            m.x.SetDuration(duration);
            m.y.SetDuration(duration);
            m.z.SetDuration(duration);
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
        
        
        static void ScaleX(TweeningHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localScale = tr.localScale.WithX(h.Evaluate(t));
        }
        
        static void ScaleY(TweeningHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localScale = tr.localScale.WithY(h.Evaluate(t));
        }
        
        static void ScaleZ(TweeningHandle h, float t)
        {
            var tr = (Transform)h.target;
            tr.localScale = tr.localScale.WithZ(h.Evaluate(t));
        }
        
        
        static void TrackingScaleX(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = (Transform)h.target;
            h.SetTo((h.customData as Transform).transform.position.x);
            tr.localScale = tr.localScale.WithX(h.Evaluate(t));
        }
        
        static void TrackingScaleY(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = (Transform)h.target;
            h.SetTo((h.customData as Transform).transform.position.y);
            tr.localScale = tr.localScale.WithY(h.Evaluate(t));
        }
        
        static void TrackingScaleZ(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = (Transform)h.target;
            h.SetTo((h.customData as Transform).transform.position.z);
            tr.localScale = tr.localScale.WithZ(h.Evaluate(t));
        }
        
    }
    
    
}
