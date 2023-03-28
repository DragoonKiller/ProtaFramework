using System.Collections;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using Prota.Unity;
using System.Linq;

namespace Prota.Unity
{

    public class PhysicsContactRecorder : MonoBehaviour
    {
        public LayerMask layerMask;
        
        public readonly HashSet<Collider> colliders = new HashSet<Collider>();
        
        void OnCollisionEnter(Collision x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Add(x.collider);
        }
        
        void OnCollisionExit(Collision x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Remove(x.collider);
        }
        
        void OnTriggerEnter(Collider c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Add(c);
        }
        
        void OnTriggerExit(Collider c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Remove(c);
        }
        
        void Update()
        {
            colliders.RemoveWhere(x => x == null);
        }
        
    }

    public static partial class UnityMethodExtensions
    {
        public static PhysicsContactRecorder PhysicsContactRecorder(this GameObject x)
        {
            return x.GetComponent<PhysicsContactRecorder>();
        }
    }


}
