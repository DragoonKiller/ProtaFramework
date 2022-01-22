using UnityEngine;
using UnityEditor;
using System;
using XLua;

namespace Prota.Lua
{
    public class LuaScript : MonoBehaviour
    {
        public TextAsset script = null;
        
        LuaTable instance;  // 实例对象.
        
        LuaTable[] selfArg;
        
        // ========================================================================================
        // ========================================================================================
        // ========================================================================================
        
        
        LuaFunction luaAwake;
        void Awake()
        {
            instance = LuaCore.instance.GetInstanceOfScript(script);
            selfArg = new LuaTable[] { instance };
            
            instance.SetInPath("gameObject", this.gameObject);
            instance.SetInPath("transform", this.transform);
            
            luaAwake = instance.GetInPath<LuaFunction>("Awake");
            luaStart = instance.GetInPath<LuaFunction>("Start");
            luaUpdate = instance.GetInPath<LuaFunction>("Update");
            luaFixedUpdate = instance.GetInPath<LuaFunction>("FixedUpdate");
            luaLateUpdate = instance.GetInPath<LuaFunction>("LateUpdate");
            luaOnDestroy = instance.GetInPath<LuaFunction>("OnDestroy");
            luaOnEnable = instance.GetInPath<LuaFunction>("OnDisabled");
            luaOnDisable = instance.GetInPath<LuaFunction>("OnEnabled");
            
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
            
            luaAwake = null;
            luaStart = null;
            luaUpdate = null;
            luaFixedUpdate = null;
            luaLateUpdate = null;
            luaOnDestroy = null;
            luaOnEnable = null;
            luaOnDisable = null;
        }
        
        
        // ========================================================================================
        // ========================================================================================
        // ========================================================================================
        
    }
}