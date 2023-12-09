using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

using Prota.Unity;
using Prota.Editor;
using UnityEditor.UIElements;
using System.Linq;
using NUnit.Framework;

namespace Prota.Editor
{
    [CustomEditor(typeof(Save), false)]
    public class SaveInspector : UnityEditor.Editor
    {
        static string filter;
        
        public override void OnInspectorGUI()
        {
            
            if(GUILayout.Button("open settings file"))
            {
                EditorUtility.OpenWithDefaultApp(Save.saveFile.FullName);
            }
            
            if(GUILayout.Button("open settings folder"))
            {
                EditorUtility.OpenWithDefaultApp(Save.dir.FullName);
            }
            
            var s = this.target as Save;
            if(!s.dataLoaded) s.LoadData();
            
            var target = this.target.ProtaReflection();
            
            base.OnInspectorGUI();
            
            GUI.enabled = false;
            EditorGUILayout.Toggle("pending", target.Get<bool>("pending"));
            EditorGUILayout.Toggle("saving", target.Get<bool>("saving"));
            GUI.enabled = true;
            
            var data = target.Get<Dictionary<string, string>>("data");
            data.AssertNotNull();
            
            EditorGUILayout.LabelField("total count", data.Count.ToString());
            
            filter = EditorGUILayout.TextField("name", filter);
            bool filterIsNone = string.IsNullOrWhiteSpace(filter);
            
            EditorGUILayout.LabelField("filtered", data.Count.ToString());
            
            using var _ = TempList.Get<(string, string)>(out var entries);
            foreach(var d in data)
            {
                if(!filterIsNone && d.Key.Contains(filter.ToLower()))
                {
                    entries.Add((d.Key, d.Value));
                }
                if(entries.Count > 30) return;
            }
            
            EditorGUILayout.LabelField("==================");
            if(entries.Count > 30)
            {
                EditorGUILayout.HelpBox("too many entries", MessageType.Warning);
            }
            else if(filterIsNone)
            {
                EditorGUILayout.HelpBox("use \"name\" to find propery", MessageType.Warning);
            }
            else if(entries.Count == 0)
            {
                EditorGUILayout.HelpBox("no entry found", MessageType.Warning);
            }
            else
            {
                foreach(var e in entries)
                {
                    var oriValue = e.Item2;
                    var value = EditorGUILayout.TextField(e.Item1, oriValue);
                    if(value != oriValue)
                    {
                        s.Write(e.Item1, value);
                    }
                }
            }
            EditorGUILayout.LabelField("==================");
            
            EditorGUILayout.BeginHorizontal();
            if(filterIsNone) GUI.enabled = false;
            if(GUILayout.Button("Add")) s.Write(filter, "");
            else if(GUILayout.Button("Remove")) s.Erase(filter);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            
            s.Update();
        }
    }
}
