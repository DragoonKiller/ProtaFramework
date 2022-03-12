
using UnityEngine;
using UnityEditor;
using XLua;
using System.Collections.Generic;
using System;
using System.IO;

namespace Prota.Lua
{
    [LuaCallCSharp]
    public static class LuaCSExt
    {
        
        
        
        
        // ========================================================================================
        // ========================================================================================
        // ========================================================================================
        
        public static string creationPath = null;
        public static LuaTable creationArgs = null;
        
        public static LuaTable CreateGameObject(string path, LuaTable args = null)
        {
            var g = new GameObject();
            return CreateScript(g, path, args);
        }
        
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
        
        
        public static bool ObjectIsNull(object x)
        {
            if(x is UnityEngine.Object o) return o == null;
            return x == null;
        }
        
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
        
        public static LuaTable GetDataBinding(GameObject g)
        {
            var d = g.GetComponent<LuaDataBinding>();
            return d.GetLuaTable();
        }
    }
}