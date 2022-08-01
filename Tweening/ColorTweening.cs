using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prota.Tweening
{
    public struct TweenComposedColor
    {
        public readonly TweeningHandle x;
        public readonly TweeningHandle y;
        public readonly TweeningHandle z;

        public TweenComposedColor(TweeningHandle x, TweeningHandle y, TweeningHandle z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    
    
    
    public static class ColorTweening
    {
        public static TweeningHandle TweenColorR(this GameObject g, float to, float time)
        {
            var handle = null as TweeningHandle;
            handle = ProtaTweeningManager.instance.New(TweeningType.ColorR, g, ColorR).SetDuration(time).RecordTime();
            return handle;
        }
        
        public static TweeningHandle TweenColorG(this GameObject g, float to, float time)
        {
            var handle = null as TweeningHandle;
            handle = ProtaTweeningManager.instance.New(TweeningType.ColorG, g, ColorG).SetDuration(time).RecordTime();
            return handle;
        }

        public static TweeningHandle TweenColorB(this GameObject g, float to, float time)
        {
            var handle = null as TweeningHandle;
            handle = ProtaTweeningManager.instance.New(TweeningType.ColorB, g, ColorB).SetDuration(time).RecordTime();
            return handle;
        }
        
        public static TweenComposedColor TweenMove(this GameObject g, Vector3 to, float time)
        {
            return new TweenComposedColor(
                TweenColorR(g, to.x, time),
                TweenColorG(g, to.y, time),
                TweenColorB(g, to.z, time)
            );
        }
        
        
        
        
        public static TweeningHandle TweenTrackingColorR(this GameObject g, GameObject target, float time)
        {
            var handle = null as TweeningHandle;
            handle = ProtaTweeningManager.instance.New(TweeningType.ColorR, g, TrackingColorR).SetDuration(time).RecordTime().SetCustomData(target);
            
            return handle;
        }
        
        public static TweeningHandle TweenTrackingColorG(this GameObject g, GameObject target, float time)
        {
            var handle = null as TweeningHandle;
            handle = ProtaTweeningManager.instance.New(TweeningType.ColorG, g, TrackingColorG).SetDuration(time).RecordTime().SetCustomData(target);
            return handle;
        }

        public static TweeningHandle TweenTrackingColorB(this GameObject g, GameObject target, float time)
        {
            var handle = null as TweeningHandle;
            handle = ProtaTweeningManager.instance.New(TweeningType.ColorB, g, TrackingColorB).SetDuration(time).RecordTime().SetCustomData(target);
            return handle;
        }
        
        public static TweenComposedColor TweenTrackingColor(this GameObject g, GameObject target, float time)
        {
            return new TweenComposedColor(
                TweenTrackingColorR(g, target, time),
                TweenTrackingColorG(g, target, time),
                TweenTrackingColorB(g, target, time)
            );
        }
        
        
        public static GameObject ClearTweenColorR(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ColorR);
            return g;
        }
        
        public static GameObject ClearTweenColorG(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ColorG);
            return g;
        }
        
        public static GameObject ClearTweenColorB(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.ColorB);
            return g;
        }
        
        public static GameObject ClearTweenColor(this GameObject g)
        {
            g.ClearTweenColorR();
            g.ClearTweenColorG();
            g.ClearTweenColorB();
            return g;
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        public static ref TweenComposedColor SetFrom(ref this TweenComposedColor m, Vector3 from)
        {
            m.x.SetFrom(from.x);
            m.y.SetFrom(from.y);
            m.z.SetFrom(from.z);
            return ref m;
        }
        
        public static ref TweenComposedColor SetTo(ref this TweenComposedColor m, Vector3 to)
        {
            m.x.SetTo(to.x);
            m.y.SetTo(to.y);
            m.z.SetTo(to.z);
            return ref m;
        }
        
        public static ref TweenComposedColor RecordTime(ref this TweenComposedColor m, bool isRealtime)
        {
            m.x.RecordTime(isRealtime);
            m.y.RecordTime(isRealtime);
            m.z.RecordTime(isRealtime);
            return ref m;
        }
        
        public static ref TweenComposedColor SetDuration(ref this TweenComposedColor m, float duration)
        {
            m.x.SetDuration(duration);
            m.y.SetDuration(duration);
            m.z.SetDuration(duration);
            return ref m;
        }
        
        public static ref TweenComposedColor SetCurve(ref this TweenComposedColor m, AnimationCurve curve = null)
        {
            m.x.SetCurve(curve);
            m.y.SetCurve(curve);
            m.z.SetCurve(curve);
            return ref m;
        }
        
        public static ref TweenComposedColor SetGuard(ref this TweenComposedColor m, UnityEngine.Object guard)
        {
            m.x.SetGuard(guard);
            m.y.SetGuard(guard);
            m.z.SetGuard(guard);
            return ref m;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static void ColorR(TweeningHandle h, float t)
        {
            var g = h.binding;
            var val = h.Evaluate(t);
            SetColor(h.binding, GetColor(h.binding).WithR(val));
        }
        
        static void ColorG(TweeningHandle h, float t)
        {
            var g = h.binding;
            var val = h.Evaluate(t);
            SetColor(h.binding, GetColor(h.binding).WithG(val));
        }
        
        static void ColorB(TweeningHandle h, float t)
        {
            var g = h.binding;
            var val = h.Evaluate(t);
            SetColor(h.binding, GetColor(h.binding).WithB(val));
        }
        
        
        static void TrackingColorR(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo(GetColor(h.customData as GameObject).r);
            var val = h.Evaluate(t);
            SetColor(h.binding, GetColor(h.binding).WithR(val));
        }
        
        static void TrackingColorG(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo(GetColor(h.customData as GameObject).g);
            var val = h.Evaluate(t);
            SetColor(h.binding, GetColor(h.binding).WithG(val));
        }
        
        static void TrackingColorB(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo(GetColor(h.customData as GameObject).b);
            var val = h.Evaluate(t);
            SetColor(h.binding, GetColor(h.binding).WithB(val));
            
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static Color GetColor(GameObject g)
        {
            if(g.TryGetComponent<SpriteRenderer>(out var sprd))
            {
                return sprd.color;
            }
            else if(g.TryGetComponent<Graphic>(out var gg))
            {
                return gg.color;
            }
            else if(g.TryGetComponent<Renderer>(out var rr))
            {
                return rr.material.GetColor("_Color");
            }
            
            throw new Exception("no color property found!");
        }
        
        static void SetColor(GameObject g, Color color)
        {
            if(g.TryGetComponent<SpriteRenderer>(out var sprd))
            {
                sprd.color = color;
            }
            else if(g.TryGetComponent<Graphic>(out var gg))
            {
                gg.color = color;
            }
            else if(g.TryGetComponent<Renderer>(out var rr))
            {
                rr.material.SetColor("_Color", color);
            }
            
            throw new Exception("no color property found!");
        }
        
    }
    
    
}
