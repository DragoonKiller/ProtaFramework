using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Animation
{
    /// <summary>
    /// Track 的基础类型.
    /// </summary>
    public abstract class ProtaAnimationTrack
    {
        public string name = "";
        
        public float fps = 30;
        
        public float spf => 1.0f / fps;     // second per frame.
        
        public string type => this.GetType().FullName;
        
        public abstract void Apply(ProtaAnimationState target);
    }
}