using System;
using UnityEngine;
using System.IO;
using UnityEditor;
using Prota.Unity;

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
        
        public static string FormatAssetPath(this Sprite s)
        {
            var texture = s.texture;
            var path = AssetDatabase.GetAssetPath(texture);
            var name = s.name;
            return $"{path}::{name}";
        }
        
        public static Sprite GetSpriteFromPath(this string s)
        {
            var split = s.Split("::", StringSplitOptions.None);
            var texturePath = split[0];
            var spriteName = split[1];
            var allAssets = AssetDatabase.LoadAllAssetsAtPath(texturePath);
            var sprite = Array.Find(allAssets, x => x.name == spriteName) as Sprite;
            return sprite;
        }
        
        public static string FormatGameObjectPath(this GameObject g)
        {
            return g.transform.GetNamePath();
        }
        
        public static GameObject GetGameObjectFromPath(this string path)
        {
            var g = GameObject.Find(path);
            if (g != null) return g;
            Debug.LogWarning($"GameObject not found at path [{path}]");
            return null;
        }
    }
}
