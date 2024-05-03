using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Prota.Unity
{
    // 一个float计数器.
    
    [Serializable]
    public class CountFloat
    {
        public float value;
        
        public float duration;
        
        public float ratio => value / duration;
        
        public float process => 1 - ratio;
        
        public bool isZero => value == 0;
        
        public CountFloat() { }
        
        public CountFloat(float duration)
        {
            this.duration = duration;
            this.value = duration;
        }
        
        public CountFloat Set(float v, float duration)
        {
            value = v;
            this.duration = duration;
            return this;
        }
        
        public CountFloat Add(float v)
        {
            value += v;
            duration += v;
            return this;
        }
        
        public CountFloat Set(float v)
        {
            value = v;
            duration = v;
            return this;
        }
        
        public CountFloat Step(float dt)
        {
            value = Mathf.MoveTowards(value, 0, dt / duration);
            return this;
        }
    }
    
    
    
    public static partial class MethodExtensions
    {
        public static float Evaluate(this AnimationCurve curve, CountFloat countFloat)
        {
            return curve.Evaluate(countFloat.process);
        }
    }
}
