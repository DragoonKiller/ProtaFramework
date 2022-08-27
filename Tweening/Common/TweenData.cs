using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    public struct TweenData
    {
        public UnityEngine.Object target;      // duplicated control. cannot be null.
        public TweenType type;
        public ValueTweeningUpdate update;
        
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
        public UnityEngine.Object guard;        // lifetime control. cannot be null.
        public bool isTimeout => timeTo < (realtime ? Time.realtimeSinceStartup : Time.time);
        public bool invalid => target == null || guard == null || update == null;
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