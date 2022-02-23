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
            
            importer.spritePixelsPerUnit = 128;
            if(importer.assetPath.Contains("_Anchor_") || importer.assetPath.Contains("_anchor_"))
            {
                importer.isReadable = true;
            }
        }
        
        
        
    }
    
    
}