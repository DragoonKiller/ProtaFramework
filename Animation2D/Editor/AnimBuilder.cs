using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Prota.Unity;
using UnityEngine.Timeline;

namespace Prota.Animation2D
{
    
    // 要求格式: 文件名的最后由 "_xxxx" 结尾, xxxx 是数字.
    public static partial class Animation2DEditor
    {

        
        const string genTrackName = "GeneratedTrack.Sprite";
        
        [MenuItem(Menu.buildTimeline, priority = Menu.buildTimelinePriority)]
        static void BuildAnimation()
        {
            var curSelectPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            var files = GetAllFiles(curSelectPath);
            
            var id2File = new Dictionary<int, string>();
            foreach(var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var sections = fileName.Split('_');
                if(sections.Length == 1)
                {
                    Debug.LogError("文件命名错误: " + file);
                    continue;
                }
                var number = int.Parse(sections[sections.Length - 1]);
                id2File.Add(number, file);
            }
            
            var timleinePath = Path.Combine(curSelectPath, Path.GetFileName(curSelectPath) + ".Timeline.playable");
            var timeline = AssetDatabase.LoadAssetAtPath<TimelineAsset>(timleinePath);
            if(timeline == null)
            {
                timeline = ScriptableObject.CreateInstance<TimelineAsset>();
                AssetDatabase.CreateAsset(timeline, timleinePath);
                Debug.Log("在 " + timleinePath + " 创建 timeline.");
            }
            else
            {
                Debug.Log("使用原有 timeline: " + timleinePath);
                for(int i = 0; i < timeline.rootTrackCount; i++)
                {
                    var track = timeline.GetRootTrack(i);
                    if(track.name == "GeneratedTrack.Sprite")
                    {
                        timeline.DeleteTrack(track);
                        break;
                    }
                }
            }
            
            var newTrack = timeline.CreateTrack<AnimationTrack>(genTrackName);
            var clip = new AnimationClip();
            newTrack.CreateClip(clip);
            
            AssetDatabase.SaveAssets();
        }
        
        [MenuItem("Assets/ProtaFramework/BuildAnimationTrack", true, priority = 1)]
        static bool BuildAnimationTrackValid()
        {
            var curSelectPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            return Directory.Exists(curSelectPath);
        }
        
        
        
    }
}