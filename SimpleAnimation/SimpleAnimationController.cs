using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Prota.Unity;
using Prota.Timer;

namespace Prota.Animation
{
    // 控制一整个 GameObject 内的 Animation Controller.
    [ExecuteAlways]
    public class SimpleAnimationController : MonoBehaviour
    {
        [Serializable]
        public class AnimEntry
        {
            public SimpleAnimation anim;
            public SimpleAnimationAsset asset;
        }
        
        [Range(0, 1)] public float currentRate = 0;
        
        public bool autoUpdate = true;
        public float duration = 1;
        public float speedMultiply = 1;
        
        public List<AnimEntry> anims = new List<AnimEntry>();
        
        
        
        [SerializeField, EditorButton] bool refresh = false;
        
        void Start()
        {
            var t = anims.ToDictionary(x => x.anim, x => x.asset);
            anims.Clear();
            foreach(var a in this.GetComponentsInChildren<SimpleAnimation>())
            {
                anims.Add(new AnimEntry() {
                    anim = a,
                    asset = t.TryGetValue(a, out var oriAsset) && oriAsset != null ? oriAsset : a.asset,
                });
                
            }
            
            foreach(var a in anims) a.anim.autoUpdate = false;
            
            if(anims.Count == 1 && anims[0].asset != null) duration = anims[0].asset.duration;
            else duration = anims.Select(x => x.anim?.asset).Where(a => a != null).FirstOrDefault()?.duration ?? 0;
            
            if(duration != 0)
            {
                foreach(var a in anims)
                {
                    if(a.anim.asset.PassValue(out var asset) == null)
                        Debug.LogError($"动画组件 { a.anim.gameObject.name } 未挂载动画.");
                    if((asset.duration - duration).Abs() >= 1e-4f)
                        Debug.LogError($"动画组件 { a.anim.gameObject.name } 时长不一致.");
                }
            }
            
            refresh = false;
        }
        
        void Update()
        {
            if(refresh) Start();
            
            if(!autoUpdate) return;
            
            foreach(var a in anims) a.anim.currentRate = currentRate;
            if(Application.isPlaying) currentRate += Time.deltaTime * speedMultiply / duration;
            currentRate = currentRate.Repeat(1f);
            
            foreach(var a in anims) a.anim.currentRate = currentRate;
        }
    }
}
