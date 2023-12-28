using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        [MenuItem("ProtaFramework/Execute [ProtaTest]")]
        public static void ExecuteProtaTestCommand()
        {
            var methods = TypeCache.GetMethodsWithAttribute<ProtaTestAttribute>().ToList();
            if(methods.Count == 0)
            {
                Debug.LogError("No [ProtaTest] methods found");
                return;
            }
            if(methods.Count > 1)
            {
                Debug.LogError("More than one [ProtaTest] method found");
                return;
            }
            
            var method = methods[0];
            
            var test = method.Invoke(null, null);
        }
    }
}
