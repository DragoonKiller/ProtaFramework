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
        
        public float currentTime;
        
        public float speedMultiply = 1;
        
        public SpriteRenderer sprite => this.GetComponent<SpriteRenderer>();
        
        public bool _mirror = false;
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
        
        void Start()
        {
            Debug.Assert(asset);
            playing = asset;
        }
        
        [NonSerialized]
        SimpleAnimationAsset playing;
        
        void Update()
        {
            if(!Application.isPlaying)
            {
                return;
            }
            
            if(playing != asset && playing.name != asset.name)
            {
                currentTime = 0;
            }
            
            playing = asset;
            
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
        
        
        // 返回: 是否为新资源.
        public bool SetAsset(SimpleAnimationAsset asset)
        {
            if(this.asset == asset || this.asset.name == asset.name) return false;
            this.asset = asset;
            return true;
        }
        
        public void Refresh()
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
        }
        
        public void Restart()
        {
            currentTime = 0;
        }
        
        
        
        
        
    }
}