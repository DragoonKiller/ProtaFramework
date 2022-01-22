using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;

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
}