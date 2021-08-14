using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Animation
{
    /// <summary>
    /// 表示一段动画.
    /// 一段动画会包含很多个 ProtaAnimationTrack.
    /// </summary>
    [Serializable]
    public class ProtaAnimationAsset
    {
        [SerializeField]
        List<ProtaAnimationTrack> trackList;
        public IReadOnlyList<ProtaAnimationTrack> tracks => trackList;
        
        
        
        
        
        
        
        
        
    }
    
}