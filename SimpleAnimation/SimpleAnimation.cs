using System;
using System.Collections.Generic;
using UnityEngine;

using Prota.Unity;

namespace Prota.Animation
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleAnimation : MonoBehaviour
    {
        [field: SerializeField] public SimpleAnimationAsset asset { get; private set; }
        
        [Header("state")]
        
        [Range(0, 1)] public float process = 0;
        
        public float currentTime
        {
            get
            {
                if(asset == null) throw new InvalidOperationException("Cannot get time when asset is null.");
                return process * asset.duration;
            }
            
            set
            {
                if(asset == null) throw new InvalidOperationException("Cannot set time when asset is null.");
                process = value / asset.duration;
            }
        }
        public bool autoPlay = true;
        
        public float speedMultiply = 1;
        
        public SpriteRenderer sprite => this.GetComponent<SpriteRenderer>();
        
        public bool mirror
        {
            get => sprite.flipX;
            set => sprite.flipX = value;
        }
        
        public float duration
        {
            get
            {
                if(asset == null) throw new InvalidOperationException("Cannot get duration when asset is null.");
                return asset.duration;
            }
        }
        
        void Update()
        {
            if(asset == null)
            {
                Clear();
                return;
            }
            
            this.name = $"Animation:{ asset.name }";
            
            if(autoPlay)
            {
                if(Application.isPlaying) process += ECS.dt * speedMultiply / asset.duration;
                process = process.Repeat(1f);
            }
            
            Refresh();
        }
        
        public SimpleAnimation Refresh()
        {
            var curFrameNum = Mathf.FloorToInt(currentTime * asset.fps);
            curFrameNum = curFrameNum.Clamp(0, asset.frames.Count - 1);
            var frame = asset.frames[curFrameNum];
            sprite.sprite = frame;
            
            if(asset.anchors.Count != 0)
            {
                var curAnchorNum = Mathf.FloorToInt(currentTime * asset.fps);
                curAnchorNum = Mathf.Min(curAnchorNum, asset.anchors.Count - 1);
                var curOffset = asset.anchors[curAnchorNum];
                this.transform.localPosition = curOffset;
            }
            
            return this;
        }
        
        public SimpleAnimation Play(SimpleAnimationAsset asset, float startTime = 0f, bool restartWithTheSameAnim = true)
        {
            if(asset == null)
            {
                Clear();
                return this;
            }
            
            if(this.asset == asset)
            {
                if(restartWithTheSameAnim) currentTime = startTime;
                return this;
            }
            
            this.asset = asset;
            currentTime = startTime;
            return this;
        }
        
        public void Clear()
        {
            this.asset = null;
            sprite.sprite = null;
            this.name = "Animation:None";
        }
        
    }
}
