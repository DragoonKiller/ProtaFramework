using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tween
{
    public struct TweenData
    {
        public UnityEngine.Object target;       // tween 所改变的物体.
        public TweenId tid;                     // 标记这个 tween 在改变物体的什么内容.
        public ValueTweeningUpdate update;      // 更新函数.
        
        public Action<TweenHandle> onFinish;
        public Action<TweenHandle> onInterrupted;
        public Action<TweenHandle> onRemove;
        
        public object customData;
        
        public float from;
        public float to;
        public AnimationCurve curve;
        public TweenEase ease;
        public float timeFrom;
        public float timeTo;
        public bool realtime;
        public LifeSpan guard;
        
        public bool isTimeout => timeTo < (realtime ? Time.realtimeSinceStartup : Time.time);
        public bool invalid => target == null || (guard != null && !guard.alive) || update == null; // 已考虑 target 被 destroy 但是引用还在.
        public bool valid => !invalid;
        
        public TweenHandle handle { get; private set; }
        
        // Sample ratio on ease/curve.
        public float EvaluateRatio(float ratio)
        {
            return ease.valid ? ease.Evaluate(ratio) : curve?.Evaluate(ratio) ?? ratio;
        }
        
        // Actual value interpolation.
        public float Evaluate(float ratio)
        {
            return (from, to).Lerp(EvaluateRatio(ratio));
        }
        
        // Current time ratio in [0, 1].
        public float GetTimeLerp()
        {
            return (timeFrom, timeTo).InvLerp(realtime ? Time.realtimeSinceStartup : Time.time);
        }
        
        // a handle cache.
        internal void SetHandle(TweenHandle handle)
        {
            this.handle = handle;
        }
        
    }
    
}