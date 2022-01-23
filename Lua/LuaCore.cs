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
                    var f = File.ReadAllBytes(Path.Combine(luaSourcePath, s + ".lua"));
                    return f;
                }
                catch(Exception e)
                {
                    Debug.LogError(e);
                    return new byte[0];
                }
            });
            
            foreach(var directory in Directory.EnumerateDirectories(luaSourcePath, "*", SearchOption.TopDirectoryOnly))
            {
                var d = new DirectoryInfo(directory);
                var fs = d.GetFiles("Init.lua");
                if(fs == null || fs.Length == 0)
                {
                    Debug.LogWarning("目录 " + directory + " 不包含 Init.lua 文件");
                    continue;
                }
                
                Load(Path.Combine(directory, "Init.lua"));
            }
            
            // TODO: 这里收集加密/压缩过后的文件.
            
            Debug.Log("LuaEnv 创建完毕!");
        }
        
        public void Reset()
        {
            if(_env == null) return;
            _env.Dispose();
            _env = null;
            Debug.Log("虚拟机已清理!");
        }
        
        
        public LuaTable GetInstanceOfScript(string path)
        {
            var t = env.NewTable();
            var loaded = Load(Path.Combine(LuaCore.luaSourcePath, path + ".lua"));
            t.SetMetaTable(loaded.scriptMeta);
            return t;
        }
        
        LoadedScript Load(string path)
        {
            if(loaded.TryGetValue(path, out var res)) return res;
            
            // 脚本在一般情况下返回一个 table 表示这个类.
            // 脚本在非一般情况下返回 nil.
            // 否则都不合法.
            var source = GetSource(path);
            var ret = env.DoString(source, path);
            LuaTable table = null;
            if(ret != null && ret.Length >= 1 && ret[0] != null) table = (LuaTable)ret[0];
            
            // 即使不是"类"的脚本也会有一个 metatable, 如果没有返回值那么里面什么都没有.
            var metaTable = env.NewTable();
            metaTable.SetInPath<LuaBase>("__index", table);
            
            res = new LoadedScript();
            res.scriptMeta = metaTable;
            return res;
        }
        
        string GetSource(string path)
        {
            return File.ReadAllText(path);
        }
        
        void Reload(string path)
        {
            loaded.Remove(path);
            Load(path);
        }
        
        public static bool IsValidPath(string path)
        {
            if(string.IsNullOrEmpty(path)) return false;
            var s = new FileInfo(path);
            return s.Exists;
        }
    }
    
    
}