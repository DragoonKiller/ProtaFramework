using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prota.Animation
{
    
    public class SimpleAnimationAsset : ScriptableObject
    {
        public List<Sprite> sprites = new List<Sprite>();
        
        public List<Sprite> frames = new List<Sprite>();
        
        public List<Vector2> anchors = new List<Vector2>();
        
        
        // in frames
        public int frameCount;
        
        public float fps = 60;
        
        public bool loop;
        
        public float duration => frameCount / fps;
        
        [NonSerialized]
        public List<Sprite> anchorResources = new List<Sprite>();
        
        public void Clear()
        {
            sprites.Clear();
            anchorResources.Clear();
            frames.Clear();
            anchors.Clear();
        }
        
        public void Process()
        {
            var prevSprite = sprites[0];
            
            sprites.Sort((a, b) => a.name.CompareTo(b.name));
            
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
            
            if(anchorResources.Count == 0) return;
            
            anchorResources.Sort((a, b) => a.name.CompareTo(b.name));
            
            var prevAnchor = new Vector3(0, 0, 0);
            foreach(var sprite in anchorResources)
            {
                var name = sprite.name;
                var info = name.Split("_");
                var numStr = info[info.Length - 1];
                var num = int.Parse(numStr);
                for(int _ = 0; _ < 1000; _++)
                {
                    if(anchors.Count >= num) break;
                    anchors.Add(GetAnchor(prevSprite));
                }
                prevSprite = sprite;
            }
            anchors.Add(GetAnchor(anchorResources.Last()));
        }
        
        Vector2 GetAnchor(Sprite sprite)
        {
            var texture = sprite.texture;
            var rawData = texture.GetRawTextureData<Color32>();
            
            int w = texture.width, h = texture.height;
            
            // 找到 R 通道 >= 0.5 的最长的行列.
            
            var rowRCnt = new int[h];
            var colRCnt = new int[w];
            
            for(int i = 0; i < h; i++)
            {
                var offset = w * i;
                for(int j = 0; j < w; j++)
                {
                    var color = rawData[j + offset];
                    var r = color.r * color.a / (255 * 128);
                    rowRCnt[i] += r;
                    colRCnt[j] += r;
                }
            }
            
            var max = 0;
            var row = 0;
            for(int i = 0, n = rowRCnt.Length; i < n; i++)
            {
                if(rowRCnt[i] > max)
                {
                    max = rowRCnt[i];
                    row = i;
                }
            }
            
            max = 0;
            var col = 0;
            for(int i = 0, n = colRCnt.Length; i < n; i++)
            {
                if(colRCnt[i] > max)
                {
                    max = colRCnt[i];
                    col = i;
                }
            }
            
            var hRate = texture.height == 1 ? 0 : -((float)row / (texture.height - 1) - 0.5f);
            var wRate = texture.width == 1 ? 0 : ((float)col / (texture.width - 1) - 0.5f);
            var worldW = texture.width / sprite.pixelsPerUnit;
            var worldH = texture.height / sprite.pixelsPerUnit;
            
            return new Vector2(wRate * worldW, hRate * worldH);
        }
        
    }
}