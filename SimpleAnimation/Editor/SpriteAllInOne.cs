using UnityEngine;
using UnityEditor;
using Prota.Editor;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Prota.Animation
{
    // 删除导出的动画序列中的重复图片.
    public static partial class Animation2DEditor
    {
        [MenuItem("Assets/ProtaFramework/动画/刷新资源 %6", priority = 1000)]
        static void ProcessSprites()
        {
            RemoveDuplicatedTextures();
            ScanAnimationInFolder();
        }

        static HashSet<string> validExtensions = new HashSet<string>() {
            ".jpg",
            ".png",
            ".tiff",
        };
        
        static string selectedFolder
        {
            get
            {
                
                var curSelectPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
                if(!AssetDatabase.IsValidFolder(curSelectPath))
                {
                    Debug.LogError("请选择一个文件夹. 所选路径不是文件夹: " + curSelectPath);
                    return null;
                }
                return curSelectPath;
            }
        }
        
        [MenuItem("Assets/ProtaFramework/动画/删除重复图片", priority = 1111)]
        static void RemoveDuplicatedTextures()
        {
            var curSelectPath = selectedFolder;
            if(curSelectPath == null) return;
            var files = Prota.Editor.Utils.GetAllFilesWithExtension(curSelectPath, validExtensions);
            var textures = files.Select(x => AssetDatabase.LoadAssetAtPath<Texture2D>(x)).ToList();
            var sameAsPrev = new List<bool>();
            for(int i = 0; i < textures.Count; i++) sameAsPrev.Add(false);
            for(int i = textures.Count - 1; i > 0; i--)
            {
                var pre = textures[i - 1];
                var cur = textures[i];
                if(pre.Same(cur)) sameAsPrev[i] = true;
            }
            
            for(int i = 1; i < textures.Count; i++) if(sameAsPrev[i])
            {
                AssetDatabase.DeleteAsset(files[i]);
            }
            
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Assets/ProtaFramework/动画/构建动画资源", priority = 1112)]
        static void ScanAnimationInFolder()
        {
            RemoveDuplicatedTextures();
            AssetDatabase.Refresh();
            
            
            
            var curSelectPath = selectedFolder;
            if(curSelectPath == null) return;
            var anims = new Dictionary<string, (List<string> anims, List<string> anchors)>();
            var dirInfo = new DirectoryInfo(curSelectPath);
            foreach(var file in dirInfo.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly))
            {
                if(!validExtensions.Contains(file.Extension)) continue;
                var s = file.Name.Split("_");
                var animName = string.Join("_", s.SkipLast(1));
                var isAnchor = false;
                if(animName.EndsWith("_Anchor"))
                {
                    animName = animName.Substring(0, animName.Length - "_Anchor".Length);
                    isAnchor = true;
                }
                // Log.Info(animName + " : " + file.Name + " : " + isAnchor);
                if(!anims.TryGetValue(animName, out var lists)) anims[animName] = lists = (new List<string>(), new List<string>());
                if(!isAnchor) lists.anims.Add(file.FullName.FullPathToAssetPath());
                else lists.anchors.Add(file.FullName.FullPathToAssetPath());
            }
            
            foreach(var (name, lists) in anims)
            {
                var assetPath = Path.Combine(curSelectPath, name + ".asset");
                var target = AssetDatabase.LoadAssetAtPath<SimpleAnimationAsset>(assetPath);
                
                var exists = true;
                if(target == null)
                {
                    exists = false;
                    target = ScriptableObject.CreateInstance<SimpleAnimationAsset>();
                }
                target.Clear();
                target.name = name;
                foreach(var spriteTexPath in lists.anims)
                {
                    var resources = AssetDatabase.LoadAllAssetsAtPath(spriteTexPath);
                    var sprite = resources.Where(x => x is Sprite).First() as Sprite;
                    target.sprites.Add(sprite);
                }
                foreach(var anchorTexPath in lists.anchors)
                {
                    var resources = AssetDatabase.LoadAllAssetsAtPath(anchorTexPath);
                    var sprite = resources.Where(x => x is Sprite).First() as Sprite;
                    target.anchorResources.Add(sprite);
                }
                
                target.Process();
                
                if(!exists) AssetDatabase.CreateAsset(target, assetPath);
            }
        }

        [MenuItem("Assets/ProtaFramework/动画/刷新贴图设置", priority = 1113)]
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