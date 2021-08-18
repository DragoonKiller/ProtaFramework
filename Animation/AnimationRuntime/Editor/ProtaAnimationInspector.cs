using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Reflection;
using System.Linq;
using Prota.Editor;

namespace Prota.Animation
{
    /// <summary>
    /// Track 的基础类型.
    /// </summary>
    [CustomEditor(typeof(ProtaAnimation))]
    [CanEditMultipleObjects]
    public sealed class ProtaAnimationStateInspector : UnityEditor.Editor
    {
        ProtaAnimation anim => serializedObject.targetObject as ProtaAnimation;
        
        bool useOriginalView;
        
        public override void OnInspectorGUI()
        {
            useOriginalView = EditorGUILayout.ToggleLeft("使用原始视图", useOriginalView);
            if(useOriginalView) base.OnInspectorGUI();
            else CustomGUI();
        }
        
        void CustomGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space(2);
            this.SeperateLine(1, Color.black);
            EditorGUILayout.Space(3);
            
            ListTrack();
            
            AddTrack();
            
        }
        
        
        
        GUIStyle _nameStyle;
        GUIStyle nameStyle
        {
            get
            {
                if(_nameStyle == null)
                {
                    _nameStyle = new GUIStyle(EditorStyles.largeLabel);
                    _nameStyle.fontStyle = FontStyle.Bold;
                    _nameStyle.richText = true;
                }
                return _nameStyle;
            }
        }
        
        void ListTrack()
        {
            if(anim.tracks.Count == 0)
            {
                EditorGUILayout.LabelField("没有正在执行的 track.");
                return;
            }
            
            foreach(var t in anim.tracks)
            {
                this.SetColor(new Color(0.5f, 0.6f, 1f, 1f));
                EditorGUILayout.LabelField("<color=#F0D890>" + t.name + "</color> <color=#A0C0FF>(" + t.GetType().Name + ")</color>",  nameStyle);
                this.ResetColor();
                
                // 反射枚举所有元素.
                foreach(var f in t.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var originalValue = f.GetValue(t);
                    var result = this.AnyField(f.Name, t, f);
                    if(!result) EditorGUILayout.LabelField(f.Name);
                }
            }
        }
        
        
        
        
        static readonly List<Type> trackTypeCache = new List<Type>(ProtaAnimationTrack.types.Select(x => x.Value)).Sorted((a, b) => a.Name.CompareTo(b.Name));
        
        Vector2 addScrollPos = Vector2.zero;
        
        bool addTrackFold = false;
        
        void AddTrack()
        {
            addTrackFold = EditorGUILayout.Foldout(addTrackFold, "添加 track");
            if(!addTrackFold) return;
            addScrollPos = EditorGUILayout.BeginScrollView(addScrollPos, GUILayout.MaxHeight(100));
            foreach(var tt in trackTypeCache)
            {
                if(GUILayout.Button(tt.Name))
                {
                    var track = Activator.CreateInstance(tt) as ProtaAnimationTrack;
                    anim.tracks.Add(track);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}