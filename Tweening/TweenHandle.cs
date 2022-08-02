using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    public delegate void ValueTweeningUpdate(TweenHandle h, float t);
    
    public enum TweeningType
    {
        Custom = -1,  // does not counted into duplicate.
        
        MoveX = 1,
        MoveY,
        MoveZ,
        ScaleX,
        ScaleY,
        ScaleZ,
        RotateX,
        RotateY,
        RotateZ,
        ColorR,
        ColorG,
        ColorB,
        Transparency,
        
    }
    
    public class TweenHandle
    {
        internal long id;
        internal UnityEngine.Object target;      // duplicated control. cannot be null.
        internal TweeningType type;
        internal ValueTweeningUpdate update;
        
        public Action<TweenHandle> onFinish;
        public object customData;
        
        public float from { get; private set; }
        public float to { get; private set; }
        public AnimationCurve curve { get; private set; }
        public float timeFrom { get; private set; }
        public float timeTo { get; private set; }
        public bool realtime { get; private set; }
        public UnityEngine.Object guard { get; private set; }        // lifetime control. cannot be null.
        
        public bool isTimeout => timeTo < (realtime ? Time.realtimeSinceStartup : Time.time);
        
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
        
        public TweenHandle SetFrom(float from)
        {
            this.from = from;
            return this;
        }
        
        public TweenHandle SetTo(float from)
        {
            this.to = to;
            return this;
        }
        
        public TweenHandle Start(float duration, bool realtime = false)
        {
            this.realtime = realtime;
            this.timeFrom = realtime ? Time.realtimeSinceStartup : Time.time;
            this.timeTo = this.timeFrom + duration;
            
            return this;
        } 
        
        public TweenHandle SetCurve(AnimationCurve curve = null)
        {
            if(curve == null) curve = AnimationCurve.Linear(0, 0, 1, 1);
            this.curve = curve;
            return this;
        }
        
        public TweenHandle SetGuard(UnityEngine.Object x)
        {
            this.guard = x;
            return this;
        }
        
        public TweenHandle SetCustomData(object x)
        {
            this.customData = x;
            return this;
        }
        
    }
    
    
    public class BindingList
    {
        public int count;
        public TweenHandle[] bindings = new TweenHandle[15];
        public TweenHandle this[TweeningType type]
        {
            get => type < 0 ? null : bindings[(int)type];
            set
            {
                
                if(type < 0) return;         // always 0 while type < 0
                var original = bindings[(int)type];
                if(original == null && value != null) count++;
                if(original != null && value == null) count--;
                bindings[(int)type] = value;
            }
        }
    }
}
