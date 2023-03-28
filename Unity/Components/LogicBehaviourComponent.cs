using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public class LogicBehaviourComponent : MonoBehaviour
    {
        public Action<LogicBehaviourComponent> start;
        public Action<LogicBehaviourComponent> update;
        public Action<LogicBehaviourComponent> fixedUpdate;
        
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
        public static LogicBehaviourComponent LogicBehaviour(this GameObject g)
        {
            var t = g.AddComponent<LogicBehaviourComponent>();
            return t;
        }
        
        public static LogicBehaviourComponent WithStart(this LogicBehaviourComponent t, Action<LogicBehaviourComponent> start)
        {
            t.start = start;
            return t;
        }
        
        public static LogicBehaviourComponent WithUpdate(this LogicBehaviourComponent t, Action<LogicBehaviourComponent> update)
        {
            t.update = update;
            return t;
        }
        
        public static LogicBehaviourComponent WithFixedUpdate(this LogicBehaviourComponent t, Action<LogicBehaviourComponent> fixedUpdate)
        {
            t.fixedUpdate = fixedUpdate;
            return t;
        }
    }
}
