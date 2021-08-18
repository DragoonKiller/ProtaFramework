using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;

namespace Prota.Animation
{
    public sealed class ProtaAnimationAsset : ScriptableObject
    {
        [SerializeField]
        public List<ProtaAnimationTrackAsset> tracks = new List<ProtaAnimationTrackAsset>();
        
        public void Clear() => tracks.Clear();
        
        public void Add(ProtaAnimationTrackAsset track) => tracks.Add(track);
        
        public void Add(ProtaAnimationTrack track)
        {
            var asset = new ProtaAnimationTrackAsset();
            track.Serialize(asset);
            tracks.Add(asset);
        }
        
        public void Remove(ProtaAnimationTrackAsset asset) => tracks.Remove(asset);
        
    }
}