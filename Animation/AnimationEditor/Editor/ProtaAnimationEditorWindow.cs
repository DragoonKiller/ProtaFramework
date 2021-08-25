using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.CommonResources;
using System.Collections.Generic;
using System;
using Prota.Animation;
using Prota.Unity;

namespace Prota.Editor
{
    
    
    

    public class ProtaAnimationEditorWindow : EditorWindow
    {
        [MenuItem("ProtaFramework/动画/动画编辑器 _F4")]
        public static void OpenWindow()
        {
            ProtaAnimationEditorWindow wnd = GetWindow<ProtaAnimationEditorWindow>();
            wnd.titleContent = new GUIContent("动画编辑器");
        }
        
        [MenuItem("ProtaFramework/动画/动画编辑器(重启) %F4")]
        public static void CloseWindow()
        {
            ProtaAnimationEditorWindow wnd = GetWindow<ProtaAnimationEditorWindow>();
            wnd.Close();
        }
        
        
        static Storage storage = new Storage();
        
        
        VisualElement root;
        
        
        public void OnEnable()
        {
            EditorApplication.update += Update;
        }
        
        public void CreateGUI()
        {
            try
            {
                LoadRoot();
                SetupAddTrack();
                SetupTimeline();
                SetupTimelineHeader();
                SetupAllTracksRoot();
                InitializeValue();
            }
            catch(Exception e) { Debug.LogException(e); }
        }
        
        public void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        
        void LoadRoot()
        {
            var visualTreeAsset = ResourcesDatabase.inst["Animation"]["ProtaAnimationEditorWindow"] as VisualTreeAsset;
            root = visualTreeAsset.CloneTree();
            rootVisualElement.Add(root);
        }
        
        public Dictionary<string, Action> onDestory = new Dictionary<string, Action>();
        void OnDestroy()
        {
            foreach(var i in onDestory) i.Value();
        }
        
        
        void InitializeValue()
        {
            time.value = 0;
            if(storage.anim != null) target.value = storage.anim;
        }
        
        
        // ============================================================================================================
        // 添加 Track
        // ============================================================================================================
        
        string addTrackName;
        
        void SetupAddTrack()
        {
            var name = root.Q<TextField>("AddTrackName");
            name.RegisterValueChangedCallback(e => {
                name.viewDataKey = e.newValue;
                addTrackName = e.newValue;
            });
            var button = root.Q<Button>("AddTrack");
            button.RegisterCallback<ClickEvent>(e => AddTrack(addTrackName));
        }
        
        void AddTrack(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) return;
            if(!ProtaAnimationTrack.types.TryGetValue(name, out var trackType)) return;
            var track = Activator.CreateInstance(trackType) as ProtaAnimationTrack;
            animation.runtimeTracks.Add(track);
            EditorUtility.SetDirty(animation);
            track.name = name + "Track";
            SetupTracks();
        }
        
        
        
        // ============================================================================================================
        // 事件
        // ============================================================================================================
        
        
        Action onUpdate;
        void Update() => onUpdate?.Invoke();
        
        
        // ============================================================================================================
        // 播放和题头
        // ============================================================================================================
        
        FloatField time;
        Button readButton;
        Button saveButton;
        Button startButton;
        Button recordButton;
        Button resetButton;
        FloatField duration;
        FloatField timeFrom;
        FloatField timeTo;
        ObjectField target;
        ObjectField asset;
        ProtaAnimation animation => target?.value as ProtaAnimation;
        
        
        
        
        double recordTime;
        
        bool _playing;
        bool playing
        {
            get => _playing;
            set
            {
                if(_playing == value) return;
                _playing = value;
                
                if(value)
                {
                    startButton.text = "停止";
                    recordTime = EditorApplication.timeSinceStartup;
                }
                else
                {
                    startButton.text = "开始";
                }
            }
        }
        
        
        
