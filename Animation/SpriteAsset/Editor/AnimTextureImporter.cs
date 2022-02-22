using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Prota.Animation
{
    public class AnimTextureImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            var importer = assetImporter as TextureImporter;
            
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = true;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            
            importer.spritePixelsPerUnit = 1;
            if(importer.assetPath.Contains("_Anchor_") || importer.assetPath.Contains("_anchor_"))
            {
                importer.isReadable = true;
            }
        }
        
        [MenuItem("Assets/ProtaFramework/动画/刷新贴图设置", priority = 1120)]
        public static void Reimport()
        {
            foreach(var g in Selection.objects)
            {
                if(!(g is Texture2D)) continue;
                var assetPath = AssetDatabase.GetAssetPath(g);
                if(assetPath == null || assetPath == "") continue;
                var options = ImportAssetOptions.DontDownloadFromCacheServer | ImportAssetOptions.ForceUncompressedImport | ImportAssetOptions.ForceUpdate;
                AssetDatabase.ImportAsset(assetPath, options);
            }
            
            AssetDatabase.Refresh();
        }
        
        
    }
    
    
}