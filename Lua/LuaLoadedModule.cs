using UnityEngine;
using UnityEditor;
using XLua;
using System.Collections.Generic;
using System;
using System.IO;
using Prota.Unity;

namespace Prota.Lua
{
    
    // 一个 LuaModule 代表一个加载完毕的 LuaScript 基类.
    // 这些 LuaScript 基类可以被替换和刷新.
    // 而 LuaScript 持有的 table 只存储数据.
    public sealed class LuaLoadedModule : IDisposable
    {
        public readonly string key;
        public readonly string path;
        
        public LuaCore core => LuaCore.instance;
        
        public readonly LuaTable meta;
        
        public LuaTable result;
        
        public LuaLoadedModule(string key)
        {
            this.key = key;
            this.path = LuaCore.KeyToSourcePath(key);
            Reload();
            
            // 即使不是"类"的脚本也会有一个 metatable, 如果没有返回值那么里面什么都没有.
            var metaTable = core.env.NewTable();
            metaTable.SetInPath<LuaBase>("__index", result);
            
            this.meta = metaTable;
        }
        
        
        public void Reload(bool log = false)
        {
            try
            {
                var ret = core.env.DoString($"return require '{ key }'", path);
                if(ret != null && ret.Length >= 1 && ret[0] != null) result = ret[0] as LuaTable;
            }
            catch(Exception e)
            {
                Log.Exception(e);
            }
            
            if(log)
            {
                Log.Info($"脚本重新加载! { key } | { path }");
            }
            
            // 没有返回值, 或返回值不是 table.
            // 是合法的操作.
            if(result == null)
            {
                // Log.Error($"脚本 [{ path }] 没有返回 table.");
                return;
            }
        }
        
        public void Attach(LuaTable target)
        {
            target.SetMetaTable(meta);
        }
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        public void Dispose(bool disposeManagedResources)
        {
            this.meta?.Dispose(disposeManagedResources);
        }
        
        public void Dispose() => Dispose(true);
        
        ~LuaLoadedModule() => Dispose(false);
    }
}