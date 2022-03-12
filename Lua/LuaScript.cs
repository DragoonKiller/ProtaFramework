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
        
        [NonSerialized]
        public LuaTable instance;  // 实例对象.
        
        [NonSerialized]
        public LuaLoadedModule loaded;   // 记录绑定的 metatable.
        
        public object[] selfArg;
        
        // ========================================================================================
        // ========================================================================================
        // ========================================================================================
        
        
        LuaFunction luaAwake;
        void Awake()
        {
            Load();
            
            if(luaAwake != null) luaAwake.Call(selfArg);
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
            Unload();
        }
        
        
        // ========================================================================================
        // ========================================================================================
        // ========================================================================================
        
        
        
        void Unload()
        {
            luaAwake = null;
            luaStart = null;
            luaUpdate = null;
            luaFixedUpdate = null;
            luaLateUpdate = null;
            luaOnDestroy = null;
            luaOnEnable = null;
            luaOnDisable = null;
        }
        
        
        void CreateData()
        {
            if(instance == null)
            {
                instance = LuaCore.instance.env.NewTable();
                instance.SetInPath("gameObject", gameObject);
                instance.SetInPath("transform", transform);
                instance.SetInPath("this", this);
                selfArg = new object[] { instance };
            }
            
            if(LuaCSExt.creationPath != null)
            {
                luaPath = LuaCSExt.creationPath;
                LuaCSExt.creationPath = null;
                instance.SetInPath("args", LuaCSExt.creationArgs);
                LuaCSExt.creationArgs = null;
            }
            
        }
        
        // 只更换 metatable.
        public void Load()
        {
            CreateData();
            
            if(!LuaCore.IsValidPath(luaPath))
            {
                Debug.LogError("路径不合法: " + this.gameObject.name + "\n" + luaPath);
                return;
            }
            
            var module = LuaCore.instance.Load(luaPath);
            if(module == null)
            {
                Debug.LogError("脚本加载失败: " + luaPath);
                return;
            }
            
            if(module == this.loaded) return; // metadata 完全相同不用重设.
            
            this.loaded = module;
            
            loaded.Attach(instance);
            
            luaAwake = instance.GetInPath<LuaFunction>("Awake");
            luaStart = instance.GetInPath<LuaFunction>("Start");
            luaUpdate = instance.GetInPath<LuaFunction>("Update");
            luaFixedUpdate = instance.GetInPath<LuaFunction>("FixedUpdate");
            luaLateUpdate = instance.GetInPath<LuaFunction>("LateUpdate");
            luaOnDestroy = instance.GetInPath<LuaFunction>("OnDestroy");
            luaOnEnable = instance.GetInPath<LuaFunction>("OnDisabled");
            luaOnDisable = instance.GetInPath<LuaFunction>("OnEnabled");
            
        }
        
        
    }
}