        void SetupTimelineHeader()
        {
            target = root.Q<ObjectField>("Target");
            target.objectType = typeof(ProtaAnimation);
            target.RegisterValueChangedCallback(e => {
                var anim = e.newValue as ProtaAnimation;
                if(anim == null)
                {
                    
                }
                else
                {
                    storage.anim = anim;
                    anim.Reset();
                    duration.value = anim.duration;
                    timeFrom.value = 0;
                    timeTo.value = timeFrom.value + anim.duration;
                    if(anim.asset != null)
                    {
                        asset.value = anim.asset;
                        anim.UseAsset();
                    }
                }
                SetupTracks();
                UpdateTimeStamp();
                UpdateMarks();
                UpdateTrackTime();
            });
            
            asset = root.Q<ObjectField>("Asset");
            asset.objectType = typeof(ProtaAnimationAsset);
            asset.RegisterValueChangedCallback(e => {
                animation.asset = e.newValue as ProtaAnimationAsset;
                var activate = e.newValue != null;
                asset.viewDataKey = e.newValue?.GetInstanceID().ToString();
                readButton.SetEnabled(activate);
                saveButton.SetEnabled(activate); 
                EditorUtility.SetDirty(animation);
            });
            
            readButton = root.Q<Button>("ReadButton");
            readButton.RegisterCallback<ClickEvent>(e => animation.UseAsset());
            
            saveButton = root.Q<Button>("SaveButton");
            saveButton.RegisterCallback<ClickEvent>(e => animation.SaveAsset());
            
            time = root.Q<FloatField>("CurrentTime");
            time.RegisterValueChangedCallback(e => {
                if(!(timeFrom.value <= time.value && time.value <= timeTo.value))
                {
                    time.value = time.value.Max(timeFrom.value).Min(timeTo.value);
                    return;
                }
                UpdateTimeStamp();
                UpdateTrackTime();
            });
            
            
            
            startButton = root.Q<Button>("StartButton");
            startButton.RegisterCallback<ClickEvent>(e => {
                playing = !playing;
            });
            
            resetButton = root.Q<Button>("ResetButton");
            resetButton.RegisterCallback<ClickEvent>(e => {
                time.value = 0;
            });
            
            recordButton = root.Q<Button>("RecordButton");
            recordButton.RegisterCallback<ClickEvent>(e => {
                Debug.Log("TODO");
            });
            
            duration = root.Q<FloatField>("Duration");
            duration.RegisterValueChangedCallback(e => {
                timeTo.value = timeTo.value.Min(e.newValue);
                if(animation)
                {
                    animation.duration = e.newValue;
                    EditorUtility.SetDirty(animation);
                }
                time.value = time.value.Min(duration.value);
                time.value = time.value;
            });
            
            timeFrom = root.Q<FloatField>("TimeFrom");
            timeFrom.RegisterValueChangedCallback(e => {
                timeTo.value = timeTo.value.Max(e.newValue + 1e-4f);
                time.value = time.value;
                UpdateTimeStamp();
                UpdateTrackTimeStamp();
            });
            
            timeTo = root.Q<FloatField>("TimeTo");
            timeTo.RegisterValueChangedCallback((EventCallback<ChangeEvent<float>>)(e => {
                timeTo.value = Unity.MethodExtensions.Max(timeTo.value, this.timeFrom.value + 1e-4f);
                time.value = time.value;
                UpdateTimeStamp();
                UpdateTrackTimeStamp();
            }));
            
            onUpdate += () => {
                if(!playing) return;
                var ccTime = EditorApplication.timeSinceStartup;
                time.value = (float)(ccTime - recordTime);
            };
            
        }
        
        // ============================================================================================================
        // 时间轴
        // ============================================================================================================
        
        VisualElement timeline;
        VisualElement timelineMarkRoot;
        VisualElement timeStamp;
        
