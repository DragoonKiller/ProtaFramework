using UnityEngine;

namespace Prota.Lua
{
    internal static class Log
    {
        public static void Info(string s = "") => Debug.Log($"Prota:Lua:{ s }");
        public static void Warning(string s = "") => Debug.LogWarning($"Prota:Lua:{ s }");
        public static void Error(string s = "") => Debug.LogError($"Prota:Lua:{ s }");
        public static void Exception(System.Exception e = null) => UnityEngine.Debug.LogException(e);
    }
}