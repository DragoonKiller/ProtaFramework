
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prota
{
    
    public class FileChangeEvent
    {
        public enum OpType
        {
            Deleted = 1,
            Created,
            Changed,
            // Renamed, no rename, just create + delete.
        }
        
        public FileTree.Node node;
        public OpType opType;
        // public string oldPath = null;
        
        public bool isDirectory => node.isDirectory;
        public bool isFile => node.isFile;
        public string path => node.fullPath;
        // extension is with dot.
        public string extension => Path.GetExtension(path);
        public string nameNoExt => Path.GetFileNameWithoutExtension(path);
        public string name => Path.GetFileName(path);
        
        public bool isDelete => opType == OpType.Deleted;
        public bool isCreated => opType == OpType.Created;
        public bool isChanged => opType == OpType.Changed;
    }
        
    
    // A tree for fast accessing directory structure and files, without accessing the file system.
    public sealed class FileTree
    {
        public sealed class Node
        {
            public Node parent { get; private set; }
            public bool isFile { get; private set; }
            public bool valid { get; private set; }
            public string fullPath { get; private set; }
            List<Node> _files = new List<Node>();
            List<Node> _dirs = new List<Node>();
            public DateTime date { get; private set; }
            public event Action<Node, FileChangeEvent> onUpdate;
            
            public FileInfo fileInfo => new FileInfo(fullPath);
            public DirectoryInfo dirInfo => new DirectoryInfo(fullPath);
            public FileSystemInfo info => isFile ? (FileSystemInfo) fileInfo : dirInfo;
            
            public string name => Path.GetFileName(fullPath);
            public string nameNoExt => Path.GetFileNameWithoutExtension(fullPath);
            public string extension => Path.GetExtension(fullPath);
            public IReadOnlyList<Node> files => _files;
            public IReadOnlyList<Node> dirs => _dirs;
            public bool isDirectory => !isFile;
            
            public bool HasFile(string path, out Node node)
            {
                node = Find(path);
                return node != null && node.isFile;
            }
            
            public bool HasFile(string path) => HasFile(path, out _);
            
            public bool HasDir(string path, out Node node)
            {
                node = Find(path);
                return node != null && node.isDirectory;
            }
            
            public bool HasDir(string path) => HasDir(path, out _);
            
            // 找到 path 对应的节点. path 是相对于自身的路径.
            // 如果 allowPartialPath 为 true, 则尽可能深地匹配 path.
            // 注意只有处于 path 末尾的节点才*有可能*是文件.
            public Node Find(string relativePath, bool allowPartialPath = false)
            {
                relativePath = relativePath.ToStandardPath();
                var parts = relativePath.Split('/');
                return InternalFind(parts, 0, allowPartialPath);
            }
            
            // 会将 path 转换为 FullPath 再做查询.
            public Node FindFullPath(string path, bool allowPartialPath = false)
            {
                path = Path.GetFullPath(path).ToStandardPath();
                if(!path.StartsWith(this.fullPath)) return null;
                var node = this;
                var parts = path.Substring(this.fullPath.Length + 1).Split('/');
                return InternalFind(parts, 0, allowPartialPath);
            }
            // ====================================================================================================
            // ====================================================================================================
            
            
            Node(string fullPath, Node parent = null)
            {
                this.parent = parent;
                this.fullPath = Path.GetFullPath(fullPath).ToStandardPath();
                isFile = File.Exists(this.fullPath);
                this.date = isFile ? File.GetLastWriteTime(this.fullPath) : Directory.GetLastWriteTime(this.fullPath);
                if(isDirectory) Task.Run(BuildSubContents).Wait();
                valid = true;
            }
            
            // create a new tree / new sub-tree.
            // return: original tree.
            public Node RebuildSubContents()
            {
                if(parent != null) throw new Exception($"Node [{fullPath}] needs to be a root node to rebuild sub-contents.");
                if(!isDirectory) throw new Exception($"Node [{fullPath}] needs to be a directory to rebuild sub-contents.");
                var original = this.CloneTree();
                original.SetTreeInvalid();
                Task.Run(this.BuildSubContents).Wait();
                return original;
            }
            
            async Task BuildSubContents()
            {
                var tdir = Task.Run(() => {
                    _dirs.Clear();
                    var directories = Directory.GetDirectories(this.fullPath);
                    foreach(var d in directories) _dirs.Add(new Node(d, this));
                });
                
                var tfile = Task.Run(() => {
                    _files.Clear();
                    var files = Directory.GetFiles(this.fullPath);
                    foreach(var f in files) _files.Add(new Node(f, this));
                });
                
                await tdir;
                await tfile;
            }
            
            public Node Clone()
            {
                var g = MemberwiseClone() as Node;
                // shallow clone may copy pointers to the same collection.
                // notice that trees are remain unchanged with parent-children structure.
                g._dirs = new List<Node>(_dirs);
                g._files = new List<Node>(_files);
                return g;
            }
            
            public Node CloneTree()
            {
                var clone = Clone();
                clone._dirs = new List<Node>(_dirs.Select(x => x.CloneTree()));
                clone._files = new List<Node>(_files.Select(x => x.CloneTree()));
                foreach(var d in clone._dirs) d.parent = clone;
                foreach(var f in clone._files) f.parent = clone;
                return clone;
            }
            
            
            Node InternalFind(string[] path, int i, bool allowPartialPath)
            {
                string ind = new string(' ', i * 4);
                
                if(i == path.Length) return this;
                
                // 找到这个名称对应的子文件夹.
                var child = _dirs.FirstOrDefault(d => d.name == path[i]);
                if(child != null)
                {
                    var c = child.InternalFind(path, i + 1, allowPartialPath);
                    return c;
                }
                
                // 没有子文件夹, 两种情况.
                // (1) 有对应的文件.
                var file = _files.FirstOrDefault(f => f.name == path[i]);
                if(file != null)
                {
                    // 当 allowPartialPath == false 时, 文件必须出现在最后一个找到的内容中.
                    if(i == path.Length - 1)
                    {
                        return file;
                    }
                    
                    // 当 allowPartialPath == true 时, 路径中的任何一个文件均合法.
                    // 由于文件不会有子文件, 接下来的所有路径都是不合法的.
                    if(allowPartialPath)
                    {
                        return file;
                    }
                    
                    return null;
                }
                
                // (2) 没有这个文件夹.
                if(allowPartialPath)
                {
                    return this;
                }
                
                return null;
            }
        
            public static Node BuildTreeRoot(string fullPath)
            {
                return new Node(fullPath);
            }
            
            public void NotifyEvent(FileChangeEvent e)
            {
                onUpdate?.Invoke(this, e);
                this.parent?.NotifyEvent(e);
            }
            
            public List<FileChangeEvent> Diff(Node otherRoot)
            {
                var res = new List<FileChangeEvent>();
                DiffInternal(this, otherRoot, res);
                return res;
            }
            
            // change from a to b.
            static void DiffInternal(Node a, Node b, List<FileChangeEvent> res)
            {
                if(a == null && b == null) return;
                
                // created.
                if(a == null && b != null)
                {
                    var e = new FileChangeEvent();
                    e.node = b;
                    e.opType = FileChangeEvent.OpType.Created;
                    res.Add(e);
                    return;
                }
                
                // removed.
                if(a != null && b == null)
                {
                    var e = new FileChangeEvent();
                    e.node = a;
                    e.opType = FileChangeEvent.OpType.Deleted;
                    res.Add(e);
                    return;
                }
                
                // file name not changed, but maybe modified.
                if(a.date != b.date)
                {
                    var e = new FileChangeEvent();
                    e.node = b;
                    e.opType = FileChangeEvent.OpType.Changed;
                    res.Add(e);
                }
                
                // actually not modified.
                // if is directory, go ahead.
                if(b.isDirectory)
                {
                    using var _ = TempHashSet.Get<string>(out var names);
                    
                    foreach(var f in a.files) names.Add(f.name);
                    foreach(var f in b.files) names.Add(f.name);
                    foreach(var f in a.dirs) names.Add(f.name);
                    foreach(var f in b.dirs) names.Add(f.name);
                    
                    foreach(var name in names)
                    {
                        var aa = a.Find(name);
                        var bb = b.Find(name);
                        DiffInternal(aa, bb, res);
                    }
                }
            }
            
            void SetTreeInvalid()
            {
                this.valid = false;
                foreach(var d in _dirs) d.SetTreeInvalid();
                foreach(var f in _files) f.SetTreeInvalid();
            }
        }
        
        public readonly Node root;
        FileSystemWatcher watcher;
        SynchronizationContext syncContext;
        
        object lockObj = new object();
        object tagObj = null;
        public event Action afterRefresh;
        public event Action<List<FileChangeEvent>> afterRefreshDetailed;
        
        public FileTree(string fullPath, SynchronizationContext cc = null)
        {
            fullPath = Path.GetFullPath(fullPath).ToStandardPath();
            if(!Directory.Exists(fullPath)) throw new Exception($"Directory not exists: {fullPath}");
            
            syncContext = cc;
            root = Node.BuildTreeRoot(fullPath);
            
            watcher = new FileSystemWatcher(fullPath);
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;
            watcher.InternalBufferSize = 128 * 1024;
        }
        
        async Task AppendRefreshRequest()
        {
            var myTag = new object();
            lock(lockObj)
            {
                if(tagObj != null) tagObj = null;
                tagObj = myTag;
            }
            
            await new SystemTimer(0.1);
            if(syncContext != null) await syncContext;
            
            lock(lockObj)
            {
                if(tagObj == myTag)
                {
                    var ori = root.RebuildSubContents();
                    tagObj = null;
                    var diff = ori.Diff(root);
                    foreach(var e in diff) e.node.NotifyEvent(e);
                    afterRefresh?.Invoke();
                    afterRefreshDetailed?.Invoke(diff);
                }
            }
        }
        
        async void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                await AppendRefreshRequest();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString() + "\n" + ex.StackTrace);
            }
        }

        async void OnDeleted(object sender, FileSystemEventArgs e)
        {
            try
            {
                await AppendRefreshRequest();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString() + "\n" + ex.StackTrace);
            }
        }

        async void OnRenamed(object sender, RenamedEventArgs e)
        {
            try
            {
                await AppendRefreshRequest();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString() + "\n" + ex.StackTrace);
            }
        }
        
        async void OnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                await AppendRefreshRequest();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString() + "\n" + ex.StackTrace);
            }
        }
    }
}
