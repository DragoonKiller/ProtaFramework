using UnityEngine;
using UnityEditor;
using Prota.Editor;
using System.Linq;
using System.Collections.Generic;

namespace Prota.Animation2D
{
    // 删除导出的动画序列中的重复图片.
    public static partial class Animation2DEditor
    {
        [MenuItem(Menu.removeDulicatedTextures, priority = Menu.removeDulicatedTexturesPriority)]
        static void RemoveDuplicatedTextures()
        {
            var curSelectPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if(!AssetDatabase.IsValidFolder(curSelectPath))
            {
                Debug.LogError("请选择一个文件夹. 所选路径不是文件夹: " + curSelectPath);
                return;
            }
            var files = GetAllFiles(curSelectPath);
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
    }
}