using System;
using UnityEngine;
using System.IO;


namespace Prota.Editor
{
    public static partial class UnityMethodExtensions
    {
        public static string FullPathToAssetPath(this string file)
        {
            var dataPath = Path.GetFullPath(Application.dataPath);
            var assetPath = "Assets/" + file.Substring(dataPath.Length);
            return assetPath;
        }
    }
}
