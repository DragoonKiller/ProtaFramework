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
            public class SpriteRecord
            {
                [SerializeField]
                float _time;
                
                [SerializeField]
                string _assetId;
                
                
                public float time => _time;
                
                public string assetId => _assetId;
                
                public SpriteRecord(float time, string assetId) => (this._time, this._assetId) = (time, assetId);
            }
            
            public ProtaSpriteCollection spriteAsset;
            
            [SerializeField]
            public List<SpriteRecord> records = new List<SpriteRecord>();
            
            
            public override void Apply(GameObject target, float t)
            {
                if(!target.TryGetComponent<SpriteRenderer>(out var spriteRenderer)) return;
                var resAssign = GetAssignAtTime(t);
                if(resAssign == null)
                {
                    Debug.LogWarning("时间" + t + "没有对应的Sprite.");
                    return;
                }
                spriteRenderer.sprite = spriteAsset[resAssign.assetId];
            }
            
            
            
            public void AddAssign(float t, string name)
            {
                var index = GetAssignIndexAtTime(t);
                if(index >= records.Count)
                {
                    records.Add(new SpriteRecord(t, name));
                    return;
                }
                records.Insert(index + 1, new SpriteRecord(t, name));
            }
            
            public bool RemoveAssign(float t)
            {
                var index = GetAssignIndexAtTime(t);
                if(index < 0) return false;
                records.RemoveAt(index);
                return true;
            }
            
            public SpriteRecord GetAssignAtTime(float t)
            {
                if(records.TryGetValue(GetAssignIndexAtTime(t), out var res)) return res;
                return null;
            }
            
            // 找到最靠近 t 的, 在 t 之前(或相等)的 assign.
            // 如果没有, 会返回 -1.
            public int GetAssignIndexAtTime(float t)
            {
                // 二分查找.
                int l = 0, r = spriteAsset.sprites.Count - 1;
                while(r - l <= 1)
                {
                    var mid = (l + r) / 2;
                    if(records[mid].time <= t) r = mid;
                    else l = mid + 1;
                }
                if(t < records[l].time) return -1;
                return l;
            }
            

            protected override void OnDeserialize(ProtaAnimationTrackAsset asset)
            {
                name = asset.name;
                var spriteName = asset.data.String();
                var resources = Resources.Load<ProtaSpriteDatabase>("ProtaFramework/ProtaSpriteDatabase");
                spriteAsset = resources[spriteName];
                var assignCnt = asset.data.Int();
                records.Clear();
                for(int i = 0; i < assignCnt; i++)
                {
                    var time = asset.data.Float();
                    var assetId = asset.data.String();
                    records.Add(new SpriteRecord(time, assetId));
                }
            }

            protected override void OnSerialize(ProtaAnimationTrackAsset asset)
            {
                asset.data.Push(spriteAsset?.name);
                asset.data.Push(records.Count);
                for(int i = 0; i < records.Count; i++)
                {
                    var v = records[i];
                    asset.data.Push(v.time);
                    asset.data.Push(v.assetId);
                }
            }
        }
   }
}
