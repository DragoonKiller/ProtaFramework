using System;
using Prota.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Prota.Tween
{
    public struct TweenComposedColor
    {
        public readonly TweenHandle r;
        public readonly TweenHandle g;
        public readonly TweenHandle b;
        public readonly TweenHandle a;

        public TweenComposedColor(TweenHandle x, TweenHandle y, TweenHandle z, TweenHandle a)
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
        
        public static TweenHandle TweenColorR(this SpriteRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorR, g, SpriteRendererColorR).SetFrom(g.color.r).SetTo(to).Start(time);
        
        static void SpriteRendererColorR(TweenHandle h, float r)
        {
            var res = (SpriteRenderer)h.target;
            res.color = res.color.WithR(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorG(this SpriteRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorG, g, SpriteRendererColorG).SetFrom(g.color.g).SetTo(to).Start(time);
        
        static void SpriteRendererColorG(TweenHandle h, float r)
        {
            var res = (SpriteRenderer)h.target;
            res.color = res.color.WithG(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorB(this SpriteRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorB, g, SpriteRendererColorB).SetFrom(g.color.b).SetTo(to).Start(time);
        
        static void SpriteRendererColorB(TweenHandle h, float r)
        {
            var res = (SpriteRenderer)h.target;
            res.color = res.color.WithB(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorA(this SpriteRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.Transparency, g, SpriteRendererColorA).SetFrom(g.color.a).SetTo(to).SetTo(to).Start(time);
        
        static void SpriteRendererColorA(TweenHandle h, float r)
        {
            var res = (SpriteRenderer)h.target;
            res.color = res.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this SpriteRenderer g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        
        
        public static void ClearTweenColorR(this SpriteRenderer g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorR);
        
        public static void ClearTweenColorG(this SpriteRenderer g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorG);
        
        public static void ClearTweenColorB(this SpriteRenderer g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorB);
        
        public static void ClearTweenColorA(this SpriteRenderer g)
            => ProtaTweenManager.instance.Remove(g, TweenType.Transparency);
        
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
        
        
        public static TweenHandle TweenColorR(this Material g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorR, g, MaterialColorR).SetFrom(g.color.r).SetTo(to).Start(time);
        
        static void MaterialColorR(TweenHandle h, float r)
        {
            var res = (Material)h.target;
            res.color = res.color.WithR(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorG(this Material g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorG, g, MaterialColorG).SetFrom(g.color.g).SetTo(to).Start(time);
        
        static void MaterialColorG(TweenHandle h, float r)
        {
            var res = (Material)h.target;
            res.color = res.color.WithG(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorB(this Material g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorB, g, MaterialColorB).SetFrom(g.color.b).SetTo(to).Start(time);
        
        static void MaterialColorB(TweenHandle h, float r)
        {
            var res = (Material)h.target;
            res.color = res.color.WithB(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorA(this Material g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.Transparency, g, MaterialColorA).SetFrom(g.color.a).SetTo(to).Start(time);
        
        static void MaterialColorA(TweenHandle h, float r)
        {
            var res = (Material)h.target;
            res.color = res.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this Material g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        
        
        public static void ClearTweenColorR(this Material g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorR);
        
        public static void ClearTweenColorG(this Material g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorG);
        
        public static void ClearTweenColorB(this Material g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorB);
        
        public static void ClearTweenColorA(this Material g)
            => ProtaTweenManager.instance.Remove(g, TweenType.Transparency);
        
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
        
        
        public static TweenHandle TweenColorR(this MeshRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorR, g, MeshRendererColorR).SetFrom(g.material.color.r).SetTo(to).Start(time);
        
        static void MeshRendererColorR(TweenHandle h, float r)
        {
            var res = (MeshRenderer)h.target;
            var mats = res.GetMaterialInstances();
            foreach(var mat in mats) mat.color = mat.color.WithR(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorG(this MeshRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorG, g, MeshRendererColorG).SetFrom(g.material.color.g).SetTo(to).Start(time);
        
        static void MeshRendererColorG(TweenHandle h, float r)
        {
            var res = (MeshRenderer)h.target;
            var mats = res.GetMaterialInstances();
            foreach(var mat in mats) mat.color = mat.color.WithG(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorB(this MeshRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorB, g, MeshRendererColorB).SetFrom(g.material.color.b).SetTo(to).Start(time);
        
        static void MeshRendererColorB(TweenHandle h, float r)
        {
            var res = (MeshRenderer)h.target;
            var mats = res.GetMaterialInstances();
            foreach(var mat in mats) mat.color = mat.color.WithB(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorA(this MeshRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.Transparency, g, MeshRendererColorA).SetFrom(g.material.color.a).SetTo(to).Start(time);
        
        static void MeshRendererColorA(TweenHandle h, float r)
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
                includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        
        
        public static void ClearTweenColorR(this MeshRenderer g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorR);
        
        public static void ClearTweenColorG(this MeshRenderer g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorG);
        
        public static void ClearTweenColorB(this MeshRenderer g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorB);
        
        public static void ClearTweenColorA(this MeshRenderer g)
            => ProtaTweenManager.instance.Remove(g, TweenType.Transparency);
        
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
        
        public static TweenHandle TweenColorR(this Image g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorR, g, ImageColorR).SetFrom(g.color.r).SetTo(to).Start(time);
        
        static void ImageColorR(TweenHandle h, float r)
        {
            var res = (Image)h.target;
            res.color = res.color.WithR(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorG(this Image g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorG, g, ImageColorG).SetFrom(g.color.g).SetTo(to).Start(time);
        
        static void ImageColorG(TweenHandle h, float r)
        {
            var res = (Image)h.target;
            res.color = res.color.WithG(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorB(this Image g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorB, g, ImageColorB).SetFrom(g.color.b).SetTo(to).Start(time);
        
        static void ImageColorB(TweenHandle h, float r)
        {
            var res = (Image)h.target;
            res.color = res.color.WithB(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorA(this Image g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.Transparency, g, ImageColorA).SetFrom(g.color.a).SetTo(to).Start(time);
        
        static void ImageColorA(TweenHandle h, float r)
        {
            var res = (Image)h.target;
            res.color = res.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this Image g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        
        
        public static void ClearTweenColorR(this Image g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorR);
        
        public static void ClearTweenColorG(this Image g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorG);
        
        public static void ClearTweenColorB(this Image g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorB);
        
        public static void ClearTweenColorA(this Image g)
            => ProtaTweenManager.instance.Remove(g, TweenType.Transparency);
        
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
        
        public static TweenHandle TweenColorR(this RawImage g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorR, g, RawImageColorR).SetFrom(g.color.r).SetTo(to).Start(time);
        
        static void RawImageColorR(TweenHandle h, float r)
        {
            var res = (RawImage)h.target;
            res.color = res.color.WithR(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorG(this RawImage g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorG, g, RawImageColorG).SetFrom(g.color.g).SetTo(to).Start(time);
        
        static void RawImageColorG(TweenHandle h, float r)
        {
            var res = (RawImage)h.target;
            res.color = res.color.WithG(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorB(this RawImage g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.ColorB, g, RawImageColorB).SetFrom(g.color.b).SetTo(to).Start(time);
        
        static void RawImageColorB(TweenHandle h, float r)
        {
            var res = (RawImage)h.target;
            res.color = res.color.WithB(h.Evaluate(r));
        }
        
        public static TweenHandle TweenColorA(this RawImage g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.Transparency, g, RawImageColorA).SetFrom(g.color.a).SetTo(to).Start(time);
        
        static void RawImageColorA(TweenHandle h, float r)
        {
            var res = (RawImage)h.target;
            res.color = res.color.WithA(h.Evaluate(r));
        }
        
        public static TweenComposedColor TweenMove(this RawImage g, Color to, float time, bool includeTransparency)
            => new TweenComposedColor(
                TweenColorR(g, to.r, time),
                TweenColorG(g, to.g, time),
                TweenColorB(g, to.b, time),
                includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        
        
        public static void ClearTweenColorR(this RawImage g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorR);
        
        public static void ClearTweenColorG(this RawImage g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorG);
        
        public static void ClearTweenColorB(this RawImage g)
            => ProtaTweenManager.instance.Remove(g, TweenType.ColorB);
        
        public static void ClearTweenColorA(this RawImage g)
            => ProtaTweenManager.instance.Remove(g, TweenType.Transparency);
        
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
        
        public static TweenHandle TweenTransparency(this CanvasGroup g, float to, float time)
            => ProtaTweenManager.instance.New(TweenType.Transparency, g, CanvasGroupTweenTransparency).SetFrom(g.alpha).SetTo(to).Start(time);
        
        static void CanvasGroupTweenTransparency(TweenHandle h, float r)
        {
            var res = (CanvasGroup)h.target;
            res.alpha = h.Evaluate(r);
        }
        public static void ClearTweenTransparency(this CanvasGroup g)
            => ProtaTweenManager.instance.Remove(g, TweenType.Transparency);
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        public static ref TweenComposedColor SetFrom(ref this TweenComposedColor m, Color from)
        {
            m.r.SetFrom(from.r);
            m.g.SetFrom(from.g);
            m.b.SetFrom(from.b);
            if(!m.a.isNone) m.a.SetFrom(from.a);
            return ref m;
        }
        
        public static ref TweenComposedColor SetTo(ref this TweenComposedColor m, Color to)
        {
            m.r.SetTo(to.r);
            m.g.SetTo(to.g);
            m.b.SetTo(to.b);
            if(!m.a.isNone) m.a.SetTo(to.a);
            return ref m;
        }
        
        public static ref TweenComposedColor SetDuration(ref this TweenComposedColor m, float duration, bool realtime = false)
        {
            m.r.Start(duration, realtime);
            m.g.Start(duration, realtime);
            m.b.Start(duration, realtime);
            if(!m.a.isNone) m.a.Start(duration, realtime);
            return ref m;
        }
        
        public static ref TweenComposedColor SetCurve(ref this TweenComposedColor m, AnimationCurve curve = null)
        {
            m.r.SetCurve(curve);
            m.g.SetCurve(curve);
            m.b.SetCurve(curve);
            if(!m.a.isNone) m.a.SetCurve(curve);
            return ref m;
        }
        
        public static ref TweenComposedColor SetGuard(ref this TweenComposedColor m, UnityEngine.Object guard)
        {
            m.r.SetGuard(guard);
            m.g.SetGuard(guard);
            m.b.SetGuard(guard);
            if(!m.a.isNone) m.a.SetGuard(guard);
            return ref m;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        
    }
    
    
}
