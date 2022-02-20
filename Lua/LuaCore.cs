using UnityEngine;
using UnityEditor;
using XLua;
using System.Collections.Generic;
using System;
using System.IO;

namespace Prota.Lua
{
    public class LuaCore : MonoBehaviour
    {
        public const string luaSourcePath = "./LuaScripts";
        
        public static LuaCore instance = null;
        
        public class LoadedScript
        {
            public string path;
            public LuaTable scriptMeta;
        }
        
        [NonSerialized]
        public Dictionary<string, LoadedScript> loaded = new Dictionary<string, LoadedScript>();
        
        LuaEnv _env;
        
        public LuaEnv env
        {
            get
            {
                if(_env == null)
                {
                    _env = new LuaEnv();
                    Init(_env);
                }
                return _env;
            }
        }
        
        public bool envLoaded => _env != null;
        
        
        public int debugMemory;
        
        
        
        LuaCore() => instance = this;
        
        void Awake()
        {
            
        }
        
        void Update()
        {
            env.Tick();
            env.GcStep(200);
            debugMemory = env.Memroy;
        }
        
        
        
        void Init(LuaEnv l)
        {
            Debug.Log("LuaEnv 开始初始化!");
            
            env.AddLoader((ref string s) => {
                try
                {
                    var path = PathToSourcePath(s);
                    s = path;
                    var f = File.ReadAllBytes(path);
                    return f;
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                    return null;
                }
            });
            
            foreach(var directory in Directory.EnumerateDirectories(luaSourcePath, "*", SearchOption.TopDirectoryOnly))
            {
                var d = new DirectoryInfo(directory);
                if(d.Name.StartsWith(".")) continue;
                var fs = d.GetFiles("Init.lua");
                if(fs == null || fs.Length == 0)
                {
                    // Debug.LogWarning("目录 " + directory + " 不包含 Init.lua 文件");
                    continue;
                }
                
                Load(Path.Combine(Path.GetFileName(directory), "Init"));
            }
            
            // TODO: 这里收集加密/压缩过后的文件.
            
            Debug.Log("LuaEnv 初始化完毕!");
        }
        
        public void Reset()
        {
            if(_env == null) return;
            _env.Dispose();
            _env = null;
            Debug.Log("虚拟机已清理!");
        }
        
        public LoadedScript Load(string key)
        {
            key = key.Replace("\\", "/");
            var path = PathToSourcePath(key);
            if(loaded.TryGetValue(path, out var res)) return res;
            
            res = new LoadedScript();
            loaded.Add(path, res);
            
            LuaTable table = null;
            try
            {
                var ret = env.DoString($"return require '{ key }'", path);
                if(ret != null && ret.Length >= 1 && ret[0] != null) table = ret[0] as LuaTable;
            }
            catch(Exception e)
            {
                Log.Exception(e);
            }
                
            // 没有返回值, 或返回值不是 table.
            if(table == null)
            {
                // Log.Error($"脚本 { key } [{ path }] 没有返回 table.");
                return null;
            }
            
            // 即使不是"类"的脚本也会有一个 metatable, 如果没有返回值那么里面什么都没有.
            var metaTable = env.NewTable();
            metaTable.SetInPath<LuaBase>("__index", table);
            
            res.path = path;
            res.scriptMeta = metaTable;
            
            return res;
        }
        
        string GetSource(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            return null;
        }
        
        void Reload(string path)
        {
            loaded.Remove(path);
            Load(path);
        }
        
        public static bool IsValidPath(string path)
        {
            if(string.IsNullOrEmpty(path)) return false;
            var s = new FileInfo(PathToSourcePath(path));
            return s.Exists;
        }
        
        public static string PathToSourcePath(string path)
        {
            path = Path.Combine(LuaCore.luaSourcePath, path + ".lua");
            path = path.Replace("\\", "/");
            path = path.ToLower();
            return path;
        }
        
    }
    
    
}