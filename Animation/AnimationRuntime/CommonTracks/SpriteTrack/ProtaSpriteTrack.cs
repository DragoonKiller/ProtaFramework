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
            
            protected override void OnApply(Data.DataBlock data, float t)
            {
                var target = data["sprite"] as DataBinding.Sprite;
                
            }

            public override void Deserialize()
            {
                name = asset.name;
                var spriteName = asset.data.String();
                var resources = Resources.Load<ProtaSpriteDatabase>("ProtaFramework/ProtaSpriteDatabase");
                sprites = resources[spriteName];
                var assignCnt = asset.data.Int();
                assign.Clear();
                for(int i = 0; i < assignCnt; i++)
                {
                    var time = asset.data.Float();
                    var assetId = asset.data.String();
                    assign.Add(new SpriteAssign(time, assetId));
                }
            }

            public override void Serialize()
            {
                asset.data.Push(sprites?.name);
                asset.data.Push(assign.Count);
                for(int i = 0; i < assign.Count; i++)
                {
                    var v = assign[i];
                    asset.data.Push(v.time);
                    asset.data.Push(v.assetId);
                }
            }
        }
   }
}
