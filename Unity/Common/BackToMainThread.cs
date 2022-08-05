using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public struct BackToMainThread
    {
        static SynchronizationContext context;
        
        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            context = SynchronizationContext.Current;
            // Debug.Log($"Get SyncContext: { context }");
        }
        
        public SynchronizationContextAwaiter GetAwaiter()
        {
            if(context == null) throw new Exception("BackToMainThread should not use in Awake.");
            UnityEngine.Debug.Log(context);
            return context.GetAwaiter();
        }
    }
}