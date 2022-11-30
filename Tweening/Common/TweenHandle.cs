using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tween
{
    public struct TweenHandle
    {
        internal readonly ArrayLinkedListKey key;
        
        internal readonly ArrayLinkedList<TweenData> data;
        
        public UnityEngine.Object target
        {
            get => data[key].target;
            set => data[key].target = value;
        }
        
        public TweenType type
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
        
        public Action<TweenHandle> onInterrupted
        {
            get => data[key].onInterrupted;
            set => data[key].onInterrupted = value;
        }
        
        public Action<TweenHandle> onRemove
        {
            get => data[key].onRemove;
            set => data[key].onRemove = value;
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
        
        public TweenEase ease
        {
            get => data[key].ease;
            set => data[key].ease = value;
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
        
        public LifeSpan guard
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
        
        public TweenHandle SetTo(float to)
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
            this.curve = curve;
            this.ease = TweenEase.linear;
            return this;
        }
        
        public TweenHandle SetEase(TweenEase? ease = null)
        {
            if(ease == null) ease = TweenEase.linear;
            this.curve = null;
            this.ease = ease.Value;
            return this;
        }
        
        public TweenHandle SetGuard(LifeSpan x)
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
        public TweenHandle this[TweenType type]
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
