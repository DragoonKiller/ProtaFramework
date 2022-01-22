using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using System.Linq;

namespace Prota.Lua
{

    [ScriptedImporter(1, "lua")]
    public class LuaImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var path = ctx.assetPath;
            var content = File.ReadAllText(path);
            var t = new LuaScriptAsset(content);
            t.name = ctx.assetPath;
            ctx.AddObjectToAsset("Content", t);
            ctx.SetMainObject(t);
        }
    }
    
    public class LuaPostImporter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            var assetGUIDs = AssetDatabase.FindAssets("t:LuaScriptList");
            if(assetGUIDs == null || assetGUIDs.Length == 0) return;
            var path = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
            var g = AssetDatabase.LoadAssetAtPath<LuaScriptList>(path);
            if(g == null) return;
            
            foreach(var s in deleted.Concat(movedFrom))
            {
                if(!s.EndsWith(".lua")) continue;
                g.Remove(s);
            }
            
            foreach(var s in imported.Concat(moved))
            {
                if(!s.EndsWith(".lua")) continue;
                g.Add(s);
            }
        }
    }
}
