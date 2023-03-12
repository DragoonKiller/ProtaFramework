using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Animation
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleAnimation : MonoBehaviour
    {
        [SerializeField]
        SimpleAnimationAsset asset;
        
        [Header("state")]
        
        [Readonly(whenPlaying = false)] public float currentTime;
        
        public float speedMultiply = 1;
        
        public SpriteRenderer sprite => this.GetComponent<SpriteRenderer>();
        
        [SerializeField] bool _mirror = false;
        public bool mirror
        {
            get => _mirror;
            set
            {
                if(value == _mirror) return;
                _mirror = value;
                var localScale = this.transform.localScale;
                localScale.x *= -1;
                this.transform.localScale = localScale;
            }
        }
        
        public float duration => asset?.duration ?? 0;
        
        void Update()
        {
            if(!Application.isPlaying) return;
            
            this.name = $"Animation:{ asset?.name }";
            
            if(asset.frames.Count == 0) return;
            
            currentTime += Time.deltaTime * speedMultiply;
            if(currentTime >= asset.frameCount / asset.fps)
            {
                if(asset.loop) currentTime -= asset.frameCount / asset.fps;
                else currentTime = asset.frameCount / asset.fps;
            }
            
            Refresh();
        }
        
        public SimpleAnimation Refresh()
        {
            var curFrameNum = Mathf.FloorToInt(currentTime * asset.fps);
            curFrameNum = Mathf.Min(curFrameNum, asset.frames.Count - 1);
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
        
        public SimpleAnimation Play(SimpleAnimationAsset asset, bool restartWithTheSameAnim = false)
        {
            if(this.asset == asset)
            {
                if(restartWithTheSameAnim) Restart();
                return this;
            }
            
            this.asset = asset;
            Restart();
            return this;
        }
        
        public SimpleAnimation Restart() => SetTime(0);
        
        public SimpleAnimation SetTime(float time)
        {
            this.currentTime = time;
            Refresh();
            return this;
        }
        
        
        
        
        
    }
}
