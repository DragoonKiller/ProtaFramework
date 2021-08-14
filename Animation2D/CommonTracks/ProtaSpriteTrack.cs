using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using Prota.Data;

namespace Prota.Animation
{
    public partial class ProtaAnimationTrack
    {

        [Serializable]
        public sealed class Sprite : ProtaAnimationTrack
        {
            public struct SpriteAssign
            {
                [SerializeField]
                float _time;
                
                [SerializeField]
                string _assetId;
                
                public float time => _time;
                
                public string assetId => _assetId;
                
                public SpriteAssign(float time, string assetId) => (this._time, this._assetId) = (time, assetId);
            }
            
            [SerializeField]
            public ProtaSpriteCollection sprites;
            
            [SerializeField]
            public List<SpriteAssign> assign = new List<SpriteAssign>();
            
            [SerializeField]
            public int lastFrame = 0;
            
            public override void Apply(Data.DataBlock data, float t)
            {
                var target = data["sprite"] as DataBinding.Sprite;
                
            }

            public override void Deserialize(ProtaAnimationTrackAsset s)
            {
                s.name = name;
                s.data.Push(sprites.name);
                s.data.Push(assign.Count);
                for(int i = 0; i < assign.Count; i++)
                {
                    var v = assign[i];
                    s.data.Push(v.time);
                    s.data.Push(v.assetId);
                }
            }

            public override void Serialize(ProtaAnimationTrackAsset s)
            {
                name = s.data.String();
                sprites = Resources.Load<ProtaSpriteDatabase>("ProtaFramework/ProtaSpriteDatabase")[s.data.String()];
                var assignCnt = s.data.Int();
                assign.Clear();
                for(int i = 0; i < assignCnt; i++)
                {
                    var time = s.data.Float();
                    var assetId = s.data.String();
                    assign.Add(new SpriteAssign(time, assetId));
                }
            }
        }
   }
}
