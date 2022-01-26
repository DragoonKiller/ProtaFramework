using UnityEngine;
using UnityEditor;
using System;
using XLua;
using System.Collections.Generic;
using Prota.Unity;
using System.Linq;

namespace Prota.Lua
{
    public class LuaScript : MonoBehaviour
    {
        public string luaPath;
        
        public LuaTable instance;   // 实例对象.
        
        public string loadedPath { get; private set; } = null;
        
        LuaTable[] selfArg;
        
        public static Dictionary<string, HashSet<LuaScript>> scripts = new Dictionary<string, HashSet<LuaScript>>();
        
        
        // ========================================================================================
        // ========================================================================================
        // ========================================================================================
        
        
        LuaFunction luaAwake;
        void Awake()
        {
            CreateData();
            if(!LuaCore.IsValidPath(luaPath)) return;
            LoadAndAdd();
        }
        
        
        LuaFunction luaStart;
        void Start()
        {
            if(luaStart != null) luaStart.Call(selfArg);
        }
        
        
        LuaFunction luaUpdate;
        void Update()
        {
            if(luaUpdate != null) luaUpdate.Call(selfArg);
        }
        
        LuaFunction luaFixedUpdate;
        void FixedUpdate()
        {
            if(luaFixedUpdate != null) luaFixedUpdate.Call(selfArg);
        }
        
        LuaFunction luaLateUpdate;
        void LateUpdate()
        {
            if(luaLateUpdate != null) luaLateUpdate.Call(selfArg);
        }
        
        
        LuaFunction luaOnEnable;
        void OnEnable()
        {
            if(luaOnEnable != null) luaOnEnable.Call(selfArg);
        }
        
        
        LuaFunction luaOnDisable;
        void OnDisable()
        {
            if(luaOnDisable != null) luaOnDisable.Call(selfArg);
        }
        
        
        LuaFunction luaOnDestroy;
        void OnDestroy()
        {
            if(luaOnDestroy != null) luaOnDestroy.Call(selfArg);
            UnloadAndRemove();
        }
        
        
        // ========================================================================================
        // ========================================================================================
        // ========================================================================================
        
        void CreateData()
        {
            instance = LuaCore.instance.env.NewTable();
            instance.SetInPath("gameObject", this.gameObject);
            instance.SetInPath("transform", this.transform);
            
            if(creationPath != null)
            {
                luaPath = creationPath;
                creationPath = null;
                instance.SetInPath("args", creationArgs);
                creationArgs = null;
            }
            
            selfArg = new LuaTable[] { instance };
        }
        
        void LoadAndAdd()
        {
            var res = LuaCore.instance.SetInstanceOfScript(instance, luaPath);
            if(res == null)
            {
                Debug.LogError("LuaScript 加载失败 " + luaPath);
                return;
            }
            
            scripts.GetOrCreate(luaPath, out var collection);
            collection.Add(this);
            loadedPath = luaPath;
            
        }
        
        void UnloadAndRemove()
        {
            if(loadedPath == null) return;
            
            scripts.GetOrCreate(luaPath, out var collection);
            collection.Remove(this);
            loadedPath = null;
            
            luaAwake = null;
            luaStart = null;
            luaUpdate = null;
            luaFixedUpdate = null;
            luaLateUpdate = null;
            luaOnDestroy = null;
            luaOnEnable = null;
            luaOnDisable = null;
            selfArg = null;
        }
        
        // 只更换 metatable.
        public void Reload(string path = null)
        {
            if(!LuaCore.IsValidPath(luaPath))
            {
                Debug.LogError("给定路径不合法: " + luaPath);
                return;
            }
            
            if(path != null)
            {
                UnloadAndRemove();
                this.luaPath = path;
                LoadAndAdd();
            }
            else
            {
                if(loadedPath != null)      // 必须有加载才能替换; 否则, 直接加载一个新的即可.
                {
                    scripts.GetOrCreate(loadedPath, out var collection);
                    foreach(var c in collection.ToArray())
                    {
                        c.Reload(luaPath);
                    }
                }
                else
                {
                    LoadAndAdd();
                }
            }
        }
        
        // ========================================================================================
        // ========================================================================================
        // ========================================================================================
        
        static string creationPath = null;
        static LuaTable creationArgs = null;
        
        [LuaCallCSharp]
        public static LuaTable CreateGameObject(string path, LuaTable args = null)
        {
            var g = new GameObject();
            return CreateScript(g, path, args);
        }
        
        [LuaCallCSharp]
        public static LuaTable CreateScript(GameObject go, string path, LuaTable args = null)
        {
            if(!LuaCore.IsValidPath(path))
            {
                Debug.LogError("给出的路径不正确: " + path);
                return null;
            }
            
            creationPath = path;
            creationArgs = args;
            var script = go.AddComponent<LuaScript>();
            return script.instance;
        }
        
        static List<LuaScript> getCache = new List<LuaScript>();
        [LuaCallCSharp]
        public static LuaTable Get(GameObject go, string path = null)
        {
            getCache.Clear();
            go.GetComponents<LuaScript>(getCache);
            foreach(var s in getCache)
            {
                if(path == null || s.luaPath == path)
                    return s.instance;
            }
            return null;
        }
    }
}