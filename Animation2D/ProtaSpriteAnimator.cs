using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Animation2D
{
    /// <summary>
    /// 动画播放器.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ProtaSpriteAnimator : MonoBehaviour
    {
        public ProtaSpriteAnimation anim;
        
        int _current;
        public int current
        {
            get => _current;
            set => Set(value);
        }
        
        public void Set(int i)
        {
            
        }
        
        public void SetNext()
        {
            
        }
        
    }
    
}