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
        public LuaCore.LoadedScript loaded;   // 记录绑定的 metatable.
        
        object[] selfArg;
        
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
        
        
        void CreateData()
        {
            if(instance == null)
            {
                instance = LuaCore.instance.env.NewTable();
                instance.SetInPath("gameObject", this.gameObject);
                instance.SetInPath("transform", this.transform);
                instance.SetInPath("this", this);
                selfArg = new object[] { instance };
            }
            
            if(creationPath != null)
            {
                luaPath = creationPath;
                creationPath = null;
                instance.SetInPath("args", creationArgs);
                creationArgs = null;
            }
            
        }
        
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
        
        
        // 只更换 metatable.
        public void Load()
        {
            CreateData();
            
            if(!LuaCore.IsValidPath(luaPath))
            {
                Debug.LogError("路径不合法: " + this.gameObject.name + "\n" + luaPath);
                return;
            }
            
            var meta = LuaCore.instance.Load(luaPath);
            if(meta == null)
            {
                Debug.LogError("脚本加载失败: " + luaPath);
            }
            
            if(meta == this.loaded) return; // metadata 完全相同不用重设.
            
            this.loaded = meta;
            
            instance.SetMetaTable(meta.scriptMeta);
            
            luaAwake = instance.GetInPath<LuaFunction>("Awake");
            luaStart = instance.GetInPath<LuaFunction>("Start");
            luaUpdate = instance.GetInPath<LuaFunction>("Update");
            luaFixedUpdate = instance.GetInPath<LuaFunction>("FixedUpdate");
            luaLateUpdate = instance.GetInPath<LuaFunction>("LateUpdate");
            luaOnDestroy = instance.GetInPath<LuaFunction>("OnDestroy");
            luaOnEnable = instance.GetInPath<LuaFunction>("OnDisabled");
            luaOnDisable = instance.GetInPath<LuaFunction>("OnEnabled");
            
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
        
        
        [LuaCallCSharp]
        public static bool ObjectIsNull(object x)
        {
            if(x is UnityEngine.Object o) return o == null;
            return x == null;
        }
        
        [LuaCallCSharp]
        public static void Activate(object target, bool value = true)
        {
            switch(target)
            {
                case Behaviour c:
                c.enabled = value;
                break;
                
                case GameObject g:
                g.SetActive(value);
                break;
                
                case Transform t:
                t.gameObject.SetActive(value);
                break;
                
                default:
                Debug.LogError("Activate类型不正确 " + target.ToString());
                break;
            }
        }
        
        [LuaCallCSharp]
        public static LuaTable GetDataBinding(GameObject g)
        {
            var d = g.GetComponent<LuaDataBinding>();
            return d.GetLuaTable();
        }
    }
}