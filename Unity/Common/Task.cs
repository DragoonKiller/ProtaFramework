using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public class ProtaTask : Task
    {
        public ProtaTask(Action action) : base(action) { }
        public ProtaTask(Action action, CancellationToken cancellationToken) : base(action, cancellationToken) { }
        public ProtaTask(Action action, TaskCreationOptions creationOptions) : base(action, creationOptions) { }
        public ProtaTask(Action<object> action, object state) : base(action, state) { }
        public ProtaTask(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, cancellationToken, creationOptions) { }
        public ProtaTask(Action<object> action, object state, CancellationToken cancellationToken) : base(action, state, cancellationToken) { }
        public ProtaTask(Action<object> action, object state, TaskCreationOptions creationOptions) : base(action, state, creationOptions) { }
        public ProtaTask(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, state, cancellationToken, creationOptions) { }

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