using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Prota.Unity;
using System.Linq;

namespace Prota.Lua
{
    
    [CreateAssetMenu(fileName = "LuaScriptList", menuName = "ProtaFramework/Lua/创建Lua脚本列表", order = 1)]
    public class LuaScriptList : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string name;
            public string path;
        }
        
        string scriptRootPath => new FileInfo(AssetDatabase.GetAssetPath(this)).Directory.FullName;
        
        public List<Entry> entries = new List<Entry>();
        
        [NonSerialized]
        public Dictionary<string, Entry> accessCache = new Dictionary<string, Entry>();
        
        public void Add(string path)
        {
            path = path.Replace("\\", "/");
            var name = GetNameFromPath(path);
            if(name == null) return;
            var entry = entries.FindIndex(x => x.name == name);
            if(entry >= 0) return;
            entries.Add(new Entry{ name = name, path = path });
            EditorUtility.SetDirty(this);
        }
        
        public void Remove(string path)
        {
            var name = GetNameFromPath(path);
            if(name == null) return;
            entries.RemoveAll(x => x.name == name);
            if(accessCache.ContainsKey(name)) accessCache.Remove(name);
            EditorUtility.SetDirty(this);
        }
        
        public void Clear()
        {
            entries.Clear();
            accessCache.Clear();
            EditorUtility.SetDirty(this);
        }
        
        public string Get(string name)
        {
            name = name.ToLower();
            if(accessCache.TryGetValue(name, out var res)) return res.path;
            
            var index = entries.FindIndex(x => x.name == name);
            if(index < 0)
            {
                Debug.LogError("找不到路径对应的 lua 文件: " + name);
                return null;
            }
            accessCache[name] = entries[index];
            
            return entries[index].path;
        }
        
        string GetNameFromPath(string path)
        {
            var myPath = Path.GetFullPath(scriptRootPath).ToLower();
            var p = Path.GetFullPath(path).ToLower();
            // Debug.Log(myPath + " " + p);
            p = Utils.GetRelativePath(myPath, p);
            if(p == null) return null;
            // Debug.Log(p);
            if(p.EndsWith(".lua")) p = p.Substring(0, p.Length - ".lua".Length);
            if(p.EndsWith(".lua.txt")) p = p.Substring(0, p.Length - ".lua.txt".Length);
            p = p.Replace("\\", "/");
            return p;
        }
        
        public void RefreshAll()
        {
            this.Clear();
            foreach(var s in Directory.EnumerateFiles(scriptRootPath, "*.lua", SearchOption.AllDirectories))
            {
                this.Add(s);
            }
            EditorUtility.SetDirty(this);
        }
    }
    
}