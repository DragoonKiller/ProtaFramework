using System;
using UnityEngine;

namespace Prota.Animation
{
    public sealed class Tweening : IDisposable
    {
        public UnityEngine.Object guard { get; private set; } = null;
        
        public UnityEngine.Object target { get; private set; } = null;
        
        public float ratio { get; set; } = 0;
        
        public float duration { get; private set; } = 0;
        
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        
        public UpdateMode updateMode { get; private set; } = UpdateMode.Update;
        
        event Action<float, bool> onUpdate;
        
        public bool valid
        {
            get
            {
                if(!ratio.In(0, 1)) return false;
                if(!guard) return false;
                return true;
            }
        }
        
        public void Update(float dt)
        {
            ratio += dt / duration;
            try
            {
                onUpdate?.Invoke(curve.Evaluate(ratio), ratio >= 1);
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }
        
        public Tweening Guard(UnityEngine.Object g)
        {
            if(this.guard != null) throw new Exception();
            this.guard = g;
            return this;
        }
        
        public Tweening OnUpdate(Action<float> a)
        {
            
            onUpdate += (t, complete) => a(t);
            return this;
        }
        
        public Tweening OnComplete(Action a)
        {
            onUpdate += (t, complete) => {
                if(complete) a();
            };
            return this;
        }
        
        public Tweening Mode(UpdateMode mode)
        {
            this.updateMode = mode;
            return this;
        }
        
        public Tweening Target(Transform target) => this.Target(target.gameObject);
        
        public Tweening Target(GameObject target)
        {
            if(this.target != null) throw new Exception();
            this.target = target;
            CodeAnimationManager.instance.Bind(target, this);
            return this;
        }
        
        Tweening() { }
        
        public static Tweening New(float t)
        {
            var res = new Tweening(){ duration = t };
            CodeAnimationManager.instance.Add(res);
            return res;
        }
        
        public void Dispose()
        {
            if(!valid) return;
            CodeAnimationManager.instance.Remove(this);
        }
    }
    
    
    public static class TweenExt
    {
        public static Tweening Move(this Transform t, float d, Vector3 to)
        {
            var from = t.position;
            return Tweening.New(d)
                .Guard(t)
                .OnUpdate(x => t.position = (from, to).Lerp(x))
                .OnComplete(() => t.position = to)
                .Target(t);
        }
        
        public static Tweening MoveLocal(this Transform t, float d, Vector3 to)
        {
            var from = t.localPosition;
            return Tweening.New(d)
                .Guard(t)
                .OnUpdate(x => t.localPosition = (from, to).Lerp(x))
                .OnComplete(() => t.localPosition = to)
                .Target(t);
        }
        
        public static Tweening MoveVec2(this Transform t, float d, Vector2 to)
        {
            var from = t.position.ToVec2();
            return Tweening.New(d)
                .Guard(t)
                .OnUpdate(x => t.position = (from, to).Lerp(x).ToVec3(t.position.z))
                .OnComplete(() => t.position = to.ToVec3(t.position.z))
                .Target(t);
        }
        
        public static Tweening MoveLocalVec2(this Transform t, float d, Vector2 to)
        {
            var from = t.localPosition.ToVec2();
            return Tweening.New(d)
                .Guard(t)
                .OnUpdate(x => t.localPosition = (from, to).Lerp(x).ToVec3(t.localPosition.z))
                .OnComplete(() => t.localPosition = to.ToVec3(t.localPosition.z))
                .Target(t);
        }
        
        public static Tweening MoveAnchored(this RectTransform t, float d, Vector2 to)
        {
            var from = t.anchoredPosition;
            return Tweening.New(d)
                .Guard(t)
                .OnUpdate(x => t.anchoredPosition = (from, to).Lerp(x))
                .OnComplete(() => t.anchoredPosition = to)
                .Target(t);
        }
        
        public static GameObject KillAllTween(this GameObject g)
        {
            CodeAnimationManager.instance.KillAll(g);
            return g;
        }
        
        public static T KillAllTween<T>(this T c) where T: Component
        {
            CodeAnimationManager.instance.KillAll(c.gameObject);
            return c;
        }
    }
    
}