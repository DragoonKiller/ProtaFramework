using UnityEngine;
using Prota.Unity;

namespace Prota.Tween
{
    [ExecuteAlways]
    public class ProtaTweener : MonoBehaviour
    {
        [Range(0, 1)] public float currentRatio = 0;
        
        [Header("自动播放")]
        public bool autoPlay = true;
        
        // 播放时长. 决定了播放速度.
        [ShowWhen("autoPlay")] public float duration = 1;
        
        [ShowWhen("autoPlay")] public bool loop = false;
        
        // 指示当前的更新是从 0 到 1 还是反过来.
        [ShowWhen("autoPlay")] public bool playReversed = false;
        
        // 循环一次后反转 from/to.
        [ShowWhen("autoPlay")] public bool reverseOnLoop = false;
        
        [Header("移动")]
        public bool move;
        
        [ShowWhen("move")] public Vector3 moveFrom = Vector3.zero;
        [ShowWhen("move")] public Vector3 moveTo = Vector3.zero;
        [ShowWhen("move")] public TweenEaseEnum moveEase = TweenEaseEnum.Linear;
        
        [Header("旋转")]
        public bool rotate;
        [ShowWhen("rotate")] public Vector3 rotateFrom = Vector3.zero;
        [ShowWhen("rotate")] public Vector3 rotateTo = Vector3.zero;
        [ShowWhen("rotate")] public TweenEaseEnum rotateEase = TweenEaseEnum.Linear;
        
        [Header("缩放")]
        public bool scale;
        [ShowWhen("scale")] public Vector3 scaleFrom = Vector3.one;
        [ShowWhen("scale")] public Vector3 scaleTo = Vector3.one;
        [ShowWhen("scale")] public TweenEaseEnum scaleEase = TweenEaseEnum.Linear;
        
        [Header("颜色")]
        public bool color;
        [ShowWhen("color"), Readonly] public UnityEngine.Object colorTarget;
        [ShowWhen("color")] public Color colorFrom = Color.white;
        [ShowWhen("color")] public Color colorTo = Color.white;
        [ShowWhen("color")] public TweenEaseEnum colorEase = TweenEaseEnum.Linear;
        
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
            
            if(playReversed) currentRatio -= Time.deltaTime / duration;
            else currentRatio += Time.deltaTime / duration;
            
            if(loop)
            {
                if(currentRatio >= 1 && reverseOnLoop) playReversed = !playReversed;
                currentRatio = Mathf.Repeat(currentRatio, 1);
            }
            else
            {
                currentRatio = currentRatio.Clamp(0, 1);
            }
            
            UpdateValue();
            
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public void UpdateValue()
        {
            // move
            if(move) this.transform.position = (moveFrom, moveTo).Lerp(TweenEase.GetFromEnum(moveEase).Evaluate(currentRatio));
            
            // rotate
            if(rotate) this.transform.eulerAngles = (rotateFrom, rotateTo).Lerp(TweenEase.GetFromEnum(rotateEase).Evaluate(currentRatio));
            
            // scale
            if(scale) this.transform.localScale = (scaleFrom, scaleTo).Lerp(TweenEase.GetFromEnum(scaleEase).Evaluate(currentRatio));
            
            // color
            if(color) UpdateColorValue(TweenEase.GetFromEnum(colorEase).Evaluate(currentRatio));
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
        
        public void SetTo(float ratio)
        {
            this.currentRatio = ratio;
            UpdateValue();
        }
        
        public void SetToFinish()
        {
            currentRatio = 1;
            Update();
        }
        
        public void SetToStart()
        {
            currentRatio = 0;
            Update();
        }
        
        public void SetTime(float time)
        {
            currentRatio = time / duration;
            Update();
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
            if(currentRatio == 0) RecordFrom();
            if(currentRatio == 1) RecordTo();
            return currentRatio == 0 || currentRatio == 1;
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

