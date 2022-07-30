using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Tweening
{
    [Flags]
    public enum ScaleTweeningMask
    {
        X = 1,
        Y = 2,
        Z = 4,
    }
    
    public struct ScaleTweeningHandle
    {
        public readonly GameObject binding;
        public readonly Transform target;
        public readonly Vector3 from;
        public readonly Vector3 to;
        public readonly float fromTime;
        public readonly float toTime;
        public readonly ScaleTweeningMask mask;

        public ScaleTweeningHandle(GameObject binding, Transform target, Vector3 from, Vector3 to, float fromTime, float toTime, ScaleTweeningMask mask)
        {
            this.binding = binding;
            this.target = target;
            this.from = from;
            this.to = to;
            this.fromTime = fromTime;
            this.toTime = toTime;
            this.mask = mask;
        }

        public void Apply(float t)
        {
            var ratio = (fromTime, toTime).InvLerp(t);
            var s = target.localScale;
            var res = (from, to).Lerp(ratio);
            if((mask & ScaleTweeningMask.X) == 0) res.x = s.x;
            if((mask & ScaleTweeningMask.Y) == 0) res.y = s.y;
            if((mask & ScaleTweeningMask.Z) == 0) res.z = s.z;
            target.localScale = res;
        }
    }
    
    [Serializable]
    public class ScaleTweening
    {
        public List<ScaleTweeningHandle> handles = new List<ScaleTweeningHandle>();
        
        
        
        
    }
    
    
}