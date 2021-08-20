using System;
using System.Collections.Generic;

namespace Prota.Editor
{
    public abstract partial class ProtaAnimationTrackEditor
    {
        
        public float time;
        
        public abstract void UpdateTrackContent(ProtaAnimationTrackContent content);
        
        
        public virtual void UpdateTime(float time)
        {
            
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
        }
    }
}