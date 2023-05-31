using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public class PhysicsBehaviour : MonoBehaviour
    {
        public Action<PhysicsBehaviour, Collision> onCollisionEnter;
        public Action<PhysicsBehaviour, Collision> onCollisionStay;
        public Action<PhysicsBehaviour, Collision> onCollisionExit;
        
        public Action<PhysicsBehaviour, Collider> onTriggerEnter;
        public Action<PhysicsBehaviour, Collider> onTriggerStay;
        public Action<PhysicsBehaviour, Collider> onTriggerExit;
        
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
        public static PhysicsBehaviour PhysicsBehaviour(this GameObject g)
        {
            var t = g.AddComponent<PhysicsBehaviour>();
            return t;
        }
        
        public static PhysicsBehaviour WithOnCollisionEnter(this PhysicsBehaviour t, Action<PhysicsBehaviour, Collision> onCollisionEnter)
        {
            t.onCollisionEnter = onCollisionEnter;
            return t;
        }
        
        public static PhysicsBehaviour WithOnCollisionStay(this PhysicsBehaviour t, Action<PhysicsBehaviour, Collision> onCollisionStay)
        {
            t.onCollisionStay = onCollisionStay;
            return t;
        }
        
        public static PhysicsBehaviour WithOnCollisionExit(this PhysicsBehaviour t, Action<PhysicsBehaviour, Collision> onCollisionExit)
        {
            t.onCollisionExit = onCollisionExit;
            return t;
        }
        
        public static PhysicsBehaviour WithOnTriggerEnter(this PhysicsBehaviour t, Action<PhysicsBehaviour, Collider> onTriggerEnter)
        {
            t.onTriggerEnter = onTriggerEnter;
            return t;
        }
        
        public static PhysicsBehaviour WithOnTriggerStay(this PhysicsBehaviour t, Action<PhysicsBehaviour, Collider> onTriggerStay)
        {
            t.onTriggerStay = onTriggerStay;
            return t;
        }
        
        public static PhysicsBehaviour WithOnTriggerExit(this PhysicsBehaviour t, Action<PhysicsBehaviour, Collider> onTriggerExit)
        {
            t.onTriggerExit = onTriggerExit;
            return t;
        }
        
        
    }
}
