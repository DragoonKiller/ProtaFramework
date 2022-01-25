using UnityEngine;
using XLua;
using System;
using System.Collections.Generic;

namespace Prota.Lua
{
    [DisallowMultipleComponent()]
    public class LuaDataBinding : MonoBehaviour
    {
        [Serializable]
        public struct Entry
        {
            public string name;
            public UnityEngine.Object target;
        }
        
        public List<Entry> targets = new List<Entry>();
        
        [LuaCallCSharp]
        public LuaTable GetLuaTable()
        {
            var res = LuaCore.instance.env.NewTable();
            foreach(var entry in targets)
            {
                res.Set<string, UnityEngine.Object>(entry.name, entry.target);
            }
            return res;
        }
        
        
    }
}