using System;
using UnityEngine;
using System.IO;
using MessagePack.Unity.Extension;
using UnityEditor;

namespace Prota.Editor
{
    public static partial class UnityMethodExtensions
    {
        public static string FullPathToAssetPath(this string file)
        {
            var dataPath = Path.GetFullPath(Application.dataPath);
            // Debug.LogError(dataPath);
            var assetPath = "Assets/" + file.Substring(dataPath.Length + 1);
            // Debug.LogError(file);
            return assetPath.ToStandardPath();
        }
    }
}
