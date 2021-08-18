using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Prota.Unity;
using UnityEngine.Timeline;

namespace Prota.Animation
{
    
    // 要求格式: 文件名的最后由 "_xxxx" 结尾, xxxx 是数字.
    public static partial class Animation2DEditor
    {
        public static HashSet<string> validExtensions = new HashSet<string> {
            ".png",
            ".jpg",
            ".tiff",
        };
        
        [MenuItem("Assets/ProtaFramework/动画/构建Sprite集合", priority = 1103)] 
        static void BuildSpriteCollection()
        {
            var curSelectPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            var folderName = Path.GetFileNameWithoutExtension(curSelectPath);
            var parentName = new DirectoryInfo(curSelectPath).Parent.Name;
            var files = Utils.GetAllFilesWithExtension(curSelectPath, validExtensions);
            
            // 创建一个 SpriteCollection.
            var data = ScriptableObject.CreateInstance<ProtaSpriteCollection>();
            var assetPath = Path.Combine(curSelectPath, Path.GetFileNameWithoutExtension(curSelectPath) + ".asset");
            Undo.RecordObject(data, "Sprite资源导入");
            Debug.Log("资源路径: " + assetPath);
            
            foreach(var file in files)
            {
                var texturePath = file;
                var name = Path.GetFileNameWithoutExtension(texturePath);
                Debug.Log("Texture: " + texturePath);
                
                var fileName = Path.GetFileNameWithoutExtension(file);
                var sections = fileName.Split('_');
                if(sections.Length == 1)
                {
                    Debug.LogError("文件命名错误: " + file);
                    continue;
                }
                var number = int.Parse(sections[sections.Length - 1]);
                var allSprites = AssetDatabase.LoadAllAssetsAtPath(texturePath)
                    .Select(x => x as Sprite)
                    .Where(x => x)
                    .ToList();
                var sprite = allSprites
                    .Where(x => x.name.Contains(name))
                    .FirstOrDefault();
                if(sprite == null) Debug.LogError("没有匹配名称 " + name + " 的 sprite.");
                else Debug.Log("Sprite: " + sprite.name);
                data.Add(number.ToString(), sprite);
            }
            
            data.name = parentName + "_" + folderName;
            EditorUtility.SetDirty(data);
            AssetDatabase.CreateAsset(data, assetPath);
        }
        
        
    }
}