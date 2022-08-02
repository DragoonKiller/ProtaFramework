using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    public delegate void ValueTweeningCallback(TweeningHandle h, float t);
    
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
    
    public class TweeningHandle
    {
        internal long id;
        internal UnityEngine.Object target;      // duplicated control. cannot be null.
        internal TweeningType type;
        internal ValueTweeningCallback callback;
        
        public Action<TweeningHandle> onFinish;
        public object customData;
        
        public float from { get; private set; }
        public float to { get; private set; }
        public AnimationCurve curve { get; private set; }
        public float timeFrom { get; private set; }
        public float timeTo { get; private set; }
        public bool realtime { get; private set; }
        public UnityEngine.Object guard { get; private set; }        // lifetime control. cannot be null.
        
        public float EvaluateRatio(float ratio)
        {
            return curve == null ? curve.Evaluate(ratio) : ratio;
        }
        
        public float Evaluate(float ratio)
        {
            return (from, to).Lerp(EvaluateRatio(ratio));
        }
        
        public float GetTimeLerp()
        {
            return (timeFrom, timeTo).InvLerp(realtime ? Time.realtimeSinceStartup : Time.time);
        }
        
        public TweeningHandle SetFrom(float from)
        {
            this.from = from;
            return this;
        }
        
        public TweeningHandle SetTo(float from)
        {
            this.to = to;
            return this;
        }
        
        public TweeningHandle RecordTime(bool isRealtime = false)
        {
            this.realtime = isRealtime;
            this.timeFrom = realtime ? Time.realtimeSinceStartup : Time.time;
            return this;
        }
        
        public TweeningHandle SetDuration(float duration)
        {
            this.timeTo = this.timeFrom + duration;
            return this;
        }
        
        public TweeningHandle SetCurve(AnimationCurve curve = null)
        {
            if(curve == null) curve = AnimationCurve.Linear(0, 0, 1, 1);
            this.curve = curve;
            return this;
        }
        
        public TweeningHandle SetGuard(UnityEngine.Object x)
        {
            this.guard = guard;
            return this;
        }
        
        public TweeningHandle SetCustomData(object x)
        {
            this.customData = x;
            return this;
        }
        
    }
    
    
    public class BindingList
    {
        public int count = 0;
        public TweeningHandle[] bindings = new TweeningHandle[15];
        public TweeningHandle this[TweeningType type]
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
