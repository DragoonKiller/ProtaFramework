using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Prota.Unity
{
    
    // 一个从 0 到 1 变化的值.
    [Serializable]
    public class TwoStateFloat
    {
        static TwoStateFloat _placeHolder;
        public static TwoStateFloat placeHolder => _placeHolder ?? (_placeHolder = new TwoStateFloat());
        
        public float ratio = 0;     // 当前运动到哪里了.
        
        public float time = 1;      // 从 0 到 1 变化的总时间.
        
        public bool proceeding => ratio.InExclusive(0, 1) || (ratio == 0 && activate) || (ratio == 1 && !activate);
        
        // true: 往1走.
        // false: 往0走.
        public bool activate;
        public bool deactivate
        {
            get => !activate;
            set => activate = !value;
        }
        
        public event Action onActivate;
        public event Action onDeactivate;
        
        public void SetToStart()
        {
            activate = false;
            onActivate?.Invoke();
            onActivate = null;
        }
        
        public void SetToEnd()
        {
            activate = true;
            onDeactivate?.Invoke();
            onDeactivate = null;
        }
        
        public bool Activate(bool forced)
        {
            if(!forced && proceeding) return false;
            activate = true;
            return true;
        }
        
        public bool Deactivate(bool forced)
        {
            if(!forced && proceeding) return false;
            activate = false;
            return true;
        }
        
        public void ChangeState()
        {
            activate = !activate;
        }
        
        public void Step(float dt)
        {
            if(activate)
            {
                ratio = Mathf.Clamp01(ratio + dt / time);
                if(ratio >= 1)
                {
                    onDeactivate?.Invoke();
                    onDeactivate = null;
                }
            }
            else
            {
                ratio = Mathf.Clamp01(ratio - dt / time);
                if(ratio <= 0)
                {
                    onActivate?.Invoke();
                    onActivate = null;
                }
            }
        }
    }
    
    
    public static partial class MethodExtensions
    {
        public static float Evaluate(this AnimationCurve curve, TwoStateFloat state)
        {
            return curve.Evaluate(state.ratio);
        }
    }
}
