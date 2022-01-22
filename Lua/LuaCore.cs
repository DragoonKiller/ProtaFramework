using UnityEngine;
using UnityEditor;
using XLua;
using System.Collections.Generic;
using System;

namespace Prota.Lua
{
    public class LuaCore : MonoBehaviour
    {
        public static LuaCore instance = null;
        
        public List<TextAsset> loadOnInit = new List<TextAsset>();
        
        
        public class LoadedScript
        {
            public TextAsset asset;
            public LuaTable scriptMeta;
        }
        
        public Dictionary<TextAsset, LoadedScript> loaded = new Dictionary<TextAsset, LoadedScript>();
        
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
            // 使用 lua 脚本调用 DontDestroyOnLoad.
            // GameObject.DontDestroyOnLoad(this.gameObject);
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
            
            foreach(var i in loadOnInit)
            {
                Debug.Log("LuaEnv 初始化加载: " + i.name);
                try
                {
                    l.DoString(i.text);
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                }
            }
            
            Debug.Log("LuaEnv 创建完毕!");
        }
        
        public void Reset()
        {
            if(_env == null) return;
            _env.Dispose();
            _env = null;
            Debug.Log("虚拟机已清理!");
        }
        
        
        public LuaTable GetInstanceOfScript(TextAsset asset)
        {
            var t = env.NewTable();
            var loaded = Load(asset);
            t.SetMetaTable(loaded.scriptMeta);
            return t;
        }
        
        LoadedScript Load(TextAsset asset)
        {
            if(loaded.TryGetValue(asset, out var res)) return res;
            
            // 脚本在一般情况下返回一个 table 表示这个类.
            // 脚本在非一般情况下返回 nil.
            // 否则都不合法.
            var ret = env.DoString(asset.text, asset.name);
            LuaTable table = null;
            if(ret.Length >= 1 && ret[0] != null) table = (LuaTable)ret[0];
            
            // 即使不是"类"的脚本也会有一个 metatable, 如果没有返回值那么里面什么都没有.
            var metaTable = env.NewTable();
            metaTable.SetInPath<LuaBase>("__index", table);
            
            res = new LoadedScript();
            res.asset = asset;
            res.scriptMeta = metaTable;
            return res;
        }
        
        void Reload(LuaScriptAsset asset)
        {
            loaded.Remove(asset);
            Load(asset);
        }
        
    }
    
    
}