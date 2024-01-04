using UnityEngine;
using Prota.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Codice.CM.Common.Tree.Partial;

namespace Prota.Tween
{
    public enum ProtaDynamicFXType
    {
        None = 0,
        Appear = 1,         // 出现, 变大到正常值一点再缩小到正常值.
        Disappear = 2,      // 消失, 变大一点然后缩小到不见.
        Charge = 3,         // 蓄力, 主动的压缩动作.
        Release = 4,        // 释放, 主动的拉长动作.
        Launch = 5,         // 蓄力+释放连续进行.
        Breathe = 6,        // 呼吸, 缓慢地变大变小.
        
    }
    
    [CreateAssetMenu(menuName = "Prota Framework/Prota Dynamic FX Asset")]
    public class ProtaDynamicFXAsset : ScriptableObject
    {
        [Header("Appear")]
        public AnimationCurve appearX;
        public AnimationCurve appearY;
        
        [Header("Disappear")]
        public AnimationCurve disappearX;
        public AnimationCurve disappearY;
        
        [Header("Charge")]
        public AnimationCurve chargeX;
        public AnimationCurve chargeY;
        
        [Header("Release")]
        public AnimationCurve releaseX;
        public AnimationCurve releaseY;
        
        [Header("Launch")]
        public AnimationCurve launchX;
        public AnimationCurve launchY;
        
        [Header("Breathe")]
        public AnimationCurve breatheX;
        public AnimationCurve breatheY;
    }
    
    
    // 用来做简单的动态效果.
    public class ProtaDynamicFX : MonoBehaviour
    {
        static ProtaDynamicFXAsset _asset;
        public static ProtaDynamicFXAsset asset
            => _asset ??= Resources.Load<ProtaDynamicFXAsset>("Config/Prota Dynamic FX Asset");
        
        public ProtaDynamicFXType type;
        
        [field:SerializeField] public float t { get; private set; }
        
        [field:SerializeField] public float duration { get; private set; }
        
        public bool loop;
        
        [field:SerializeField] public bool executing { get; private set; }
        
        [field:SerializeField, Readonly] public bool finished { get; private set; }
        
        [EditorButton] public bool play;
        
        public ProtaDynamicFX SetType(ProtaDynamicFXType type)
        {
            this.type = type;
            return this;
        }
        
        public ProtaDynamicFX SetLoop(bool loop)
        {
            this.loop = loop;
            return this;
        }
        
        public ProtaDynamicFX Execute(float duration)
        {
            this.duration = duration;
            t = 0;
            executing = true;
            finished = false;
            return this;
        }
        
        public ProtaDynamicFX Continue()
        {
            executing = true;
            return this;
        }
        
        public ProtaDynamicFX Pause()
        {
            executing = false;
            return this;
        }
        
        void Update()
        {
            if(play)
            {
                play = false;
                t = 0;
                executing = true;
                finished = false;
            }
            
            if(duration <= 0) return;
            if(!executing) return;
            
            t += Time.deltaTime;
            SetInterpolate(t);
            if(t < duration) return;
            if(loop)
            {
                t -= duration;
                finished = true;
                return;
            }
            else
            {
                executing = false;
                finished = true;
            }
        }

        public void SetInterpolate(float t)
        {
            switch(type)
            {
                case ProtaDynamicFXType.None:
                    break;
                case ProtaDynamicFXType.Appear:
                    SetLocalScale(t, asset.appearX, asset.appearY);
                    break;
                case ProtaDynamicFXType.Disappear:
                    SetLocalScale(t, asset.disappearX, asset.disappearY);
                    break;
                case ProtaDynamicFXType.Charge:
                    SetLocalScale(t, asset.chargeX, asset.chargeY);
                    break;
                case ProtaDynamicFXType.Release:
                    SetLocalScale(t, asset.releaseX, asset.releaseY);
                    break;
                case ProtaDynamicFXType.Launch:
                    SetLocalScale(t, asset.launchX, asset.launchY);
                    break;
                case ProtaDynamicFXType.Breathe:
                    SetLocalScale(t, asset.breatheX, asset.breatheY);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
        
        void SetLocalScale(float t, AnimationCurve x, AnimationCurve y)
        {
            this.transform.localScale = new Vector3(
                x.Evaluate(t / duration),
                y.Evaluate(t / duration),
                1
            );
        }
    }
    
    
}
