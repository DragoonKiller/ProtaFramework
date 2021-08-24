using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using Prota.Data;

namespace Prota.Animation
{
    /// <summary>
    /// 运行时 Track 的基础类型.
    /// </summary>
    [Serializable]
    public partial class ProtaAnimationTrack
    {
        
        [SerializeField]
        public string name = "";
        
        [SerializeField]
        public ProtaAnimationTrackAsset asset = new ProtaAnimationTrackAsset();
        
        [NonSerialized]
        bool deserialized;
        
        public string type => this.GetType().Name;
        
        public void Apply(DataBlock anim, float t)
        {
            if(!deserialized) Deserialize();
        }
        
        protected virtual void OnApply(DataBlock anim, float t) { }
        
        public virtual void Serialize() { }
        
        public virtual void Deserialize() { }


        public static IReadOnlyDictionary<string, Type> types => _types;
        static Dictionary<string, Type> _types = new Dictionary<string, Type>(); 
        
        static ProtaAnimationTrack()
        {
            foreach(var i in typeof(ProtaAnimationTrack).GetNestedTypes())
            {
                if(!typeof(ProtaAnimationTrack).IsAssignableFrom(i)) continue;
                _types.Add(i.Name, i);
            }
        }
    }
}