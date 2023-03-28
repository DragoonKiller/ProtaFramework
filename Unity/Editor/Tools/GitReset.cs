using System;
using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        [MenuItem("Assets/ProtaFramework/Git/Reset file")]
        public static void GitReset()
        {
            void Backup(FileSystemInfo src)
            {
                (src.ToFileInfo().Exists || src.ToDirectoryInfo().Exists).Assert();
                var dst = Application.dataPath.AsDirectoryInfo().Parent.SubDirectory(".backup");
                dst.EnsureExists();
                var backupFile = dst.SubFile(src.Name + "." + DateTime.Now.Ticks);
                backupFile.EnsureNotExists();
                FileUtil.CopyFileOrDirectory(src.FullName.ConvertSlash(), backupFile.FullName.ConvertSlash());
            }
            
            
            var file = AssetDatabase.GetAssetPath(Selection.activeInstanceID).AsFileInfo();
            Backup(file);
            $"git checkout HEAD -- { file.FullName }".ExecuteAsCommand();
            
            file = file.AppendExtension(".meta");
            if(file.Exists || file.ToDirectoryInfo().Exists)
            {
                Backup(file);
                $"git checkout HEAD -- { file.FullName }".ExecuteAsCommand();
            }
        }
        
        
    } 
}
