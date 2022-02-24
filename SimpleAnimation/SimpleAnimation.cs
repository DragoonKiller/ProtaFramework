using System;
using System.Collections.Generic;
using Prota.Unity;
using UnityEngine;

namespace Prota.Animation
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleAnimation : MonoBehaviour
    {
        public SimpleAnimationAsset asset;
        
        
        [Header("state")]
        public float currentTime;
        
        SpriteRenderer sprite => this.GetComponent<SpriteRenderer>();
        
        void Start()
        {
            Debug.Assert(asset);
        }
    
        void Update()
        {
            this.name = $"Animation:{ asset?.name }";
            
            if(!Application.isPlaying)
            {
                return;
            }
            
            currentTime += Time.deltaTime;
            if(currentTime >= asset.duration / asset.fps)
            {
                if(asset.loop) currentTime -= asset.duration / asset.fps;
                else currentTime = asset.duration / asset.fps;
            }
            
            if(asset.frames.Count == 0) return;
            
            var curFrameNum = Mathf.FloorToInt(currentTime * asset.fps);
            curFrameNum = Mathf.Min(curFrameNum, asset.frames.Count - 1);
            var frame = asset.frames[curFrameNum];
            sprite.sprite = frame;
            
            var curAnchorNum = Mathf.FloorToInt(currentTime * asset.fps);
            curAnchorNum = Mathf.Min(curAnchorNum, asset.anchors.Count - 1);
            var curOffset = asset.anchors[curAnchorNum];
            this.transform.localPosition = curOffset;
        }
        
        
        
        
        
        
        
        
    }
}