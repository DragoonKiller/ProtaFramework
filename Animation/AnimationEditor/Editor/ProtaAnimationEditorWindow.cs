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
        
        
        
        
        VisualElement root;
        
        
        public void OnEnable()
        {
            EditorApplication.update += Update;
        }
        
        public void CreateGUI()
        {
            LoadRoot();
            SetupAddTrack();
            SetupTimelineHeader();
            SetupTracks();
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
        
        void Update()
        {
            StepFrame();
        }
        
        // ============================================================================================================
        // 添加 Track
        // ============================================================================================================
        
        string addTrackName;
        
        void SetupAddTrack()
        {
            var name = root.Q<TextField>("AddTrackName");
            name.RegisterValueChangedCallback(a => addTrackName = a.newValue);
            var button = root.Q<Button>("AddTrack");
            button.RegisterCallback<ClickEvent>(a => AddTrack(addTrackName));
        }
        
        void AddTrack(string name)
        {
            if(string.IsNullOrWhiteSpace(name)) return;
            if(!ProtaAnimationTrack.types.TryGetValue(name, out var trackType)) return;
            var track = Activator.CreateInstance(trackType) as ProtaAnimationTrack;
            animation.tracks.Add(track);
            track.name = name + "Track";
            SyncTracks();
        }
        
        // ============================================================================================================
        // 播放
        // ============================================================================================================
        
        ProtaAnimation animation;
        
        bool playing;
        
        float time;
        
        double recordTime;
        
        FloatField currentTime;
        
        void SetupTimelineHeader()
        {
            var target = root.Q<ObjectField>("Target");
            target.objectType = typeof(ProtaAnimation);
            target.RegisterCallback<ChangeEvent<UnityEngine.Object>>(a => {
                animation = a.newValue as ProtaAnimation;
                ResetTime();
                SyncTracks();
            });
            
            var startButton = root.Q<Button>("StartButton");
            startButton.RegisterCallback<ClickEvent>(a => {
                playing = !playing;
                if(playing) startButton.text = "停止";
                else startButton.text = "开始";
                if(playing) recordTime = EditorApplication.timeSinceStartup;
                startButton.MarkDirtyRepaint();
            });
            
            var resetButton = root.Q<Button>("ResetButton");
            resetButton.RegisterCallback<ClickEvent>(ResetTime);
            
            var recordButton = root.Q<Button>("RecordButton");
            recordButton.RegisterCallback<ClickEvent>(a => {
                Debug.Log("TODO");
            });
            
            // 完全初始化...
            currentTime = root.Q<FloatField>("CurrentTime");
            time = 0;
            currentTime.value = time;
        }
        
        
        void StepFrame()
        {
            if(!playing) return;
            var ccTime = EditorApplication.timeSinceStartup;
            time = (float)(ccTime - recordTime);
            currentTime.value = time;
        }
        
        void ResetTime(ClickEvent e) => ResetTime();
        void ResetTime()
        {
            recordTime = EditorApplication.timeSinceStartup;
            time = 0;
            currentTime.value = 0;
        }
        
        // ============================================================================================================
        // Track 生成和维护
        // ============================================================================================================
        
        
        VisualElement trackRoot;
        
        List<ProtaAnimationTrackContent> trackContents = new List<ProtaAnimationTrackContent>();
        
        List<ProtaAnimationTrackEditor> trackEditors = new List<ProtaAnimationTrackEditor>();
        
        void SetupTracks()
        {
            trackRoot = root.Q<ScrollView>("Tracks");
            trackRoot.Clear();
            SyncTracks();
        }
        
        // 重新绑定各个 track 到编辑器上.
        void SyncTracks()
        {
            if(animation == null) return;
            
            trackRoot.Clear();
            trackContents.Clear();
            trackEditors.Clear();
            
            for(int i = 0; i < animation.tracks.Count; i++)
            {
                var track = animation.tracks[i];
                var trackContentRes = ResourcesDatabase.inst["Animation"]["ProtaAnimationTrackContent"] as VisualTreeAsset;
                var trackContent = new ProtaAnimationTrackContent(trackContentRes.CloneTree());
                trackRoot.Add(trackContent.root);
                trackContents.Add(trackContent);
                trackEditors.Add(Activator.CreateInstance(ProtaAnimationTrackEditor.types[track.GetType()]) as ProtaAnimationTrackEditor);
                var idLabel = trackContents.Last().root.Q<Label>("Id");
                idLabel.text = i.ToString();
                var typeLabel = trackContents.Last().root.Q<Label>("Type");
                typeLabel.text = track.GetType().Name;
                var trackName = trackContents.Last().root.Q<TextField>("TrackName");
                trackName.value = track.name;
                trackName.RegisterValueChangedCallback(a => track.name = a.newValue);
                var deleteButton = trackContents.Last().root.Q<Button>("Delete");
                deleteButton.RegisterCallback<ClickEvent>(a => {
                    animation.tracks.RemoveAt(i);
                    SyncTracks();
                });
            }
            UpdateAllTracks();
            trackRoot.MarkDirtyRepaint();
        }
        
        // 更新每个 track 的表演.
        void UpdateAllTracks()
        {
            for(int i = 0; i < trackEditors.Count; i++)
            {
                trackEditors[i].time = time;
                trackEditors[i].UpdateTrackContent(trackContents[i]);
            }
        }
        
        
        
    }   
    
    
}