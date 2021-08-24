using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Prota.Unity;
using UnityEditor;

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
            asset.type = track.GetType().Name;
            asset.name = track.name;
            track.Serialize();
            tracks.Add(asset);
        }
        
        public void Remove(ProtaAnimationTrackAsset asset) => tracks.Remove(asset);
        
        [MenuItem("Assets/ProtaFramework/动画/动画资源")]
        static void CreateAsset()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            var x = ScriptableObject.CreateInstance<ProtaAnimationAsset>();
            AssetDatabase.CreateAsset(x, Path.Combine(path, "ProtaAnimationAsset.asset"));
        }
    }
}