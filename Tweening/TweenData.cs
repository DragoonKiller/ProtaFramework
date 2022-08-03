using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    internal struct TweenData
    {
        public UnityEngine.Object target;      // duplicated control. cannot be null.
        public TweeningType type;
        public ValueTweeningUpdate update;
        
        public Action<TweenHandle> onFinish;
        public Action<TweenHandle> onInterrupted;
        public Action<TweenHandle> onRemove;
        public object customData;
        
        public float from;
        public float to;
        public AnimationCurve curve;
        public float timeFrom;
        public float timeTo;
        public bool realtime;
        public UnityEngine.Object guard;        // lifetime control. cannot be null.
        public bool isTimeout => timeTo < (realtime ? Time.realtimeSinceStartup : Time.time);
        public bool invalid => target == null || guard == null || update == null;
        public bool valid => !invalid;
        
        public TweenHandle handle { get; private set; }
        
        public float EvaluateRatio(float ratio)
        {
            return curve?.Evaluate(ratio) ?? ratio;
        }
        
        public float Evaluate(float ratio)
        {
            return (from, to).Lerp(EvaluateRatio(ratio));
        }
        
        public float GetTimeLerp()
        {
            return (timeFrom, timeTo).InvLerp(realtime ? Time.realtimeSinceStartup : Time.time);
        }
        
        internal void SetHandle(TweenHandle handle)
        {
            this.handle = handle;
        }
        
    }
    
}