using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Animation;
using System;

namespace Prota.Editor
{

    public class ProtaAnimationTrackContent
    {
        public VisualElement root;
        
        public Label id;
        public TextField trackName;
        public VisualElement track;
        public VisualElement trackLine;
        public VisualElement trackPanel;
        public VisualElement timeStamp;
        public VisualElement trackContent;
        public VisualElement dataPanel;
        
        public Action onSetTime;
        
        public Action onSetRange;
        
        
        public ProtaAnimationTrackContent(VisualElement r)
        {
            this.root = r;
            id = r.Q<Label>("Id");
            trackName = r.Q<TextField>("TrackName");
            trackLine = r.Q("TrackLine");
            trackPanel = r.Q("TrackPanel");
            timeStamp = r.Q("TimeStamp");
            trackContent = r.Q("TrackContent");
            dataPanel = r.Q("DataPanel");
        }
        
        public ProtaAnimationTrackContent SetRange(int lpx, int rpx)
        {
            trackContent.style.left = lpx;
            trackContent.style.right = rpx;
            onSetRange?.Invoke();
            return this;
        }
        
        
        public ProtaAnimationTrackContent SetTime(int lpx)
        {
            timeStamp.style.left = lpx;
            onSetTime?.Invoke();
            return this;
        }
        
        
        
        
    }   
    
    
}