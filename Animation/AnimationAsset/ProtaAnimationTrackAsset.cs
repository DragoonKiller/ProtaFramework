using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;

namespace Prota.Animation
{
    [Serializable]
    public sealed class ProtaAnimationTrackAsset : ICloneable<ProtaAnimationTrackAsset>, ICloneable
    {
        [SerializeField]
        public string name;
        
        [SerializeField]
        public string type;
        
        [SerializeField]
        public SerializedData data = new SerializedData();
        
        
        object ICloneable.Clone() => this.Clone();
        public ProtaAnimationTrackAsset Clone()
        {
            var res = new ProtaAnimationTrackAsset();
            res.name = name;
            res.type = type;
            res.data = data.Clone();
            return res;
        }
    }
}