using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Animation
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ProtaAnimationState : MonoBehaviour
    {
        // ========== 动画资源 ==========
        
        [SerializeField]
        ProtaAnimationAsset _asset;
        public ProtaAnimationAsset asset
        {
            get => _asset;
            set
            {
                var original = _asset;
                _asset = value;
                if(original != value) UpdateAnimation(); 
            }
        }
        
        
        // ========== 当前状态 ==========
        
        [SerializeField]
        float _time;
        public float time { get => _time; private set => _time = value; }
        
        
        
        // ========== 播放设置 ==========
        
        // 自动播放.
        public bool playAutomatic;
        
        // 重复播放.
        public bool repeat;
        
        // ============================================================================================================
        // 自动播放
        // ============================================================================================================
        
        void Update()
        {
            if(playAutomatic)
            {
                time += Time.deltaTime;
                UpdateAnimation();
            }
        }
        
        public void Reset() => SetTime(0);
        
        public void UpdateAnimation()
        {
            if(asset == null) return;
            foreach(var track in asset.tracks) track.Apply(this);
        }
        
        // ============================================================================================================
        // 独立帧控制
        // ============================================================================================================
        
        public void Refresh() => SetTime(time);
        
        public void SetTime(float t)
        {
            time = t;
            UpdateAnimation();
        }
        
        // ============================================================================================================
        // 其它
        // ============================================================================================================
        
        
    }
    
    
    
    
    // ================================================================================================================
    // 动画配置
    // ================================================================================================================
    
    /// <summary>
    /// 表示动画中的一帧.
    /// </summary>
    [Serializable]
    public class ProtaSpriteAnimationFrame
    {
        public Sprite sprite;       // 当前帧使用的 Sprite.
        public Vector2 offset;      // 从上一帧播放到这一帧时, 角色的 pivot 会移动这么多.
    }
    
    
    /// <summary>
    /// 表示一段动画.
    /// </summary>
    public class ProtaSpriteAnimationAsset : ScriptableObject
    {
        // ========== 内部存储的属性 ==========
        
        // Sprite 列表.
        [SerializeField]
        List<ProtaSpriteAnimationFrame> frameList;
        
        [SerializeField]
        float framePerSec;
        
        // ========== 外部接口 ==========
        public float fps => framePerSec;
        
        
        
        
        
        
        
        
        
        
    }
    
}