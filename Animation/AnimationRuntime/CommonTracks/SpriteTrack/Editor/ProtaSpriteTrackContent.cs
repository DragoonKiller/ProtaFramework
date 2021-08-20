using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Animation;
using UnityEngine;

namespace Prota.Editor
{
    public partial class ProtaAnimationTrackEditor
    {
        [TrackEditor(typeof(ProtaAnimationTrack.Sprite))]
        public class ProtaSpriteTrackContent : ProtaAnimationTrackEditor
        {
            public override void UpdateTrackContent(ProtaAnimationTrackContent content)
            {
                
            }
        }
    }
}