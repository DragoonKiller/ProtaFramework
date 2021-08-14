using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prota.Unity;

namespace Prota.Animation
{
    public static partial class Animation2DEditor
    {        
        public static readonly HashSet<string> validExtension = new HashSet<string> {
            ".png",
            ".jpg",
            ".tga",
        };
        
        public const int defaultFrameRate = 30;
        
        
        public static class Menu
        {
            public const string oneKeySetup = "Assets/ProtaFramework/动画/一键设置动画";
            public const int oneKeySetupPriority = 1001;
            
            public const string compareTextures = "Assets/ProtaFramework/动画/比较两个贴图";
            public const int compareTexturesPriority = 1081;
            
            public const string buildTimeline = "Assets/ProtaFramework/动画/构建动画Timeline";
            public const int buildTimelinePriority = 1102;
            
            
            public const string refreshTextureSettings = "Assets/ProtaFramework/动画/刷新贴图设置";
            public const int refreshTextureSettingsPriority = 1103;
            
            public const string removeDulicatedTextures = "Assets/ProtaFramework/动画/删除重复图片";
            public const int removeDulicatedTexturesPriority = 1104;
            
            
        }
        
        
        public static List<string> GetAllFiles(string curSelectPath)
        {
            return Directory.GetFiles(curSelectPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => validExtension.Contains(Path.GetExtension(x)))
                .Select(x => Utils.AssetDatabase.FullPathToAssetPath(Path.GetFullPath(x)))
                .ToList();
        }
    }
}