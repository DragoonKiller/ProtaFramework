using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public class PhysicsBehaviourCompoenet : MonoBehaviour
    {
        public Action<PhysicsBehaviourCompoenet, Collision> onCollisionEnter;
        public Action<PhysicsBehaviourCompoenet, Collision> onCollisionStay;
        public Action<PhysicsBehaviourCompoenet, Collision> onCollisionExit;
        
        public Action<PhysicsBehaviourCompoenet, Collider> onTriggerEnter;
        public Action<PhysicsBehaviourCompoenet, Collider> onTriggerStay;
        public Action<PhysicsBehaviourCompoenet, Collider> onTriggerExit;
        
        void OnCollisionEnter(Collision c)
        {
            onCollisionEnter?.Invoke(this, c);
        }
        
        void OnCollisionStay(Collision c)
        {
            onCollisionStay?.Invoke(this, c);
        }
        
        void OnCollisionExit(Collision c)
        {
            onCollisionExit?.Invoke(this, c);
        }
        
        void OnTriggerEnter(Collider c)
        {
            onTriggerEnter?.Invoke(this, c);
        }
        
        void OnTriggerStay(Collider c)
        {
            onTriggerStay?.Invoke(this, c);
        }
        
        void OnTriggerExit(Collider c)
        {
            onTriggerExit?.Invoke(this, c);
        }
    }
    
    public static partial class UnityMethodExtensions
    {
        public static PhysicsBehaviourCompoenet PhysicsBehaviour(this GameObject g)
        {
            var t = g.AddComponent<PhysicsBehaviourCompoenet>();
            return t;
        }
        
        public static PhysicsBehaviourCompoenet WithOnCollisionEnter(this PhysicsBehaviourCompoenet t, Action<PhysicsBehaviourCompoenet, Collision> onCollisionEnter)
        {
            t.onCollisionEnter = onCollisionEnter;
            return t;
        }
        
        public static PhysicsBehaviourCompoenet WithOnCollisionStay(this PhysicsBehaviourCompoenet t, Action<PhysicsBehaviourCompoenet, Collision> onCollisionStay)
        {
            t.onCollisionStay = onCollisionStay;
            return t;
        }
        
        public static PhysicsBehaviourCompoenet WithOnCollisionExit(this PhysicsBehaviourCompoenet t, Action<PhysicsBehaviourCompoenet, Collision> onCollisionExit)
        {
            t.onCollisionExit = onCollisionExit;
            return t;
        }
        
        public static PhysicsBehaviourCompoenet WithOnTriggerEnter(this PhysicsBehaviourCompoenet t, Action<PhysicsBehaviourCompoenet, Collider> onTriggerEnter)
        {
            t.onTriggerEnter = onTriggerEnter;
            return t;
        }
        
        public static PhysicsBehaviourCompoenet WithOnTriggerStay(this PhysicsBehaviourCompoenet t, Action<PhysicsBehaviourCompoenet, Collider> onTriggerStay)
        {
            t.onTriggerStay = onTriggerStay;
            return t;
        }
        
        public static PhysicsBehaviourCompoenet WithOnTriggerExit(this PhysicsBehaviourCompoenet t, Action<PhysicsBehaviourCompoenet, Collider> onTriggerExit)
        {
            t.onTriggerExit = onTriggerExit;
            return t;
        }
        
        
    }
}
