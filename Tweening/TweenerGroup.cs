using UnityEngine;
using Prota.Unity;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Prota.Tween
{
    [ExecuteAlways]
    public class ProtaTweenerGroup : MonoBehaviour
    {
        [Serializable]
        public class TweenEntry
        {
            public ProtaTweener tweener;
            public bool activeWhenNotPlay = true;
            [Range(0, 1)] public float from = 0;
            [Range(0, 1)] public float to = 1;
        }
        
        [Header("播放")]
        public bool autoPlay = true;
        [ShowWhen("autoPlay")] public float duration = 1;
        [ShowWhen("autoPlay")] public bool playReversed = false;
        [ShowWhen("autoPlay")] public bool reverseOnLoop = false;
        [ShowWhen("autoPlay")] public bool loop = false;
        [ShowWhen("autoPlay")] public TweenEaseEnum ease = TweenEaseEnum.Linear;
        
        [Header("状态")]
        [Range(0, 1)] public float currentRatio = 0;
        
        [Header("录制")]
        public bool recordMode = false;
        
        [Header("Tweeners")]
        [SerializeField] public TweenEntry[] tweeners;
        
        
        void OnValidate()
        {
            Dictionary<ProtaTweener, TweenEntry> e = new Dictionary<ProtaTweener, TweenEntry>();
            if(tweeners != null)
            {
                foreach(var i in tweeners)
                {
                    if(i.tweener == null) continue;
                    e.Add(i.tweener, i);
                }
            }
            
            var tt = this.GetComponentsInChildren<ProtaTweener>();
            foreach(var t in tt) if(!e.ContainsKey(t)) e.Add(t, new TweenEntry() { tweener = t });
            
            tweeners = e.Values.ToArray();
            Array.Sort(tweeners, (a, b) => a.tweener.name.CompareTo(b.tweener.name));
        
            foreach(var t in tweeners) t.tweener.running = false;
        }
        
        
        void Update()
        {
            if(!Application.isPlaying)
            {
                foreach(var tw in tweeners) tw.tweener.recordMode = recordMode;
                if(recordMode) TryRecord();
                else UpdateTweeners();
                return;
            }
            
            recordMode = false;
            
            if(playReversed) currentRatio -= Time.deltaTime / duration;
            else currentRatio += Time.deltaTime / duration;
            
            if(loop)
            {
                if(reverseOnLoop) playReversed = !playReversed;
                currentRatio = Mathf.Repeat(currentRatio, 1);
            }
            
            SetTo(currentRatio);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public void SetTo(float ratio)
        {
            currentRatio = ratio;
            UpdateTweeners();
        }
        
        public void SetToStart() => SetTo(0);
        
        public void SetToEnd() => SetTo(1);
        
        void UpdateTweeners()
        {
            foreach(var tw in tweeners)
            {
                var shouldActivate = (currentRatio >= tw.from && currentRatio <= tw.to) || tw.activeWhenNotPlay;
                var i = TweenEase.GetFromEnum(tw.tweener.moveEase).Evaluate(currentRatio);
                tw.tweener.SetTo(i);
            }
        }
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        public bool TryRecord()
        {
            if(currentRatio == 0) RecordFrom();
            if(currentRatio == 1) RecordTo();
            return currentRatio == 0 || currentRatio == 1;
        }
        
        public void RecordFrom()
        {
            foreach(var tw in tweeners) tw.tweener.RecordFrom();
        }
        
        public void RecordTo()
        {
            foreach(var tw in tweeners) tw.tweener.RecordTo();
        }
        
        
    }
    
    
}

