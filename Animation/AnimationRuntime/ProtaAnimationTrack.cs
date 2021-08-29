using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using Prota.Data;
using UnityEditor;
using System.Linq;

namespace Prota.Animation
{
    /// <summary>
    /// 运行时 Track 的基础类型.
    /// </summary>
    [Serializable]
    [InitializeOnLoad]
    public partial class ProtaAnimationTrack
    {
        
        [SerializeField]
        public string name = "";
        
        [NonSerialized]
        bool deserialized;
        
        public string type => this.GetType().Name;
        
        public virtual void Apply(GameObject anim, float t) { }
        
        public void Serialize(ProtaAnimationTrackAsset a)
        {
            a.data.Reset();
            a.name = name;
            a.type = this.GetType().Name;
            OnSerialize(a);
        }
        
        public void Deserialize(ProtaAnimationTrackAsset a)
        {
            a.data.Reset();
            name = a.name;
            this.OnDeserialize(a);
        }
        
        protected virtual void OnSerialize(ProtaAnimationTrackAsset a) { }
        protected virtual void OnDeserialize(ProtaAnimationTrackAsset a) { }



        public static IReadOnlyDictionary<string, Type> types => _types;
        static Dictionary<string, Type> _types = new Dictionary<string, Type>(); 
        
        static ProtaAnimationTrack()
        {
            foreach(var i in typeof(ProtaAnimationTrack).GetNestedTypes())
            {
                if(!typeof(ProtaAnimationTrack).IsAssignableFrom(i)) continue;
                _types.Add(i.Name, i);
            }
            
            UnityEngine.Debug.Log("可用的Track: " + types.Select(x => x.Key).Aggregate("", (a, x) => a + " " + x));
        }
    }
}