        void SetupTimeline()
        {
            const float scrollScale = 0.05f;
            
            bool inField = false;
            bool pressing = false;
            
            timeline = root.Q("Timeline");
            timeStamp = root.Q("TimeStamp");
            
            var originalColor = timeline.resolvedStyle.backgroundColor;
            var activeColor = originalColor + new Color(0.05f, 0.05f, 0.05f, 0f);
            
            void SetTime(IMouseEvent e)
            {
                var rate = e.localMousePosition.x / timeline.resolvedStyle.width;
                time.value = rate.XMap(0, 1, timeFrom.value, timeTo.value);
                UpdateTimeStamp();
                UpdateMarks();
                UpdateTrackTime();
            }
            
            timeline.RegisterCallback<GeometryChangedEvent>(e => {
                UpdateTrackContentLayout();
            });
            
            timeline.RegisterCallback<MouseEnterEvent>(e => {
                inField = true;
                timeline.style.backgroundColor = activeColor;
            });
            
            timeline.RegisterCallback<MouseLeaveEvent>(e => {
                inField = false;
                pressing = false;
                timeline.style.backgroundColor = originalColor;
            });
            
            timeline.RegisterCallback<MouseDownEvent>(e => {
                if(e.button != 0) return;
                pressing = true;
                SetTime(e);
            });
            
            timeline.RegisterCallback<MouseUpEvent>(e => {
                if(e.button != 0) return;
                pressing = false;
            });
            
            timeline.RegisterCallback<MouseMoveEvent>(e => {
                if(!inField || !pressing) return;
                SetTime(e);
            });
            
            timeline.RegisterCallback<WheelEvent>(e => {
                if(!inField) return;
                var scaleFactor = (e.delta.y * scrollScale).Exp();
                var cur = e.localMousePosition.x.XMap(0, timeline.resolvedStyle.width, timeFrom.value, timeTo.value);
                var distL = cur - timeFrom.value;
                var distR = timeTo.value - cur;
                distL *= scaleFactor;
                distR *= scaleFactor;
                timeFrom.value = cur - distL;
                timeTo.value = cur + distR;
                UpdateMarks();
                UpdateTimeStamp();
                UpdateTrackTimeStamp();
            });
        }
        
        void UpdateTimeStamp()
        {
            if(time.value < timeFrom.value) time.value = timeFrom.value;
            if(time.value > timeTo.value) time.value = timeTo.value;
            
            var pos = time.value.XMap(timeFrom.value, timeTo.value, 0, timeline.resolvedStyle.width);
            
            var pp = timeStamp.style.left;
            pp.value = new Length(pos - timeStamp.resolvedStyle.width * 0.5f, LengthUnit.Pixel);
            timeStamp.style.left = pp;
        }
        readonly List<VisualElement> marks = new List<VisualElement>();
        void UpdateMarks()
        {
            if(timelineMarkRoot == null)
            {
                timelineMarkRoot = new VisualElement();
                timelineMarkRoot.style.flexGrow = 1;
                timelineMarkRoot.style.flexShrink = 0;
                timeline.Add(timelineMarkRoot);
                timelineMarkRoot.SendToBack();
            }
            
            foreach(var mark in marks) timelineMarkRoot.Remove(mark);
            marks.Clear();
            
            VisualElement NewNormalMark()
            {
                var x = new VisualElement();
                x.style.flexShrink = 0;
                x.style.flexGrow = 1;
                x.style.position = Position.Absolute;
                x.style.maxHeight = new StyleLength(StyleKeyword.None);
                x.style.minHeight = new StyleLength(StyleKeyword.None);
                x.style.height = timeline.resolvedStyle.height;
                x.style.width = 1;
                x.style.maxWidth = 1;
                x.style.minWidth = 1;
                x.pickingMode = PickingMode.Ignore;
                return x;
            }
            
            // 时间标记.
            for(var i = timeFrom.value.CeilToInt(); i <= timeTo.value.FloorToInt(); i++)
            {
                if(i < 0 || i >= duration.value) continue;
                var pos = ((float)i).XMap(timeFrom.value, timeTo.value, 0, timeline.resolvedStyle.width);
                var x = NewNormalMark();
                x.style.left = pos;
                x.style.backgroundColor = new Color(.05f, .05f, .05f, 1);
                marks.Add(x);
                timelineMarkRoot.Add(x);
            }
            
            // 开始和结束时间标记.
            if(timeFrom.value <= 1e-6f)
            {
                var x = NewNormalMark();
                var pos = (0f).XMap(timeFrom.value, timeTo.value, 0, timeline.resolvedStyle.width);
                x.style.width = 3;
                x.style.minWidth = 3;
                x.style.maxWidth = 3;
                x.style.left = pos;
                x.style.backgroundColor = new Color(1, 0.4f, 0.4f, 1);
                marks.Add(x);
                timelineMarkRoot.Add(x);
            }
            if(duration.value <= timeTo.value)
            {
                var x = NewNormalMark();
                var pos = duration.value.XMap(timeFrom.value, timeTo.value, 0, timeline.resolvedStyle.width);
                x.style.width = 3;
                x.style.minWidth = 3;
                x.style.maxWidth = 3;
                x.style.left = pos - x.style.width.value.value;
                x.style.left = pos;
                x.style.backgroundColor = new Color(1, 0.4f, 0.4f, 1);
                marks.Add(x);
                timelineMarkRoot.Add(x);
            }
        }
        
        
        // ============================================================================================================
        // Track 生成和维护
        // ============================================================================================================
        
        
        VisualElement trackRoot;
        
