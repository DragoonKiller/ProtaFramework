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
        
        public virtual void Apply(DataBlock anim, float t) { }
        
        public virtual void Serialize(ProtaAnimationTrackAsset a) => throw new NotImplementedException();
        
        public virtual void Deserialize(ProtaAnimationTrackAsset a) => throw new NotImplementedException();


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