using System;
using System.Collections.Generic;
using Prota.Unity;
using UnityEngine;

namespace Prota.Animation
{
    
    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleAnimation : MonoBehaviour
    {
        public List<Sprite> sprites = new List<Sprite>();
        
        public List<Sprite> frames = new List<Sprite>();
        
        // seconds.
        public int duration;
        
        // frame per sec.
        public float fps;
        
        public bool loop;
        
        [Header("state")]
        public float currentTime;
        
        SpriteRenderer sprite => this.GetComponent<SpriteRenderer>();
        
        
        void Start()
        {
            if(sprites.Count == 0) return;
            var prevSprite = sprites[0];
            foreach(var sprite in sprites)
            {
                var name = sprite.name;
                var info = name.Split("_");
                var numStr = info[info.Length - 1];
                var num = int.Parse(numStr);
                for(int _ = 0; _ < 1000; _++)
                {
                    if(frames.Count >= num) break;
                    frames.Add(prevSprite);
                }
                prevSprite = sprite;
            }
            frames.Add(sprites.Last());
        }
        
        void Update()
        {
            currentTime += Time.deltaTime;
            if(currentTime >= duration / fps)
            {
                if(loop) currentTime -= duration / fps;
                else currentTime = duration * fps;
            }
            
            if(frames.Count == 0) return;
            
            var curFrameNum = Mathf.FloorToInt(currentTime * fps);
            curFrameNum = Mathf.Min(curFrameNum, frames.Count - 1);
            var frame = frames[curFrameNum];
            sprite.sprite = frame;
        }
        
        
        
        
        
        
        
        
    }
}