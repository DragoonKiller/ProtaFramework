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
            var t = new TextAsset(content);
            t.name = ctx.assetPath;
            ctx.AddObjectToAsset("Content", t);
            var s = ScriptableObject.CreateInstance<LuaScriptAsset>();
            s.path = ctx.assetPath;
            s.asset = t;
            ctx.AddObjectToAsset("Config", s);
            ctx.SetMainObject(s);
        }
    }
    
    public class LuaPostImporter : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            var assetGuids = AssetDatabase.FindAssets("t:LuaScriptList");
            if(assetGuids == null || assetGuids.Length == 0) return;
            
            foreach(var assetGuid in assetGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetGuid);
                var g = AssetDatabase.LoadAssetAtPath<LuaScriptList>(path);
                if(g == null) continue;
                
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
}
