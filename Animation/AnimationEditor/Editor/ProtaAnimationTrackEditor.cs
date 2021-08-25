using System;
using System.Collections.Generic;
using System.Linq;
using Prota.Animation;
using UnityEditor;

namespace Prota.Editor
{
    [InitializeOnLoad]
    public abstract partial class ProtaAnimationTrackEditor
    {
        
        public float time;
        
        public ProtaAnimationTrack track;
        
        public abstract void UpdateTrackContent(ProtaAnimationTrackContent content);
        
        protected class TrackEditorAttribute : Attribute
        {
            public Type trackType;

            public TrackEditorAttribute(Type trackType)
            {
                this.trackType = trackType;
            }
        }
        
        
        
        
        public static Dictionary<Type, Type> types = new Dictionary<Type, Type>();
        
        static ProtaAnimationTrackEditor()
        {
            foreach(var t in typeof(ProtaAnimationTrackEditor).GetNestedTypes())
            {
                if(typeof(ProtaAnimationTrackEditor).IsAssignableFrom(t))
                {
                    var g = t.GetCustomAttributes(typeof(TrackEditorAttribute), true);
                    foreach(var attr in g)
                    {
                        if(!(attr is TrackEditorAttribute p)) continue;
                        types.Add(p.trackType, t);
                    }
                }
            }
            
            UnityEngine.Debug.Log("可用的Track编辑器: " + types.Select(x => x.Key.Name).Aggregate("", (a, x) => a + " " + x));
        }
    }
}