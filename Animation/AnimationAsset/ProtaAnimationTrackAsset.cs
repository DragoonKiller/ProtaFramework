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
        
        public ProtaAnimationTrack Instantiate()
        {
            if(string.IsNullOrWhiteSpace(type)) return null;
            var trackType = ProtaAnimationTrack.types[type];
            var track = Activator.CreateInstance(trackType) as ProtaAnimationTrack;
            track.name = name;
            data.Reset();
            track.Deserialize(this);
            return track;
        }
        
        public T Instantiate<T>() where T: ProtaAnimationTrack => Instantiate() as T;
        
        public static ProtaAnimationTrackAsset Save(ProtaAnimationTrack track)
        {
            var asset = new ProtaAnimationTrackAsset();
            asset.type = track.GetType().Name;
            asset.name = track.name;
            asset.data.Reset();
            track.Serialize(asset);
            return asset;
        }
    }
}