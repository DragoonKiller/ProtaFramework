using System;
using UnityEngine;
using System.IO;


namespace Prota.Unity
{
    public static partial class Utils
    {
        public static class AssetDatabase
        {
            public static string FullPathToAssetPath(string file)
            {
                var dataPath = Path.GetFullPath(Application.dataPath);
                var assetPath = "Assets/" + file.Substring(dataPath.Length);
                return assetPath;
            }
        }
    }
}
