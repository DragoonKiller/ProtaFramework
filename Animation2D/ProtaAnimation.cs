using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using Prota.Data;

namespace Prota.Animation
{
    public enum UseAnimationAssetMode
    {
        Replace,
        Append,
    }
    
    
    
    [RequireComponent(typeof(Data.DataBlock))]
    [ExecuteAlways]
    [SerializeField]
    public class ProtaAnimation : ProtaScript
    {
        [Header("时间")]
        [SerializeField]
        public float duration;
        
        // 当前时间.
        [SerializeField]
        public float time;
        
        [Header("自动播放")]
        
        // 是否在编辑器下自动播放.
        [SerializeField]
        public bool executeInEditor;
        
        // 是否自动播放.
        [SerializeField]
        public bool autoUpdate;
        
        // 只有自动播放时才生效.
        // true: 循环播放
        // false: 时间到末尾以后, 自动播放暂停.
        [SerializeField]
        public bool loop;
        
        [Header("轨道")]
        [SerializeField]
        public List<ProtaAnimationTrack> tracks = new List<ProtaAnimationTrack>();
        
        
        public DataBlock dataBlock => this.GetComponent<DataBlock>();
        
        
        public void Set(float t)
        {
            if(t >= duration + 1e-6f) t = t % duration;
            foreach(var track in tracks) track.Apply(dataBlock, t);
        }
        
        const int updatePerFrameLimit = 100;
        void Update()
        {
            if(!executeInEditor && Application.isPlaying) return;
            if(!autoUpdate) return;
            
            time += Time.deltaTime;
            int guard = 0;
            
            if(loop)
            {
                while(time > duration && ++guard < updatePerFrameLimit) time -= duration;
                if(guard >= updatePerFrameLimit) Debug.LogError("duration 设置太小了!!!", this);
            }
            else
            {
                time = Mathf.Min(duration, time);
            }
            
            Set(time);
        }
        
        public void UseAsset(ProtaAnimationAsset asset, UseAnimationAssetMode mode = UseAnimationAssetMode.Replace)
        {
            if(mode == UseAnimationAssetMode.Replace) tracks.Clear();
            foreach(var i in tracks)
            {
                
            }
        }
        
        
        
    }
}