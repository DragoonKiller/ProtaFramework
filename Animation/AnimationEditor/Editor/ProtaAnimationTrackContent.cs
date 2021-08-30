using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Animation;
using System;
using Prota.Unity;

namespace Prota.Editor
{

    public class ProtaAnimationTrackContent
    {
        public VisualElement root;
        
        public Label id { get; private set; }
        public Label type { get; private set; }
        public TextField trackName { get; private set; }
        public VisualElement track { get; private set; }
        public VisualElement trackLine { get; private set; }
        public VisualElement trackPanel { get; private set; }
        public VisualElement timeStamp { get; private set; }
        public VisualElement trackContent { get; private set; }
        public VisualElement dataPanel { get; private set; }
        
        public Action onRefresh;
        
        public Action onSetTime;
        
        public Action onSetRange;
        
        public bool folded;
        
        static readonly Color hoverColor = new Color(.1f, .15f, .3f, 1);
        static Color stdColor;
        
        
        public ProtaAnimationTrackContent(VisualElement r)
        {
            this.root = r.Q("Template");
            id = r.Q<Label>("Id");
            type = r.Q<Label>("Type");
            track = r.Q("Track");
            trackName = r.Q<TextField>("TrackName");
            trackLine = r.Q("TrackLine");
            trackPanel = r.Q("TrackPanel");
            timeStamp = r.Q("TimeStamp");
            trackContent = r.Q("TrackContent").SetFixedSize().SetAbsolute();
            dataPanel = r.Q("DataPanel");
            
            trackPanel.RegisterCallback<GeometryChangedEvent>(e => {
                trackContent.SetWidth(trackPanel.resolvedStyle.width).SetHeight(trackPanel.resolvedStyle.height);
            });
            
            stdColor = type.style.backgroundColor.value;
            type.RegisterCallback<MouseEnterEvent>(e => {
                type.style.backgroundColor = hoverColor;
            });
            type.RegisterCallback<MouseLeaveEvent>(e => {
                type.style.backgroundColor = stdColor;
            });
            type.RegisterCallback<ClickEvent>(e => {
                folded = !folded;
                track.SetVisible(!folded);
                onRefresh?.Invoke();
            });
            
            folded = true;
            track.SetVisible(false);
            onRefresh?.Invoke();
        }
        
        public ProtaAnimationTrackContent SetRange(int lpx, int rpx)
        {
            trackContent.style.left = lpx;
            trackContent.style.right = rpx;
            onSetRange?.Invoke();
            return this;
        }
        
        
        public ProtaAnimationTrackContent SetTimePos(Length lpx)
        {
            timeStamp.style.left = lpx;
            onSetTime?.Invoke();
            return this;
        }
        
        
        
        
    }   
    
    
}