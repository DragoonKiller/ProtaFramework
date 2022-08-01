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
        public static TweeningHandle TweenScaleX(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleX, g, ScaleX).SetDuration(time).RecordTime();
        }
        
        public static TweeningHandle TweenScaleY(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleY, g, ScaleY).SetDuration(time).RecordTime();
        }

        public static TweeningHandle TweenScaleZ(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.ScaleZ, g, ScaleZ).SetDuration(time).RecordTime();
        }
        
        public static TweenComposedScale TweenScale(this GameObject g, Vector3 to, float time)
        {
            return new TweenComposedScale(
                TweenScaleX(g, to.x, time),
                TweenScaleY(g, to.y, time),
                TweenScaleZ(g, to.z, time)
            );
        }
        
        
        
        public static TweeningHandle TweenTrackingScaleX(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveX, g, TrackingScaleX).SetDuration(time).RecordTime().SetCustomData(target);
        }
        
        public static TweeningHandle TweenTrackingScaleY(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveY, g, TrackingScaleY).SetDuration(time).RecordTime().SetCustomData(target);
        }

        public static TweeningHandle TweenTrackingScaleZ(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveZ, g, TrackingScaleZ).SetDuration(time).RecordTime().SetCustomData(target);
        }
        
        public static TweenComposedScale TweenTrackingScale(this GameObject g, GameObject target, float time)
        {
            return new TweenComposedScale(
                TweenTrackingScaleX(g, target, time),
                TweenTrackingScaleY(g, target, time),
                TweenTrackingScaleZ(g, target, time)
            );
        }
        
        
        public static GameObject ClearTweenScaleX(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ScaleX);
            return g;
        }
        
        public static GameObject ClearTweenScaleY(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ScaleY);
            return g;
        }
        
        public static GameObject ClearTweenScaleZ(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ScaleZ);
            return g;
        }
        
        public static GameObject ClearTweenScale(this GameObject g)
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
            var tr = h.binding.transform;
            tr.localScale = tr.localScale.WithX(h.Evaluate(t));
        }
        
        static void ScaleY(TweeningHandle h, float t)
        {
            var tr = h.binding.transform;
            tr.localScale = tr.localScale.WithY(h.Evaluate(t));
        }
        
        static void ScaleZ(TweeningHandle h, float t)
        {
            var tr = h.binding.transform;
            tr.localScale = tr.localScale.WithZ(h.Evaluate(t));
        }
        
        
        static void TrackingScaleX(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo((h.customData as GameObject).transform.position.x);
            tr.localScale = tr.localScale.WithX(h.Evaluate(t));
        }
        
        static void TrackingScaleY(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo((h.customData as GameObject).transform.position.y);
            tr.localScale = tr.localScale.WithY(h.Evaluate(t));
        }
        
        static void TrackingScaleZ(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo((h.customData as GameObject).transform.position.z);
            tr.localScale = tr.localScale.WithZ(h.Evaluate(t));
        }
        
    }
    
    
}
