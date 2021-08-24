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
    
    
    
    
    
    [RequireComponent(typeof(Data.DataBlock))]
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
        
        [Header("轨道")]
        [SerializeField]
        public List<ProtaAnimationTrack> tracks = new List<ProtaAnimationTrack>();
        
        
        public DataBlock dataBlock => this.GetComponent<DataBlock>();
        
        
        
        public void Set(float t)
        {
            if(t >= duration + 1e-6f) t = t % duration;
            foreach(var track in tracks) track.Apply(dataBlock, t);
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
        
        public void Deserialize(string assetName, UseAnimationAssetMode mode = UseAnimationAssetMode.Replace)
        {
            var asset = ProtaSpriteDatabase.instance[assetName];
            Debug.Assert(asset != null);
            Deserialize(assetName, mode);
        }
        
        public void Deserialize(ProtaAnimationAsset asset, UseAnimationAssetMode mode = UseAnimationAssetMode.Replace)
        {
            if(mode == UseAnimationAssetMode.Replace) tracks.Clear();
            foreach(var trackAsset in asset.tracks)
            {
                trackAsset.data.Reset();
                var track = Activator.CreateInstance(ProtaAnimationTrack.types[trackAsset.type]) as ProtaAnimationTrack;
                track.Deserialize();
                tracks.Add(track);
            }
        }
        
        public void Serialize(ProtaAnimationAsset asset)
        {
            foreach(var track in tracks) asset.Add(track);
        }
    }
}