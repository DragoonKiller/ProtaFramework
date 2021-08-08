using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prota.Unity;

namespace Prota.Animation2D
{
    public static partial class Animation2DEditor
    {        
        public static readonly HashSet<string> validExtension = new HashSet<string> {
            ".png",
            ".jpg",
            ".tga",
        };
        
        public const int defaultFrameRate = 30;
        
        public static List<string> GetAllFiles(string curSelectPath)
        {
            return Directory.GetFiles(curSelectPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => validExtension.Contains(Path.GetExtension(x)))
                .Select(x => Utils.AssetDatabase.FullPathToAssetPath(Path.GetFullPath(x)))
                .ToList();
        }
    }
}