using UnityEngine;
using UnityEditor;

namespace Prota.Lua
{
    public class LuaScriptAsset : UnityEngine.TextAsset
    {
        public string path;
        
        public LuaScriptAsset(string content) : base(content)
        {
            this.hideFlags = HideFlags.None;
            this.name = "Lua Script";
        }
        
        public override string ToString()
            => "Lua Script";
    }
}