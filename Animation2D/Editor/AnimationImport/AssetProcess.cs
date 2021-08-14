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
        
        [MenuItem(Menu.oneKeySetup, priority = Menu.oneKeySetupPriority)]
        static void Setup()
        {
            RemoveDuplicatedTextures();
            AnimTextureImporter.Reimport();
            BuildAnimation();
        }
        
    }
}