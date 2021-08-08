using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Animation2D
{
    /// <summary>
    /// 表示一段动画.
    /// </summary>
    public class ProtaSpriteAnimation : ScriptableObject
    {
        public List<Sprite> keyFrames;
        
        public float maxFrame;
        
        public float frameRate;
        
    }
    
}