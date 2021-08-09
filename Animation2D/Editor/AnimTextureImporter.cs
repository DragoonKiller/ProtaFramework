using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Prota.Animation2D
{
    public class AnimTextureImporter : AssetPostprocessor
    {
        static Dictionary<string, int> needImport = new Dictionary<string, int>();
        
        void Add(string a)
        {
            if(needImport.TryGetValue(a, out var v)) needImport[a] = v + 1;
            else needImport[a] = 1;
        }
        
        void Remove(string a)
        {
            if(needImport.TryGetValue(a, out var v))
            {
                v = v - 1;
                if(v == 0) needImport.Remove(a);
                else needImport[a] = v;
            }
            else throw new InvalidOperationException();
        }
        
        bool CanImport(string a)
        {
            return needImport.TryGetValue(a, out var v) && v != 0;
        }
        
        void OnPreprocessTexture()
        {
            var importer = assetImporter as TextureImporter;
            // 第一次导入 或 记录强制导入时, 走改变选项的流程.
            if(importer.importSettingsMissing) Add(importer.assetPath);
            if(!CanImport(importer.assetPath)) return;
            importer.isReadable = true;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = true;
        }
        
        [MenuItem(Animation2DEditor.Menu.refreshTextureSettings, priority = Animation2DEditor.Menu.refreshTextureSettingsPriority)]
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