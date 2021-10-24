using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using Prota.Data;
using System.Collections;

namespace Prota.Animation
{
    public enum UseAnimationAssetMode
    {
        Replace,
        Append,
    }
    
    
    [RequireComponent(typeof(DataCore))]        // Track 会从这个 DataCore 读取/赋值数据.
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
        
        // 是否自动播放.
        [SerializeField]
        public bool autoUpdate;
        
        // 只有自动播放时才生效.
        // true: 循环播放
        // false: 时间到末尾以后, 自动播放暂停.
        [SerializeField]
        public bool loop;
        
        
        [Header("资源")]
        public ProtaAnimationAsset asset;
        
        
        [Header("轨道")]
        public List<ProtaAnimationTrack> runtimeTracks = new List<ProtaAnimationTrack>();
        
        public void Set(float t)
        {
            if(t >= duration + 1e-6f) t = t % duration;
            foreach(var track in runtimeTracks) track.Apply(this.GetComponent<DataCore>(), t);
        }
        
        void Awake()
        {
            Debug.LogError(typeof(ProtaAnimation).FullName);
        }
        
        const int updatePerFrameLimit = 100;
        void Update()
        {
            Debug.Log(Time.deltaTime);
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
        
        public void UseAsset(ProtaAnimationAsset asset = null, UseAnimationAssetMode mode = UseAnimationAssetMode.Replace)
        {
            if(asset == null) asset = this.asset;
            if(asset == null)
            {
                Debug.LogWarning("给定 Animation 没有装载 Asset");
                return;
            }
            
            if(mode == UseAnimationAssetMode.Replace) runtimeTracks.Clear();
            foreach(var trackAsset in asset.tracks)
            {
                runtimeTracks.Add(trackAsset.Instantiate());
            }
        }
        
        public void SaveAsset(ProtaAnimationAsset asset = null)
        {
            if(asset == null) asset = this.asset;
            if(asset == null)
            {
                Debug.LogError("没有需要保存的 asset!");
                return;
            }
            
            asset.Clear();
            foreach(var track in runtimeTracks) asset.Add(track);
        }
        
        public void Reset()
        {
            runtimeTracks.Clear();
        }
    }
}