        List<ProtaAnimationTrackContent> trackContents = new List<ProtaAnimationTrackContent>();
        
        List<ProtaAnimationTrackEditor> trackEditors = new List<ProtaAnimationTrackEditor>();
        
        void SetupAllTracksRoot()
        {
            trackRoot = root.Q<ScrollView>("Tracks");
            trackRoot.Clear();
        }
        
        // 重新绑定各个 track 到编辑器上.
        void SetupTracks(ProtaAnimation anim = null)
        {
            if(anim == null) anim = this.animation;
            
            var oriCnt = trackContents.Count;
            var originalState = new List<bool>();
            for(int i = 0; i < oriCnt; i++)
            {
                originalState.Add(trackContents[i].folded);
            }
            
            foreach(var content in trackContents) trackRoot.Remove(content.root);
            trackRoot.Clear();
            trackContents.Clear();
            trackEditors.Clear();
            
            if(anim == null) return;
            
            for(int i = 0; i < anim.runtimeTracks.Count; i++)
            {
                var track = anim.runtimeTracks[i];
                var trackContentRes = ResourcesDatabase.inst["Animation"]["ProtaAnimationTrackContent"] as VisualTreeAsset;
                var trackContent = new ProtaAnimationTrackContent(trackContentRes.CloneTree());
                trackRoot.Add(trackContent.root);
                trackContents.Add(trackContent);
                var editorType = ProtaAnimationTrackEditor.types[track.GetType()];
                var trackEditor = Activator.CreateInstance(editorType) as ProtaAnimationTrackEditor;
                trackEditors.Add(trackEditor);
                trackContent.id.text = i.ToString();
                trackContent.type.text = track.GetType().Name;
                trackContent.trackName.value = track.name;
                trackContent.trackName.RegisterValueChangedCallback(a => track.name = a.newValue);
                var deleteButton = trackContent.root.Q<Button>("Delete");
                deleteButton.RegisterCallback<ClickEvent>((EventCallback<ClickEvent>)(e => {
                    anim.runtimeTracks.RemoveAt(i);
                    SetupTracks();
                }));
                if(i < oriCnt) trackContent.folded = originalState[i];
            }
            
            
            UpdateTrackContentLayout();
            ApplyTimeToTracks();
            trackRoot.MarkDirtyRepaint();
            UpdateTrackTime();
        }
        
        void UpdateTrackContentLayout()
        {
            var space = root.Q("TimelineSpace");
            foreach(var content in trackContents)
            {
                content.dataPanel.style.maxWidth = space.resolvedStyle.maxWidth.value;
                content.dataPanel.style.minWidth = space.resolvedStyle.minWidth.value;
                content.dataPanel.style.width = space.resolvedStyle.width;
                content.dataPanel.MarkDirtyRepaint();
            }
        }
        
        void UpdateTrackTime()
        {
            UpdateTrackTimeStamp();
            ApplyTimeToTracks();
        }
        
        void UpdateTrackTimeStamp()
        {
            var pos = time.value.XMap(timeFrom.value, timeTo.value, 0, timeline.resolvedStyle.width);
            for(int i = 0; i < trackContents.Count; i++)
            {
                var content = trackContents[i];
                content.SetTimePos(pos);
            }
        }
        
        void ApplyTimeToTracks()
        {
            for(int i = 0; i < trackEditors.Count; i++)
            {
                trackEditors[i].time = time.value;
                trackEditors[i].track = animation.runtimeTracks[i];
                trackEditors[i].UpdateTrackContent(trackContents[i]);
            }
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        [Serializable]
        class Storage
        {
            public ProtaAnimation anim;
            
        }
        
    }   
    
    
}