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
        
        [NonSerialized]
        public Dictionary<string, LuaLoadedModule> loaded = new Dictionary<string, LuaLoadedModule>();
        
        public int memory => env.Memroy;
        
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
        
        LuaCore() => instance = this;
        
        void Awake()
        {
            
        }
        
        void Update()
        {
            env.Tick();
            env.GcStep(200);
        }
        
        public LuaLoadedModule Load(string key)
        {
            if(!IsValidPath(key)) return null;
            var path = KeyToSourcePath(key);
            if(loaded.TryGetValue(path, out var res)) return res;
            loaded.Add(path, res = new LuaLoadedModule(key));
            return res;
        }
        
        void Init(LuaEnv l)
        {
            Debug.Log("LuaEnv 开始初始化!");
            
            env.AddLoader((ref string s) => {
                try
                {
                    var path = KeyToSourcePath(s);
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
            
            Debug.Log("LuaEnv 初始化完毕!");
        }
        
        public void Reset()
        {
            if(_env == null) return;
            _env.Dispose();
            _env = null;
            Debug.Log("虚拟机已清理!");
        }
        
        public static bool IsValidPath(string path)
        {
            if(string.IsNullOrEmpty(path)) return false;
            var s = new FileInfo(KeyToSourcePath(path));
            return s.Exists;
        }
        
        public static string KeyToSourcePath(string path)
        {
            path = Path.Combine(LuaCore.luaSourcePath, path + ".lua")
                .Replace("\\", "/")
                .ToLower();
            return path;
        }
        
    }
    
    
}