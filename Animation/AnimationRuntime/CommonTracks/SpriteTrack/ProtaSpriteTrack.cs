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
            /// <summary>
            /// 在某个时间点记录某个 Sprite.
            /// </summary>
            [Serializable]
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
            
            public ProtaSpriteCollection spriteAsset;
            
            [SerializeField]
            public List<SpriteAssign> assign = new List<SpriteAssign>();
            
            [SerializeField]
            public int lastFrame = 0;
            
            public override void Apply(GameObject target, float t)
            {
                if(!target.TryGetComponent<SpriteRenderer>(out var spriteRenderer)) return;
                var resAssign = GetAssignAtTime(t);
                if(t < resAssign.time)
                {
                    Debug.LogWarning("时间" + t + "没有对应的Sprite.");
                    return;
                }
                spriteRenderer.sprite = spriteAsset[resAssign.assetId];
            }
            
            public void AddAssign(float t, string name)
            {
                var index = GetAssignIndexAtTime(t);
                if(index >= assign.Count)
                {
                    assign.Add(new SpriteAssign(t, name));
                    return;
                }
                if(t < assign[index].time) index += 1;
                assign.Insert(index + 1, new SpriteAssign(t, name));
            }
            
            public bool RemoveAssign(float t)
            {
                var index = GetAssignIndexAtTime(t);
                if(t < assign[index].time) return false;
                assign.RemoveAt(index);
                return true;
            }
            
            public SpriteAssign GetAssignAtTime(float t) => assign[GetAssignIndexAtTime(t)];
            
            // 找到最靠近 t 的, 在 t 之前的 assign.
            // 如果没有会返回第一个. 通过
            public int GetAssignIndexAtTime(float t)
            {
                // 二分查找.
                int l = 0, r = spriteAsset.sprites.Count - 1;
                while(r - l <= 1)
                {
                    var mid = (l + r) / 2;
                    if(assign[mid].time <= t) r = mid;
                    else l = mid + 1;
                }
                return l;
            }
            

            protected override void OnDeserialize(ProtaAnimationTrackAsset asset)
            {
                name = asset.name;
                var spriteName = asset.data.String();
                var resources = Resources.Load<ProtaSpriteDatabase>("ProtaFramework/ProtaSpriteDatabase");
                spriteAsset = resources[spriteName];
                var assignCnt = asset.data.Int();
                assign.Clear();
                for(int i = 0; i < assignCnt; i++)
                {
                    var time = asset.data.Float();
                    var assetId = asset.data.String();
                    assign.Add(new SpriteAssign(time, assetId));
                }
            }

            protected override void OnSerialize(ProtaAnimationTrackAsset asset)
            {
                asset.data.Push(spriteAsset?.name);
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
