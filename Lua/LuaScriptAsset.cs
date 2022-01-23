using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using System.Linq;

namespace Prota.Lua
{
    public class LuaScriptAsset : ScriptableObject
    {
        [SerializeField]
        public string path;
        
        [SerializeReference]
        public TextAsset asset;
    }
    
}