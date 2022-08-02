using System;
using Prota.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Prota.Tweening
{
    public struct TweenComposedColor
    {
        public readonly TweeningHandle r;
        public readonly TweeningHandle g;
        public readonly TweeningHandle b;
        public readonly TweeningHandle a;

        public TweenComposedColor(TweeningHandle x, TweeningHandle y, TweeningHandle z, TweeningHandle a)
        {
            this.r = x;
            this.g = y;
            this.b = z;
            this.a = a;
        }
    }
    
    
    
    public static class ColorTweening
    {
        // ============================================================================================================
        // SpriteRenderer
        // ============================================================================================================
        
        public static TweeningHandle TweenColorR(this SpriteRenderer g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorR, g, SpriteRendererColorR).SetDuration(time).RecordTime();
        
        static void SpriteRendererColorR(TweeningHandle h, float r)
        {
            var res = (SpriteRenderer)h.target;
            res.color = res.color.WithR(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorG(this SpriteRenderer g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorG, g, SpriteRendererColorG).SetDuration(time).RecordTime();
        
        static void SpriteRendererColorG(TweeningHandle h, float r)
        {
            var res = (SpriteRenderer)h.target;
            res.color = res.color.WithG(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorB(this SpriteRenderer g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorB, g, SpriteRendererColorB).SetDuration(time).RecordTime();
        
        static void SpriteRendererColorB(TweeningHandle h, float r)
        {
            var res = (SpriteRenderer)h.target;
            res.color = res.color.WithB(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorA(this SpriteRenderer g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.Transparency, g, SpriteRendererColorA).SetDuration(time).RecordTime();
        
        static void SpriteRendererColorA(TweeningHandle h, float r)
        {
            var res = (SpriteRenderer)h.target;
            res.color = res.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this SpriteRenderer g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : null
            );
        
        
        
        public static void ClearTweenColorR(this SpriteRenderer g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorR);
        
        public static void ClearTweenColorG(this SpriteRenderer g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorG);
        
        public static void ClearTweenColorB(this SpriteRenderer g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorB);
        
        public static void ClearTweenColorA(this SpriteRenderer g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.Transparency);
        
        public static SpriteRenderer ClearTweenColor(this SpriteRenderer g)
        {
            g.ClearTweenColorR();
            g.ClearTweenColorG();
            g.ClearTweenColorB();
            g.ClearTweenColorA();
            return g;
        }
        
        
        // ============================================================================================================
        // Material
        // ============================================================================================================
        
        
        public static TweeningHandle TweenColorR(this Material g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorR, g, MaterialColorR).SetDuration(time).RecordTime();
        
        static void MaterialColorR(TweeningHandle h, float r)
        {
            var res = (Material)h.target;
            res.color = res.color.WithR(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorG(this Material g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorG, g, MaterialColorG).SetDuration(time).RecordTime();
        
        static void MaterialColorG(TweeningHandle h, float r)
        {
            var res = (Material)h.target;
            res.color = res.color.WithG(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorB(this Material g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorB, g, MaterialColorB).SetDuration(time).RecordTime();
        
        static void MaterialColorB(TweeningHandle h, float r)
        {
            var res = (Material)h.target;
            res.color = res.color.WithB(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorA(this Material g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.Transparency, g, MaterialColorA).SetDuration(time).RecordTime();
        
        static void MaterialColorA(TweeningHandle h, float r)
        {
            var res = (Material)h.target;
            res.color = res.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this Material g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : null
            );
        
        
        
        public static void ClearTweenColorR(this Material g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorR);
        
        public static void ClearTweenColorG(this Material g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorG);
        
        public static void ClearTweenColorB(this Material g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorB);
        
        public static void ClearTweenColorA(this Material g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.Transparency);
        
        public static Material ClearTweenColor(this Material g)
        {
            g.ClearTweenColorR();
            g.ClearTweenColorG();
            g.ClearTweenColorB();
            g.ClearTweenColorA();
            return g;
        }
        
        // ============================================================================================================
        // MeshRenderer
        // ============================================================================================================
        
        
        public static TweeningHandle TweenColorR(this MeshRenderer g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorR, g, MeshRendererColorR).SetDuration(time).RecordTime();
        
        static void MeshRendererColorR(TweeningHandle h, float r)
        {
            var res = (MeshRenderer)h.target;
            var mats = res.GetMaterialInstances();
            foreach(var mat in mats) mat.color = mat.color.WithR(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorG(this MeshRenderer g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorG, g, MeshRendererColorG).SetDuration(time).RecordTime();
        
        static void MeshRendererColorG(TweeningHandle h, float r)
        {
            var res = (MeshRenderer)h.target;
            var mats = res.GetMaterialInstances();
            foreach(var mat in mats) mat.color = mat.color.WithG(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorB(this MeshRenderer g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorB, g, MeshRendererColorB).SetDuration(time).RecordTime();
        
        static void MeshRendererColorB(TweeningHandle h, float r)
        {
            var res = (MeshRenderer)h.target;
            var mats = res.GetMaterialInstances();
            foreach(var mat in mats) mat.color = mat.color.WithB(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorA(this MeshRenderer g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.Transparency, g, MeshRendererColorA).SetDuration(time).RecordTime();
        
        static void MeshRendererColorA(TweeningHandle h, float r)
        {
            var res = (MeshRenderer)h.target;
            var mats = res.GetMaterialInstances();
            foreach(var mat in mats) mat.color = mat.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this MeshRenderer g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : null
            );
        
        
        
        public static void ClearTweenColorR(this MeshRenderer g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorR);
        
        public static void ClearTweenColorG(this MeshRenderer g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorG);
        
        public static void ClearTweenColorB(this MeshRenderer g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorB);
        
        public static void ClearTweenColorA(this MeshRenderer g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.Transparency);
        
        public static MeshRenderer ClearTweenColor(this MeshRenderer g)
        {
            g.ClearTweenColorR();
            g.ClearTweenColorG();
            g.ClearTweenColorB();
            g.ClearTweenColorA();
            return g;
        }
        
        
        // ============================================================================================================
        // Image
        // ============================================================================================================
        
        public static TweeningHandle TweenColorR(this Image g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorR, g, ImageColorR).SetDuration(time).RecordTime();
        
        static void ImageColorR(TweeningHandle h, float r)
        {
            var res = (Image)h.target;
            res.color = res.color.WithR(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorG(this Image g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorG, g, ImageColorG).SetDuration(time).RecordTime();
        
        static void ImageColorG(TweeningHandle h, float r)
        {
            var res = (Image)h.target;
            res.color = res.color.WithG(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorB(this Image g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorB, g, ImageColorB).SetDuration(time).RecordTime();
        
        static void ImageColorB(TweeningHandle h, float r)
        {
            var res = (Image)h.target;
            res.color = res.color.WithB(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorA(this Image g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.Transparency, g, ImageColorA).SetDuration(time).RecordTime();
        
        static void ImageColorA(TweeningHandle h, float r)
        {
            var res = (Image)h.target;
            res.color = res.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this Image g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : null
            );
        
        
        
        public static void ClearTweenColorR(this Image g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorR);
        
        public static void ClearTweenColorG(this Image g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorG);
        
        public static void ClearTweenColorB(this Image g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorB);
        
        public static void ClearTweenColorA(this Image g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.Transparency);
        
        public static Image ClearTweenColor(this Image g)
        {
            g.ClearTweenColorR();
            g.ClearTweenColorG();
            g.ClearTweenColorB();
            g.ClearTweenColorA();
            return g;
        }
        
        
        // ============================================================================================================
        // RawImage
        // ============================================================================================================
        
        public static TweeningHandle TweenColorR(this RawImage g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorR, g, RawImageColorR).SetDuration(time).RecordTime();
        
        static void RawImageColorR(TweeningHandle h, float r)
        {
            var res = (RawImage)h.target;
            res.color = res.color.WithR(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorG(this RawImage g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorG, g, RawImageColorG).SetDuration(time).RecordTime();
        
        static void RawImageColorG(TweeningHandle h, float r)
        {
            var res = (RawImage)h.target;
            res.color = res.color.WithG(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorB(this RawImage g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.ColorB, g, RawImageColorB).SetDuration(time).RecordTime();
        
        static void RawImageColorB(TweeningHandle h, float r)
        {
            var res = (RawImage)h.target;
            res.color = res.color.WithB(h.Evaluate(r));
        }
        
        public static TweeningHandle TweenColorA(this RawImage g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.Transparency, g, RawImageColorA).SetDuration(time).RecordTime();
        
        static void RawImageColorA(TweeningHandle h, float r)
        {
            var res = (RawImage)h.target;
            res.color = res.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this RawImage g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : null
            );
        
        
        
        public static void ClearTweenColorR(this RawImage g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorR);
        
        public static void ClearTweenColorG(this RawImage g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorG);
        
        public static void ClearTweenColorB(this RawImage g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.ColorB);
        
        public static void ClearTweenColorA(this RawImage g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.Transparency);
        
        public static RawImage ClearTweenColor(this RawImage g)
        {
            g.ClearTweenColorR();
            g.ClearTweenColorG();
            g.ClearTweenColorB();
            g.ClearTweenColorA();
            return g;
        }
        
        
        // ============================================================================================================
        // CanvasGroup
        // ============================================================================================================
        
        public static TweeningHandle TweenTransparency(this CanvasGroup g, float to, float time)
            => ProtaTweeningManager.instance.New(TweeningType.Transparency, g, CanvasGroupTweenTransparency).SetDuration(time).RecordTime();
        
        static void CanvasGroupTweenTransparency(TweeningHandle h, float r)
        {
            var res = (CanvasGroup)h.target;
            res.alpha = h.Evaluate(r);
        }
        public static void ClearTweenTransparency(this CanvasGroup g)
            => ProtaTweeningManager.instance.Remove(g, TweeningType.Transparency);
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        public static ref TweenComposedColor SetFrom(ref this TweenComposedColor m, Color from)
        {
            m.r.SetFrom(from.r);
            m.g.SetFrom(from.g);
            m.b.SetFrom(from.b);
            m.a?.SetFrom(from.a);
            return ref m;
        }
        
        public static ref TweenComposedColor SetTo(ref this TweenComposedColor m, Color to)
        {
            m.r.SetTo(to.r);
            m.g.SetTo(to.g);
            m.b.SetTo(to.b);
            m.a?.SetTo(to.a);
            return ref m;
        }
        
        public static ref TweenComposedColor RecordTime(ref this TweenComposedColor m, bool isRealtime)
        {
            m.r.RecordTime(isRealtime);
            m.g.RecordTime(isRealtime);
            m.b.RecordTime(isRealtime);
            m.a?.RecordTime(isRealtime);
            return ref m;
        }
        
        public static ref TweenComposedColor SetDuration(ref this TweenComposedColor m, float duration)
        {
            m.r.SetDuration(duration);
            m.g.SetDuration(duration);
            m.b.SetDuration(duration);
            m.a?.SetDuration(duration);
            return ref m;
        }
        
        public static ref TweenComposedColor SetCurve(ref this TweenComposedColor m, AnimationCurve curve = null)
        {
            m.r.SetCurve(curve);
            m.g.SetCurve(curve);
            m.b.SetCurve(curve);
            m.a?.SetCurve(curve);
            return ref m;
        }
        
        public static ref TweenComposedColor SetGuard(ref this TweenComposedColor m, UnityEngine.Object guard)
        {
            m.r.SetGuard(guard);
            m.g.SetGuard(guard);
            m.b.SetGuard(guard);
            m.a?.SetGuard(guard);
            return ref m;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        
    }
    
    
}
