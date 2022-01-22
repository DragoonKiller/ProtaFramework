using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class Utils
    {
        public static List<string> GetAllFilesWithExtension(string curSelectPath, HashSet<string> validExtension)
        {
            return Directory.GetFiles(curSelectPath, "*.*", SearchOption.TopDirectoryOnly)
                .Where(x => validExtension.Contains(Path.GetExtension(x)))
                .Select(x => Utils.AssetDatabase.FullPathToAssetPath(Path.GetFullPath(x)))
                .ToList();
        }
        
        public static string GetRelativePath(string parent, string sub)
        {
            var fpParent = Path.GetFullPath(parent);
            var fpSub = Path.GetFullPath(sub);
            if(fpSub.StartsWith(fpParent)) return fpSub.Substring(fpParent.Length + 1);
            throw new System.Exception("给定路径不是包含关系.");
        }
    }
}