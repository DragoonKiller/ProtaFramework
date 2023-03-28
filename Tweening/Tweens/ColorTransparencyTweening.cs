using System;
using Prota.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Prota.Tween
{
    public static class ColorTweening
    {
        public static TweenHandle TweenColorR(this SpriteRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorR, g, ColorR)
                .SetGuard(g.LifeSpan()).SetFromTo(g.color.r, to).Start(time);
        
        public static TweenHandle TweenColorG(this SpriteRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorG, g, ColorG)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.g, to).Start(time);
        
        public static TweenHandle TweenColorB(this SpriteRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorB, g, ColorB)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.b, to).Start(time);
        
        public static TweenHandle TweenColorA(this SpriteRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.Transparency, g, ColorA)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.a, to).Start(time);
        
        public static ComposedHandle TweenColor(this SpriteRenderer g, Color to, float time, bool includeTransparency = false)
            => ComposedHandle.Make(
                "r", TweenColorR(g, to.r, time),
                "g", TweenColorG(g, to.g, time),
                "b", TweenColorB(g, to.b, time),
                "a", includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        // ====================================================================================================
        // Material
        // ====================================================================================================
        
        public static TweenHandle TweenColorR(this Material g, GameObject guard, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorR, g, ColorR)
            .SetGuard(guard.LifeSpan()).SetFromTo(g.color.r, to).Start(time);
        
        public static TweenHandle TweenColorG(this Material g, GameObject guard, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorG, g, ColorG)
            .SetGuard(guard.LifeSpan()).SetFromTo(g.color.g, to).Start(time);
            
        public static TweenHandle TweenColorB(this Material g, GameObject guard, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorB, g, ColorB)
            .SetGuard(guard.LifeSpan()).SetFromTo(g.color.b, to).Start(time);
            
        public static TweenHandle TweenColorA(this Material g, GameObject guard, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.Transparency, g, ColorA)
            .SetGuard(guard.LifeSpan()).SetFromTo(g.color.a, to).Start(time);
            
        public static ComposedHandle TweenColor(this Material g, GameObject guard, Color to, float time, bool includeTransparency = false)
            => ComposedHandle.Make(
                "r", TweenColorR(g, guard, to.r, time),
                "g", TweenColorG(g, guard, to.g, time),
                "b", TweenColorB(g, guard, to.b, time),
                "a", includeTransparency ? TweenColorA(g, guard, to.a, time) : TweenHandle.none
            );
        
        // ====================================================================================================
        // Mesh Renderer
        // ====================================================================================================
        
        public static TweenHandle TweenColorR(this MeshRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorR, g, ColorR)
            .SetGuard(g.LifeSpan()).SetFromTo(g.GetMaterialInstance().color.r, to).Start(time);
        
        public static TweenHandle TweenColorG(this MeshRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorG, g, ColorG)
            .SetGuard(g.LifeSpan()).SetFromTo(g.GetMaterialInstance().color.g, to).Start(time);
            
        public static TweenHandle TweenColorB(this MeshRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorB, g, ColorB)
            .SetGuard(g.LifeSpan()).SetFromTo(g.GetMaterialInstance().color.b, to).Start(time);
        
        public static TweenHandle TweenColorA(this MeshRenderer g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.Transparency, g, ColorA)
            .SetGuard(g.LifeSpan()).SetFromTo(g.GetMaterialInstance().color.a, to).Start(time);
            
        public static ComposedHandle TweenColor(this MeshRenderer g, Color to, float time, bool includeTransparency = false)
            => ComposedHandle.Make(
                "r", TweenColorR(g, to.r, time),
                "g", TweenColorG(g, to.g, time),
                "b", TweenColorB(g, to.b, time),
                "a", includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        
        // ============================================================================================================
        // Image
        // ============================================================================================================
        
        public static TweenHandle TweenColorR(this Image g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorR, g, ColorR)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.r, to).Start(time);
            
        public static TweenHandle TweenColorG(this Image g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorG, g, ColorG)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.g, to).Start(time);
            
        public static TweenHandle TweenColorB(this Image g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorB, g, ColorB)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.b, to).Start(time);
            
        public static TweenHandle TweenColorA(this Image g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.Transparency, g, ColorA)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.a, to).Start(time);
            
        public static ComposedHandle TweenColor(this Image g, Color to, float time, bool includeTransparency = false)
            => ComposedHandle.Make(
                "r", TweenColorR(g, to.r, time),
                "g", TweenColorG(g, to.g, time),
                "b", TweenColorB(g, to.b, time),
                "a", includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        // ============================================================================================================
        // RawImage
        // ============================================================================================================
        
        public static TweenHandle TweenColorR(this RawImage g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorR, g, ColorR)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.r, to).Start(time);
            
        public static TweenHandle TweenColorG(this RawImage g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorG, g, ColorG)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.g, to).Start(time);
            
        public static TweenHandle TweenColorB(this RawImage g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorB, g, ColorB)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.b, to).Start(time);
            
        public static TweenHandle TweenColorA(this RawImage g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.Transparency, g, ColorA)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.a, to).Start(time);
            
        public static ComposedHandle TweenColor(this RawImage g, Color to, float time, bool includeTransparency = false)    
            => ComposedHandle.Make(
                "r", TweenColorR(g, to.r, time),
                "g", TweenColorG(g, to.g, time),
                "b", TweenColorB(g, to.b, time),
                "a", includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
            
        
        // ============================================================================================================
        // Text
        // ============================================================================================================
        
        public static TweenHandle TweenColorR(this Text g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorR, g, ColorR)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.r, to).Start(time);
            
        public static TweenHandle TweenColorG(this Text g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorG, g, ColorG)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.g, to).Start(time);
            
        public static TweenHandle TweenColorB(this Text g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.ColorB, g, ColorB)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.b, to).Start(time);
            
        public static TweenHandle TweenColorA(this Text g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.Transparency, g, ColorA)
            .SetGuard(g.LifeSpan()).SetFromTo(g.color.a, to).Start(time);
            
        public static ComposedHandle TweenColor(this Text g, Color to, float time, bool includeTransparency = false)
            => ComposedHandle.Make(
                "r", TweenColorR(g, to.r, time),
                "g", TweenColorG(g, to.g, time),
                "b", TweenColorB(g, to.b, time),
                "a", includeTransparency ? TweenColorA(g, to.a, time) : TweenHandle.none
            );
        
        // ============================================================================================================
        // CanvasGroup
        // ============================================================================================================
        
        public static TweenHandle TweenTransparency(this CanvasGroup g, float to, float time)
            => ProtaTweenManager.instance.New(TweenId.Transparency, g, CanvasGroupTweenTransparency)
            .SetGuard(g.LifeSpan()).SetFromTo(g.alpha, to).Start(time);
        
        static void CanvasGroupTweenTransparency(TweenHandle h, float r)
        {
            var res = (CanvasGroup)h.target;
            res.alpha = h.Evaluate(r);
        }
        public static void ClearTweenTransparency(this CanvasGroup g)
            => ProtaTweenManager.instance.Remove(g, TweenId.Transparency);
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static void ColorR(TweenHandle h, float t)
        {
            switch(h.target)
            {
                case Graphic g:
                    g.color = g.color.WithR(h.Evaluate(t));
                    break;
                
                case SpriteRenderer sr:
                    sr.color = sr.color.WithR(h.Evaluate(t));
                    break;
                
                case MeshRenderer mr:
                    var mat = mr.GetMaterialInstance();
                    mat.color = mat.color.WithR(h.Evaluate(t));
                    break;
            }
        }
        
        static void ColorG(TweenHandle h, float t)
        {
            switch(h.target)
            {
                case Graphic g:
                    g.color = g.color.WithG(h.Evaluate(t));
                    break;
                
                case SpriteRenderer sr:
                    sr.color = sr.color.WithG(h.Evaluate(t));
                    break;
                
                case MeshRenderer mr:
                    var mat = mr.GetMaterialInstance();
                    mat.color = mat.color.WithG(h.Evaluate(t));
                    break;
            }
        }
        
        static void ColorB(TweenHandle h, float t)
        {
            switch(h.target)
            {
                case Graphic g:
                    g.color = g.color.WithB(h.Evaluate(t));
                    break;
                
                case SpriteRenderer sr:
                    sr.color = sr.color.WithB(h.Evaluate(t));
                    break;
                
                case MeshRenderer mr:
                    var mat = mr.GetMaterialInstance();
                    mat.color = mat.color.WithB(h.Evaluate(t));
                    break;
            }
        }
        
        static void ColorA(TweenHandle h, float t)
        {
            switch(h.target)
            {
                case Graphic g:
                    g.color = g.color.WithA(h.Evaluate(t));
                    break;
                
                case SpriteRenderer sr:
                    sr.color = sr.color.WithA(h.Evaluate(t));
                    break;
                
                case MeshRenderer mr:
                    var mat = mr.GetMaterialInstance();
                    mat.color = mat.color.WithA(h.Evaluate(t));
                    break;
            }
        }
    }
    
    
}
