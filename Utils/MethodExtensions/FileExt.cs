using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static string ToStandardPath(this string file)
        {
            return file.Replace("\\", "/").Trim('/');
        }
        
        public static string GetRelativePath(string parent, string sub)
        {
            var fpParent = Path.GetFullPath(parent);
            var fpSub = Path.GetFullPath(sub);
            if(fpSub.StartsWith(fpParent)) return fpSub.Substring(fpParent.Length + 1);
            // throw new System.Exception("给定路径不是包含关系.");
            return null;
        }
        
        // standardFormat == true: /.
        // standardFormat == false: \.
        public static string ConvertSlash(this string name, bool standardFormat = true)
            => standardFormat ? name.Replace("\\", "/") : name.Replace("/", "\\");
        
        public static string PathCombine(this string s, string x)
            => Path.Combine(s, x);
            
        public static string PathCombine(this string s, params string[] x)
        {
            var a = new string[x.Length + 1];
            a[0] = s;
            x.CopyTo(a, 1);
            return Path.Combine(a);
        }
        
        public static string SubPath(this string fullPath, string subName)
            => Path.Combine(fullPath, subName);
            
        public static string RelativePath(this DirectoryInfo d, FileSystemInfo f)
        {
            var dpath = d.FullName;
            var fpath = f.FullName;
            if(!fpath.StartsWith(dpath)) throw new Exception($"file { fpath } not in directory { dpath }");
            var pp = fpath.Substring(dpath.Length);
            if(pp[0] == '\\' || pp[0] == '/') pp = pp.Substring(1);
            return pp.ToStandardPath();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static bool IsDirectory(this FileSystemInfo d)
            => d is DirectoryInfo;
        
        public static bool IsFile(this FileSystemInfo d)
            => d is FileInfo;
        
        public static FileSystemInfo Sub(this FileSystemInfo d, string subName)
            => d.FullName.SubPath(subName).AsFileSystemInfo();
        
        public static DirectoryInfo SubDirectory(this FileSystemInfo d, string subName)
            => new DirectoryInfo(d.FullName.SubPath(subName));
        
        public static FileInfo SubFile(this FileSystemInfo d, string subName)
            => new FileInfo(d.FullName.SubPath(subName));
        
        public static DirectoryInfo Directory(this FileSystemInfo d)
        {
            if(d is FileInfo f) return f.Directory;
            if(d is DirectoryInfo dd) return dd.Parent;
            throw new ArgumentException(d.FullName);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static DirectoryInfo AsDirectoryInfo(this string path)
            => new DirectoryInfo(path);
        
        public static FileInfo AsFileInfo(this string path)
            => new FileInfo(path);
        
        public static FileSystemInfo AsFileSystemInfo(this string path)
            => (File.GetAttributes(path) & FileAttributes.Directory) != 0
                ? new DirectoryInfo(path)
                : new FileInfo(path);
        
        public static FileInfo ToFileInfo(this FileSystemInfo f)
            => f is FileInfo ff ? ff : new FileInfo(f.FullName);
            
        public static DirectoryInfo ToDirectoryInfo(this FileSystemInfo f)
            => f is DirectoryInfo ff ? ff : new DirectoryInfo(f.FullName);
        
        // ====================================================================================================
        // ====================================================================================================
        
            
        public static DirectoryInfo EnsureExists(this DirectoryInfo d)
        {
            if(d.Exists) return d;
            d.Parent.EnsureExists();
            d.Create();
            return d;
        }
        
        public static T EnsureNotExists<T>(this T f) where T: FileSystemInfo
        {
            if(f.Exists) f.Delete();
            f.Refresh();
            return f;
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static DirectoryInfo WithName(this DirectoryInfo f, Func<string, string> newName)
            => f.Parent.SubDirectory(newName(f.FullName));
        
        public static DirectoryInfo WithName(this DirectoryInfo f, string newName)
            => f.Parent.SubDirectory(newName);
        
        public static FileInfo WithName(this FileInfo f, Func<string, string> newName)
            => f.Directory.SubFile(newName(f.FullName));
        
        public static FileInfo WithName(this FileInfo f, string newName)
            => f.Directory.SubFile(newName);
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public static FileInfo ReplaceExtension(this FileInfo f, string newExtension)
            => f.Directory.SubFile(Path.GetFileNameWithoutExtension(f.Name) + newExtension);
        
        public static FileInfo AppendExtension(this FileInfo f, string appendExtension)
            => f.Directory.SubFile(f.Name + appendExtension);
            
        public static DirectoryInfo ReplaceExtension(this DirectoryInfo f, string newExtension)
            => f.Parent.SubDirectory(Path.GetFileNameWithoutExtension(f.Name) + newExtension);
        
        public static DirectoryInfo AppendExtension(this DirectoryInfo f, string appendExtension)
            => f.Parent.SubDirectory(f.Name + appendExtension);
            
            
        // ====================================================================================================
        // ====================================================================================================
        
        public static bool Same(this FileSystemInfo a, FileSystemInfo b, bool caseSensitive = false)
        {
            if(a.GetType() != b.GetType()) return false;
            if(caseSensitive) return a.FullName == b.FullName;
            return a.FullName.Equals(b.FullName, StringComparison.OrdinalIgnoreCase);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static bool IsInDirectory(this FileSystemInfo f, DirectoryInfo d)
            => f.FullName.StartsWith(d.FullName);
        
        public static bool IsInDirectroyDirectly(this FileSystemInfo f, DirectoryInfo d)
            => f.Directory().FullName.Length == d.FullName.Length;
        
        
        
        
    }
    
}
