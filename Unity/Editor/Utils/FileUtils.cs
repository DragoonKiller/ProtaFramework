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
    }
}