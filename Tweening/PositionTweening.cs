using System;
using UnityEngine;

namespace Prota.Tweening
{
    public struct TweenComposedMove
    {
        public readonly TweeningHandle x;
        public readonly TweeningHandle y;
        public readonly TweeningHandle z;

        public TweenComposedMove(TweeningHandle x, TweeningHandle y, TweeningHandle z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    
    
    
    public static class PositionTweening
    {
        public static TweeningHandle TweenMoveX(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveX, g, SingleMoveX).SetDuration(time).RecordTime();
        }
        
        public static TweeningHandle TweenMoveY(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveY, g, SingleMoveY).SetDuration(time).RecordTime();
        }

        public static TweeningHandle TweenMoveZ(this GameObject g, float to, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveZ, g, SingleMoveZ).SetDuration(time).RecordTime();
        }
        
        public static TweenComposedMove TweenMove(this GameObject g, Vector3 to, float time)
        {
            return new TweenComposedMove(
                TweenMoveX(g, to.x, time),
                TweenMoveY(g, to.y, time),
                TweenMoveZ(g, to.z, time)
            );
        }
        
        
        
        
        public static TweeningHandle TweenTrackingX(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveX, g, TrackingMoveX).SetDuration(time).RecordTime().SetCustomData(target);
        }
        
        public static TweeningHandle TweenTrackingY(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveY, g, TrackingMoveY).SetDuration(time).RecordTime().SetCustomData(target);
        }

        public static TweeningHandle TweenTrackingZ(this GameObject g, GameObject target, float time)
        {
            return ProtaTweeningManager.instance.New(TweeningType.MoveZ, g, TrackingMoveZ).SetDuration(time).RecordTime().SetCustomData(target);
        }
        
        public static TweenComposedMove TweenTracking(this GameObject g, GameObject target, float time)
        {
            return new TweenComposedMove(
                TweenTrackingX(g, target, time),
                TweenTrackingY(g, target, time),
                TweenTrackingZ(g, target, time)
            );
        }
        
        
        
        public static GameObject ClearTweenMoveX(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.MoveX);
            return g;
        }
        
        public static GameObject ClearTweenMoveY(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.MoveY);
            return g;
        }
        
        public static GameObject ClearTweenMoveZ(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.MoveZ);
            return g;
        }
        
        public static GameObject ClearTweenMove(this GameObject g)
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
        
        public static ref TweenComposedMove RecordTime(ref this TweenComposedMove m, bool isRealtime)
        {
            m.x.RecordTime(isRealtime);
            m.y.RecordTime(isRealtime);
            m.z.RecordTime(isRealtime);
            return ref m;
        }
        
        public static ref TweenComposedMove SetDuration(ref this TweenComposedMove m, float duration)
        {
            m.x.SetDuration(duration);
            m.y.SetDuration(duration);
            m.z.SetDuration(duration);
            return ref m;
        }
        
        public static ref TweenComposedMove SetCurve(ref this TweenComposedMove m, AnimationCurve curve = null)
        {
            m.x.SetCurve(curve);
            m.y.SetCurve(curve);
            m.z.SetCurve(curve);
            return ref m;
        }
        
        public static ref TweenComposedMove SetGuard(ref this TweenComposedMove m, UnityEngine.Object guard)
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
