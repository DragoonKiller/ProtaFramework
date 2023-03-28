using System;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tween
{
    // 一个访问 ArrayLinkedList 中 TweenData 元素的外部接口.
    public struct TweenHandle
    {
        internal readonly ArrayLinkedListKey key;
        
        internal readonly ArrayLinkedList<TweenData> data;
        
        public UnityEngine.Object target
        {
            get => data[key].target;
            set => data[key].target = value;
        }
        
        public TweenId tid
        {
            get => data[key].tid;
            set => data[key].tid = value;
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
        
        public bool loop
        {
            get => data[key].loop;
            set => data[key].loop = value;
        }
        
        public bool reverseOnLoopFinish
        {
            get => data[key].reverseOnLoopFinish;
            set => data[key].reverseOnLoopFinish = value;
        }
        
        public bool started
        {
            get => data[key].started;
            private set => data[key].started = value;
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
        
        public float duration => timeTo - timeFrom;
        
        public bool isTimeout => timeTo < (realtime ? Time.realtimeSinceStartup : Time.time);
        
        // ====================================================================================================
        // 
        // ====================================================================================================
        
        internal TweenHandle(ArrayLinkedListKey handle, ArrayLinkedList<TweenData> data)
        {
            this.key = handle;
            this.data = data;
        }
        
        public TweenHandle Clear()
        {
            data[key] = default;
            return this;
        }
        
        // ====================================================================================================
        // Set 系列函数
        // ====================================================================================================
        
        
        // from 和 to 互换.
        public TweenHandle SetReverse()
        {
            var a = from;
            from = to;
            to = a;
            return this;
        }
        
        
        public TweenHandle SetEase(AnimationCurve curve = null)
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
        public TweenHandle SetEase(TweenEaseEnum ease)
        {
            this.curve = null;
            this.ease = TweenEase.GetFromEnum(ease);
            return this;
        }
        
        public TweenHandle SetGuard(LifeSpan x)
        {
            this.guard = x;
            return this;
        }
        
        public TweenHandle SetLoop(bool loop, bool reverseOnLoopFinish = false)
        {
            this.loop = loop;
            this.reverseOnLoopFinish = reverseOnLoopFinish;
            return this;
        }
        
        public TweenHandle SetCustomData(object customData)
        {
            this.customData = customData;
            return this;
        }
        
        public TweenHandle OnFinish(Action<TweenHandle> onFinish)
        {
            this.onFinish += onFinish;
            return this;
        }
        
        public TweenHandle OnInterrupted(Action<TweenHandle> onInterrupted)
        {
            this.onInterrupted += onInterrupted;
            return this;
        }
        
        public TweenHandle OnRemove(Action<TweenHandle> onRemove)
        {
            this.onRemove += onRemove;
            return this;
        }
        
        public TweenHandle SetFromTo(float? from, float? to)
        {
            if(from.HasValue) this.from = from.Value;
            if(to.HasValue) this.to = to.Value;
            return this;
        }
        
        public TweenHandle SetCurrentRatio(float ratio)
        {
            var d = duration;
            var startTime = duration * ratio;
            var endTime = duration * (1 - ratio);
            this.timeFrom = realtime ? Time.realtimeSinceStartup - startTime : Time.time - startTime;
            this.timeTo = realtime ? Time.realtimeSinceStartup + endTime : Time.time + endTime;
            return this;
        }
        
        // ====================================================================================================
        // 通用函数.
        // ====================================================================================================
        
        public float EvaluateRatio(float ratio) => data[key].EvaluateRatio(ratio);
        
        public float Evaluate(float ratio) => data[key].Evaluate(ratio);
        
        public float GetTimeLerp() => data[key].GetTimeLerp();
        
        
        // 使用先前的持续时间配置.
        public TweenHandle Restart()
        {
            Debug.Assert(this.started, "Tween need to start before calling restart.");
            var d = duration;
            this.timeFrom = realtime ? Time.realtimeSinceStartup : Time.time;
            this.timeTo = this.timeFrom + d;
            return this;
        }
        
        public TweenHandle Start(float duration, bool realtime = false)
        {
            this.realtime = realtime;
            this.timeFrom = realtime ? Time.realtimeSinceStartup : Time.time;
            this.timeTo = this.timeFrom + duration;
            this.started = true;
            return this;
        } 
        
        public TweenHandle Kill()
        {
            ProtaTweenManager.instance.TagRemoved(this);
            return this;
        }
        
        public static TweenHandle none => new TweenHandle(ArrayLinkedListKey.none, null);
        
        public bool isNone => data == null;
        
        public override string ToString() => $"handle[{ key.ToString() }]";
    }
}
