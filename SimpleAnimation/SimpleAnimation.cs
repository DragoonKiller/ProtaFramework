using System;
using System.Collections.Generic;
using UnityEngine;

using Prota.Unity;
using Prota.Timer;

namespace Prota.Animation
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleAnimation : MonoBehaviour
    {
        [field: SerializeField] public SimpleAnimationAsset asset { get; private set; }
        
        [Header("state")]
        
        [Range(0, 1)] public float currentRate = 0;
        
        public float currentTime => currentRate * (asset != null ? asset.duration * currentRate : 0);
        
        public bool autoUpdate = true;
        
        public bool playOnStart = false;
        
        public float playOnStartDelay = 0;
        
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
        
        void Start()
        {
            if(!playOnStart) return;
            if(asset == null) return;
            if(playOnStartDelay <= 0) Play(asset, true);
            else
            {
                this.gameObject.SetActive(false);
                this.NewTimer(playOnStartDelay, () => {
                    this.gameObject.SetActive(true);
                    this.Play(asset, true);
                });
            }
        }
        
        void Update()
        {
            if(asset == null)
            {
                sprite.sprite = null;
                this.name = "Animation:None";
                return;
            }
            
            this.name = $"Animation:{ asset?.name }";
            
            if(autoUpdate)
            {
                if(Application.isPlaying) currentRate += Time.deltaTime * speedMultiply / asset.duration;
                currentRate = currentRate.Repeat(1f);
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
            this.currentRate = time / asset.duration;
            Refresh();
            return this;
        }
        
    }
}
