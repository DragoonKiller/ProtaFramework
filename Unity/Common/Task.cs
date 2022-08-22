using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public static class ProtaTask
    {
        public static Task Run(Action action, CancellationToken? token = null)
        {
            var t = token ?? CancellationToken.None;
            return Task.Run(() => {
                try
                {
                    action();
                }
                catch(Exception e)
                {
                    UnityEngine.Debug.LogException(e);
                }
            }, t);
        }
    }
    
}