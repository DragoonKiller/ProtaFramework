using UnityEngine;

namespace Prota.Net
{
    internal static class Log
    {
        public static void Info(string s = "") => Debug.Log($"Prota:Net:{ s }");
        public static void Warning(string s = "") => Debug.Log($"Prota:Net:{ s }");
        public static void Error(string s = "") => Debug.Log($"Prota:Net:{ s }");
        public static void Exception(System.Exception e = null) => UnityEngine.Debug.LogException(e);
    }
}