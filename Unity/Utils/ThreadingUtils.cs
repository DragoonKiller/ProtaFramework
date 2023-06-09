using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public struct SwitchToMainThread
    {
        static SynchronizationContext _context;
        public static SynchronizationContext context
        {
            get
            {
                if(_context != null) return _context;
                _context = SynchronizationContext.Current;
                
                // this should not happen since the most early execution is [InitializeOnLoadMethod]
                // so either SwitchToMainThread.Init is excuted normally
                // or this getter is executed in some [InitializeOnLoadMethod]
                if(_context == null) throw new Exception("context is used too early, try delay it.");
                
                return _context;
            }
        }
        
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            if(_context == null) _context = SynchronizationContext.Current;
            // Debug.Log($"Get SyncContext: { context }");
        }
        
        public SynchronizationContextAwaiter GetAwaiter()
        {
            if(_context == null) throw new Exception("BackToMainThread should not use in Awake.");
            return context.GetAwaiter();
        }
    }
    
    public struct WaitForNextFrame
    {
        public SynchronizationContextAwaiter GetAwaiter()
        {
            if(SwitchToMainThread.context == null) throw new Exception("BackToMainThread should not use in Awake.");
            return SwitchToMainThread.context.GetAwaiter();
        }
    }
    
    public struct SwitchToWorkerThread
    {   
        public SynchronizationContextAwaiter GetAwaiter()
        {
            return (null as SynchronizationContext).GetAwaiter();
        }
    }
    
    
    public static partial class MethodExtensions
    {
        public static Task<T> HandleError<T>(this Task<T> t)
        {
            return t.ContinueWith(task =>
            {
                if(task.IsFaulted) Debug.LogException(task.Exception);
                return task.Result;
            });
        }
        
        public static Task HandleError(this Task t)
        {
            return t.ContinueWith(task =>
            {
                if(task.IsFaulted) Debug.LogException(task.Exception);
            });
        }
    }
    
}
