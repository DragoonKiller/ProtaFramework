using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public class LogicBehaviour : MonoBehaviour
    {
        public Action<LogicBehaviour> start;
        public Action<LogicBehaviour> update;
        public Action<LogicBehaviour> fixedUpdate;
        
        void Start()
        {
            start?.Invoke(this);
        }
        
        void Update()
        {
            update?.Invoke(this);
        }
        
        void FixedUpdate()
        {
            fixedUpdate?.Invoke(this);
        }
    }
    
    public static partial class UnityMethodExtensions
    {
        public static LogicBehaviour LogicBehaviour(this GameObject g)
        {
            var t = g.AddComponent<LogicBehaviour>();
            return t;
        }
        
        public static LogicBehaviour WithStart(this LogicBehaviour t, Action<LogicBehaviour> start)
        {
            t.start = start;
            return t;
        }
        
        public static LogicBehaviour WithUpdate(this LogicBehaviour t, Action<LogicBehaviour> update)
        {
            t.update = update;
            return t;
        }
        
        public static LogicBehaviour WithFixedUpdate(this LogicBehaviour t, Action<LogicBehaviour> fixedUpdate)
        {
            t.fixedUpdate = fixedUpdate;
            return t;
        }
    }
}
