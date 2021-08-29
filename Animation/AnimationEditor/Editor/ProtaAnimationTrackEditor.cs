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
        
        public float timeFrom;
        
        public float timeTo;
        
        public float displayWidth;
        
        public ProtaAnimation animation;
        
        public ProtaAnimationTrack track;
        
        public ProtaAnimationTrackContent content;
        
        public abstract void UpdateTrackContent();
        
        public void PrepareLayout()
        {
            content.trackContent.style.width = content.trackContent.style.minWidth = content.trackContent.style.maxWidth = content.trackPanel.resolvedStyle.width;
            content.trackContent.style.height = content.trackContent.style.minHeight = content.trackContent.style.maxHeight = content.trackPanel.resolvedStyle.height;
            content.trackContent.style.left = 0;
            content.trackContent.style.top = 0;
        }
        
        
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