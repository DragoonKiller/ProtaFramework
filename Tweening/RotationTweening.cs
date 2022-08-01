using System;
using UnityEngine;

namespace Prota.Tweening
{
    public struct TweenComposedRotate
    {
        public readonly TweeningHandle x;
        public readonly TweeningHandle y;
        public readonly TweeningHandle z;

        public TweenComposedRotate(TweeningHandle x, TweeningHandle y, TweeningHandle z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    
    
    
    public static class RotationTweening
    {
        public static TweeningHandle TweenRotateX(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.RotateX, g, SingleMoveX).SetDuration(time).RecordTime();
        }
        
        public static TweeningHandle TweenRotateY(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.RotateY, g, SingleMoveY).SetDuration(time).RecordTime();
        }

        public static TweeningHandle TweenRotateZ(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.RotateZ, g, SingleMoveZ).SetDuration(time).RecordTime();
        }
        
        public static TweenComposedRotate TweenRotate(this GameObject g, Vector3 to, float time)
        {
            return new TweenComposedRotate(
                TweenRotateX(g, to.x, time),
                TweenRotateY(g, to.y, time),
                TweenRotateZ(g, to.z, time)
            );
        }
        
        
        
        
        public static TweeningHandle TweenTrackingRotateX(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.RotateX, g, TrackingMoveX).SetDuration(time).RecordTime().SetCustomData(target);
        }
        
        public static TweeningHandle TweenTrackingRotateY(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.RotateY, g, TrackingMoveY).SetDuration(time).RecordTime().SetCustomData(target);
        }

        public static TweeningHandle TweenTrackingRotateZ(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.RotateZ, g, TrackingMoveZ).SetDuration(time).RecordTime().SetCustomData(target);
        }
        
        public static TweenComposedRotate TweenTracking(this GameObject g, GameObject target, float time)
        {
            return new TweenComposedRotate(
                TweenTrackingRotateX(g, target, time),
                TweenTrackingRotateY(g, target, time),
                TweenTrackingRotateZ(g, target, time)
            );
        }
        
        
        
        public static GameObject ClearTweenRotateX(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.RotateX);
            return g;
        }
        
        public static GameObject ClearTweenRotateY(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.RotateY);
            return g;
        }
        
        public static GameObject ClearTweenRotateZ(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.RotateZ);
            return g;
        }
        
        public static GameObject ClearTweenRotate(this GameObject g)
        {
            g.ClearTweenRotateX();
            g.ClearTweenRotateY();
            g.ClearTweenRotateZ();
            return g;
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        public static ref TweenComposedRotate SetFrom(ref this TweenComposedRotate m, Vector3 from)
        {
            m.x.SetFrom(from.x);
            m.y.SetFrom(from.y);
            m.z.SetFrom(from.z);
            return ref m;
        }
        
        public static ref TweenComposedRotate SetTo(ref this TweenComposedRotate m, Vector3 to)
        {
            m.x.SetTo(to.x);
            m.y.SetTo(to.y);
            m.z.SetTo(to.z);
            return ref m;
        }
        
        public static ref TweenComposedRotate RecordTime(ref this TweenComposedRotate m, bool isRealtime)
        {
            m.x.RecordTime(isRealtime);
            m.y.RecordTime(isRealtime);
            m.z.RecordTime(isRealtime);
            return ref m;
        }
        
        public static ref TweenComposedRotate SetDuration(ref this TweenComposedRotate m, float duration)
        {
            m.x.SetDuration(duration);
            m.y.SetDuration(duration);
            m.z.SetDuration(duration);
            return ref m;
        }
        
        public static ref TweenComposedRotate SetCurve(ref this TweenComposedRotate m, AnimationCurve curve = null)
        {
            m.x.SetCurve(curve);
            m.y.SetCurve(curve);
            m.z.SetCurve(curve);
            return ref m;
        }
        
        public static ref TweenComposedRotate SetGuard(ref this TweenComposedRotate m, UnityEngine.Object guard)
        {
            m.x.SetGuard(guard);
            m.y.SetGuard(guard);
            m.z.SetGuard(guard);
            return ref m;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static void SingleMoveX(TweeningHandle h, float t)
        {
            var tr = h.binding.transform;
            tr.position = tr.position.WithX(h.Evaluate(t));
        }
        
        static void SingleMoveY(TweeningHandle h, float t)
        {
            var tr = h.binding.transform;
            tr.position = tr.position.WithY(h.Evaluate(t));
        }
        
        static void SingleMoveZ(TweeningHandle h, float t)
        {
            var tr = h.binding.transform;
            tr.position = tr.position.WithZ(h.Evaluate(t));
        }
        
        
        static void TrackingMoveX(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo((h.customData as GameObject).transform.position.x);
            tr.position = tr.position.WithX(h.Evaluate(t));
        }
        
        static void TrackingMoveY(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo((h.customData as GameObject).transform.position.y);
            tr.position = tr.position.WithY(h.Evaluate(t));
        }
        
        static void TrackingMoveZ(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo((h.customData as GameObject).transform.position.z);
            tr.position = tr.position.WithZ(h.Evaluate(t));
        }
        
    }
    
    
}
