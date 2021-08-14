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
    }
}