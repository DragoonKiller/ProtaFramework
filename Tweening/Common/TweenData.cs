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
        
        // 事件回调.
        public Action<TweenHandle> onFinish;
        public Action<TweenHandle> onInterrupted;
        public Action<TweenHandle> onRemove;
        
        // 用于存储用户自定义数据.
        public object customData;
        
        // 插值参数.
        public float from;
        public float to;
        
        // 插值曲线. 优先使用 ease.
        public AnimationCurve curve;
        public TweenEase ease;
        
        // 插值起始时间标记.
        public float timeFrom;
        public float timeTo;
        
        // 是否为循环.
        public bool loop;
        
        // 循环结束后是否反向.
        public bool reverseOnLoopFinish;
        
        // 是否已经开始.
        public bool started;
        
        // 是否使用真实时间.
        public bool realtime;
        
        // 用于存储生命周期. 如果没有就是不由生命周期控制.
        public LifeSpan guard;
        
        public bool isTimeout => started && (timeTo < (realtime ? Time.realtimeSinceStartup : Time.time));
        
        // 已考虑 target 被 destroy 但是引用还在.
        public bool invalid => started && (target == null || (guard != null && !guard.alive) || update == null);
        
        public bool valid => !invalid;
        
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
        
    }
    
}
