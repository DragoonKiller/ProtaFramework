using UnityEngine;

namespace Prota.Unity
{
    public static class Log
    {
        public static void Info(string s = "") => Debug.Log($"Prota:Unity:{ s }");
        public static void Warning(string s = "") => Debug.LogWarning($"Prota:Unity:{ s }");
        public static void Error(string s = "") => Debug.LogError($"Prota:Unity:{ s }");
        public static void Exception(System.Exception e = null) => UnityEngine.Debug.LogException(e);
    }
}