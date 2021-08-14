using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using Prota.Data;

namespace Prota.Animation
{
    /// <summary>
    /// Track 的基础类型.
    /// </summary>
    [Serializable]
    public abstract partial class ProtaAnimationTrack
    {
        [SerializeField]
        public string name = "";
        
        public string type => this.GetType().Name;
        
        public abstract void Apply(DataBlock anim, float t);
        
        public abstract void Serialize(ProtaAnimationTrackAsset s);
        
        public abstract void Deserialize(ProtaAnimationTrackAsset s);
    }
}