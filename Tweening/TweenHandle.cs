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
    
    internal struct TweenData
    {
        public UnityEngine.Object target;      // duplicated control. cannot be null.
        public TweeningType type;
        public ValueTweeningUpdate update;
        
        public Action<TweenHandle> onFinish;
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
        
    }
    
    public struct TweenHandle
    {
        internal readonly ArrayLinkedListKey key;
        
        internal readonly ArrayLinkedList<TweenData> data;
        
        public UnityEngine.Object target
        {
            get => data[key].target;
            set => data[key].target = value;
        }
        
        public TweeningType type
        {
            get => data[key].type;
            set => data[key].type = value;
        }
        
        public ValueTweeningUpdate update
        {
            get => data[key].update;
            set => data[key].update = value;
        }
        
        public Action<TweenHandle> onFinish
        {
            get => data[key].onFinish;
            set => data[key].onFinish = value;
        }
        
        public object customData
        {
            get => data[key].customData;
            set => data[key].customData = value;
        }
        
        public float from
        {
            get => data[key].from;
            set => data[key].from = value;
        }
        
        public float to
        {
            get => data[key].to;
            set => data[key].to = value;
        }
        
        public AnimationCurve curve
        {
            get => data[key].curve;
            set => data[key].curve = value;
        }
        
        public float timeFrom
        {
            get => data[key].timeFrom;
            set => data[key].timeFrom = value;
        }
        
        public float timeTo
        {
            get => data[key].timeTo;
            set => data[key].timeTo = value;
        }
        
        public bool realtime
        {
            get => data[key].realtime;
            set => data[key].realtime = value;
        }
        
        public UnityEngine.Object guard
        {
            get => data[key].guard;
            set => data[key].guard = value;
        }
        
        internal TweenHandle(ArrayLinkedListKey handle, ArrayLinkedList<TweenData> data)
        {
            this.key = handle;
            this.data = data;
        }
        
        public bool isTimeout => timeTo < (realtime ? Time.realtimeSinceStartup : Time.time);
        
        public float EvaluateRatio(float ratio) => data[key].EvaluateRatio(ratio);
        public float Evaluate(float ratio) => data[key].Evaluate(ratio);
        
        public float GetTimeLerp() => data[key].GetTimeLerp();
        
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
        
        
        public static TweenHandle none => new TweenHandle(new ArrayLinkedListKey(-1, -1, null), null);
        public bool isNone => key.id == -1;

        public override string ToString() => $"handle[{ key.ToString() }]";
    }
    
    
    public class BindingList
    {
        public int count;
        public TweenHandle[] bindings = new TweenHandle[15];
        public TweenHandle this[TweeningType type]
        {
            get => type < 0 ? TweenHandle.none : bindings[(int)type];
            set
            {
                
                if(type < 0) return;         // always 0 while type < 0
                var original = bindings[(int)type];
                if(original.isNone && !value.isNone) count++;
                if(!original.isNone && value.isNone) count--;
                bindings[(int)type] = value;
            }
        }
    }
}
