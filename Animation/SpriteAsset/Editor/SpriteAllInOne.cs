using UnityEngine;
using UnityEditor;
using Prota.Editor;
using System.Linq;
using System.Collections.Generic;
using Prota.Unity;

namespace Prota.Animation
{
    // 删除导出的动画序列中的重复图片.
    public static partial class Animation2DEditor
    {
        [MenuItem("Assets/ProtaFramework/动画/刷新资源", priority = 1000)]
        static void ProcessSprites()
        {
            RemoveDuplicatedTextures();
        }
        
        
        
    }
}