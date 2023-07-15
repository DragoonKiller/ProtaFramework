using UnityEngine;
using Prota.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Prota.Tween
{
    [ExecuteAlways]
    public class ProtaTweener : MonoBehaviour
    {
        [Serializable]
        public struct ActivateEntry
        {
            public GameObject g;
            public float from;
            public float to;
        }
        
        [Range(0, 1)] public float progress = 0;
        
        [Header("自动播放")]
        public bool autoPlay = true;
        
        // 播放完毕后是否停下来(设置 autoPlay = true).
        // 如果 loop = true, 则在起始位置(包括反向时的起始位置)停留. 否则, 在结束位置停留.
        public bool stopWhenFinished = false;
        
        // 播放时长. 决定了播放速度.
        public float duration = 1;
        
        public bool loop = false;
        
        // 指示当前的更新是从 0 到 1 还是反过来.
        public bool playReversed = false;
        
        // 循环一次后反转 from/to.
        public bool reverseOnLoop = false;
        
        // 组件被激活后是否重置.
        public bool resetWhenEnabled = false;
        
        // 组件被激活后重置到哪里.
        public float resetPosition = 0;
        
        [Header("移动")]
        public bool move;
        
        public Vector3 moveFrom = Vector3.zero;
        public Vector3 moveTo = Vector3.zero;
        public TweenEaseEnum moveEase = TweenEaseEnum.Linear;
        
        [Header("旋转")]
        public bool rotate;
        public Vector3 rotateFrom = Vector3.zero;
        public Vector3 rotateTo = Vector3.zero;
        public TweenEaseEnum rotateEase = TweenEaseEnum.Linear;
        
        [Header("缩放")]
        public bool scale;
        public Vector3 scaleFrom = Vector3.one;
        public Vector3 scaleTo = Vector3.one;
        public TweenEaseEnum scaleEase = TweenEaseEnum.Linear;
        
        [Header("颜色")]
        public bool color;
        public UnityEngine.Object colorTarget;
        public Color colorFrom = Color.white;
        public Color colorTo = Color.white;
        public TweenEaseEnum colorEase = TweenEaseEnum.Linear;
        
        [Header("录制")]
        public bool recordMode = false;
        
        void OnValidate()
        {
            AttachColorTarget();
        }
        
        
        void Update()
        {
            if(!Application.isPlaying)
            {
                if(recordMode) TryRecord();
                else UpdateValue();
                return;
            }
            
            recordMode = false;
            
            if(!autoPlay) return;
            
            if(playReversed) progress -= Time.deltaTime / duration;
            else progress += Time.deltaTime / duration;
            
            bool finished = false;
            if(loop)
            {
                if(reverseOnLoop)
                {
                    finished = (!playReversed && progress >= 1)
                        || (playReversed && progress <= 0);
                    if(finished && reverseOnLoop) playReversed = !playReversed;
                    else if(finished && reverseOnLoop) playReversed = !playReversed;
                    progress = progress.Clamp(0, 1);
                }
                else
                {
                    if(progress >= 1 || progress <= 0) finished = true;
                    if(stopWhenFinished && finished) progress = progress.Clamp(0, 1);
                    else progress = progress.Repeat(1);
                }
            }
            else
            {
                progress = progress.Clamp(0, 1);
                finished = progress == 1;
            }
            
            if(stopWhenFinished && finished) autoPlay = false;
            
            UpdateValue();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public void UpdateValue()
        {
            // move
            if(move) this.transform.position = (moveFrom, moveTo).Lerp(TweenEase.GetFromEnum(moveEase).Evaluate(progress));
            
            // rotate
            if(rotate) this.transform.eulerAngles = (rotateFrom, rotateTo).Lerp(TweenEase.GetFromEnum(rotateEase).Evaluate(progress));
            
            // scale
            if(scale) this.transform.localScale = (scaleFrom, scaleTo).Lerp(TweenEase.GetFromEnum(scaleEase).Evaluate(progress));
            
            // color
            if(color) UpdateColorValue(TweenEase.GetFromEnum(colorEase).Evaluate(progress));
        }
        
        public void ClearAllTween()
        {
            this.transform.ClearAllTween();
            if(colorTarget is MeshRenderer mr) mr.ClearAllTween();
            else if(colorTarget is SpriteRenderer sr) sr.ClearAllTween();
            else if(colorTarget is UnityEngine.UI.Image img) img.ClearAllTween();
            else if(colorTarget is UnityEngine.UI.Text txt) txt.ClearAllTween();
            else if(colorTarget is UnityEngine.UI.RawImage ri) ri.ClearAllTween();
            else if(colorTarget is UnityEngine.CanvasGroup cg) cg.ClearAllTween();
        }
        
        public void Play() => autoPlay = true;
        
        public void Play(float startProgress)
        {
            SetTo(startProgress);
            Play();
        }
        
        public void PlayForward(float startProgress = 0)
        {
            SetTo(startProgress);
            playReversed = false;
            Play();
        }
        
        public void PlayBackward(float startProgress = 1)
        {
            SetTo(startProgress);
            playReversed = true;
            Play();
        }
        
        // progress 在 0 到 1 之间.
        public void SetTo(float progress)
        {
            this.progress = progress;
            Update();
        }
        
        public void SetToFinish()
        {
            progress = 1;
            Update();
        }
        
        public void SetToStart()
        {
            progress = 0;
            Update();
        }
        
        public void SetTime(float time)
        {
            progress = time / duration;
            Update();
        }
        
        void OnEnable()
        {
            if(!resetWhenEnabled) return;
            if(!Application.isPlaying) return;
            progress = resetPosition;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void AttachColorTarget()
        {
            if(this.TryGetComponent<MeshRenderer>(out var mr)) colorTarget = mr;
            else if(this.TryGetComponent<SpriteRenderer>(out var sr)) colorTarget = sr;
            else if(this.TryGetComponent<CanvasGroup>(out var cg)) colorTarget = cg;
            else if(this.TryGetComponent<UnityEngine.UI.Image>(out var img)) colorTarget = img;
            else if(this.TryGetComponent<UnityEngine.UI.RawImage>(out var ri)) colorTarget = ri;
            else if(this.TryGetComponent<UnityEngine.UI.Text>(out var txt)) colorTarget = txt;
            else colorTarget = null;
        }
        

        void UpdateColorValue(float ratio)
        {
            if (colorTarget is MeshRenderer mr) mr.material.color = Color.Lerp(colorFrom, colorTo, ratio);
            else if (colorTarget is SpriteRenderer sr) sr.color = Color.Lerp(colorFrom, colorTo, ratio);
            else if (colorTarget is UnityEngine.UI.Image img) img.color = Color.Lerp(colorFrom, colorTo, ratio);
            else if (colorTarget is UnityEngine.UI.Text txt) txt.color = Color.Lerp(colorFrom, colorTo, ratio);
            else if (colorTarget is UnityEngine.UI.RawImage ri) ri.color = Color.Lerp(colorFrom, colorTo, ratio);
            else if (colorTarget is CanvasGroup cg) cg.alpha = ratio;
            
        }
        
        
        Color GetColorValue()
        {
            if (colorTarget is MeshRenderer mr) return mr.material.color;
            else if(colorTarget is SpriteRenderer sr) return sr.color;
            else if(colorTarget is UnityEngine.UI.Image img) return img.color;
            else if(colorTarget is UnityEngine.UI.Text txt) return txt.color;
            else if(colorTarget is UnityEngine.UI.RawImage ri) return ri.color;
            else if(colorTarget is CanvasGroup cg) return cg.alpha == 0 ? Color.clear : Color.white;
            else return Color.white;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public bool TryRecord()
        {
            if(progress == 0) RecordFrom();
            if(progress == 1) RecordTo();
            return progress == 0 || progress == 1;
        }

        public void RecordTo()
        {
            if (move) moveTo = this.transform.position;
            if (rotate) rotateTo = this.transform.eulerAngles;
            if (scale) scaleTo = this.transform.localScale;
            if (color) colorTo = GetColorValue();
        }

        public void RecordFrom()
        {
            if (move) moveFrom = this.transform.position;
            if (rotate) rotateFrom = this.transform.eulerAngles;
            if (scale) scaleFrom = this.transform.localScale;
            if (color) colorFrom = GetColorValue();
        }
    }
    
    
}

