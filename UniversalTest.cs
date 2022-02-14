using UnityEngine;
using UnityEditor;

namespace Prota.Test
{
    public static class UniversalTest
    {
        [MenuItem("ProtaFramework/Test")]
        static void Test()
        {
            var a = GameObject.Find("#Game");
            a.AddComponent<Prota.Net.Server>();
            a.AddComponent<Prota.Net.Client>();
        }
    }
}