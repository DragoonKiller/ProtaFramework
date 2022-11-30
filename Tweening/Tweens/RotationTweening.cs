using System;
using UnityEngine;
using Prota.Unity;

namespace Prota.Tween
{
    public struct TweenComposedRotate
    {
        public readonly TweenHandle x;
        public readonly TweenHandle y;
        public readonly TweenHandle z;

        public TweenComposedRotate(TweenHandle x, TweenHandle y, TweenHandle z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    
    
    
    public static class RotationTweening
    {
        public static TweenHandle TweenRotateX(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenType.RotateX, g, RotateX)
                .SetGuard(g.LifeSpan()).SetFrom(g.rotation.eulerAngles.x).SetTo(to).Start(time);
        }
        
        public static TweenHandle TweenRotateY(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenType.RotateY, g, RotateY)
                .SetGuard(g.LifeSpan()).SetFrom(g.rotation.eulerAngles.y).SetTo(to).Start(time);
        }

        public static TweenHandle TweenRotateZ(this Transform g, float to, float time)
        {
            return ProtaTweenManager.instance.New(TweenType.RotateZ, g, RotateZ)
                .SetGuard(g.LifeSpan()).SetFrom(g.rotation.eulerAngles.z).SetTo(to).Start(time);
        }
        
        public static TweenComposedRotate TweenRotate(this Transform g, Vector3 to, float time)
        {
            return new TweenComposedRotate(
                TweenRotateX(g, to.x, time),
                TweenRotateY(g, to.y, time),
                TweenRotateZ(g, to.z, time)
            );
        }
        
        
        public static Transform ClearTweenRotateX(this Transform g)
        {
            ProtaTweenManager.instance.Remove(g, TweenType.RotateX);
            return g;
        }
        
        public static Transform ClearTweenRotateY(this Transform g)
        {
            ProtaTweenManager.instance.Remove(g, TweenType.RotateY);
            return g;
        }
        
        public static Transform ClearTweenRotateZ(this Transform g)
        {
            ProtaTweenManager.instance.Remove(g, TweenType.RotateZ);
            return g;
        }
        
        public static Transform ClearTweenRotate(this Transform g)
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
        
        public static ref TweenComposedRotate SetDuration(ref this TweenComposedRotate m, float duration, bool realtime = false)
        {
            m.x.Start(duration, realtime);
            m.y.Start(duration, realtime);
            m.z.Start(duration, realtime);
            return ref m;
        }
        
        public static ref TweenComposedRotate SetCurve(ref this TweenComposedRotate m, AnimationCurve curve = null)
        {
            m.x.SetCurve(curve);
            m.y.SetCurve(curve);
            m.z.SetCurve(curve);
            return ref m;
        }
        
        public static ref TweenComposedRotate SetGuard(ref this TweenComposedRotate m, LifeSpan guard)
        {
            m.x.SetGuard(guard);
            m.y.SetGuard(guard);
            m.z.SetGuard(guard);
            return ref m;
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
