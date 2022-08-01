using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prota.Tweening
{
    public static class TransparencyTweening
    {
        public static TweeningHandle TweenAlpha(this GameObject g, float to, float time)
        {
            var handle = null as TweeningHandle;
            handle = ProtaTweeningManager.instance.New(TweeningType.Transparency, g, TweenTransparency).SetDuration(time).RecordTime();
            return handle;
        }
        
        
        
        
        public static TweeningHandle TweenTrackingAlpha(this GameObject g, GameObject target, float time)
        {
            var handle = null as TweeningHandle;
            handle = ProtaTweeningManager.instance.New(TweeningType.Transparency, g, TweenTransparencyTracking).SetDuration(time).RecordTime().SetCustomData(target);
            
            return handle;
        }
        
        public static GameObject ClearTweenAplha(this GameObject g)
        {
            ProtaTweeningManager.instance.Remove(g, TweeningType.Transparency);
            return g;
        }
        
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static void TweenTransparency(TweeningHandle h, float t)
        {
            var val = h.Evaluate(t);
            SetAlpha(h.binding, val);
        }
        
        
        static void TweenTransparencyTracking(TweeningHandle h, float t)
        {
            if(h.customData == null) return;
            var tr = h.binding.transform;
            h.SetTo(GetAlpha(h.customData as GameObject));
            var val = h.Evaluate(t);
            SetAlpha(h.binding, val);
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        static float GetAlpha(GameObject g)
        {
            if(g.TryGetComponent<SpriteRenderer>(out var sprd))
            {
                return sprd.color.a;
            }
            else if(g.TryGetComponent<Graphic>(out var gg))
            {
                return gg.color.a;
            }
            else if(g.TryGetComponent<Renderer>(out var rr))
            {
                return rr.material.GetColor("_Color").a;
            }
            else if(g.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                return canvasGroup.alpha;
            }
            
            throw new Exception("no color property found!");
        }
        
        static void SetAlpha(GameObject g, float alpha)
        {
            if(g.TryGetComponent<SpriteRenderer>(out var sprd))
            {
                sprd.color = sprd.color.WithA(alpha);
            }
            else if(g.TryGetComponent<Graphic>(out var gg))
            {
                gg.color = gg.color.WithA(alpha);
            }
            else if(g.TryGetComponent<Renderer>(out var rr))
            {
                rr.material.SetColor("_Color", rr.material.GetColor("_Color").WithA(alpha));
            }
            else if(g.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.alpha = alpha;
            }
            
            throw new Exception("no color property found!");
        }
        
    }
    
    
}
