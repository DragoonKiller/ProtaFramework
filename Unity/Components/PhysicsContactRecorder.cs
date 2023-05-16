using System.Collections;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using Prota.Unity;
using System.Linq;
using System;

namespace Prota.Unity
{
    public enum PhysicsEventType
    {
        Enter = 1,
        Stay = 2,
        Exit = 3,
    }
    
    public class PhysicsContactRecorder : MonoBehaviour
    {
        public struct ContactEntry
        {
            public readonly PhysicsEventType type;
            public readonly Collider collider;
            public readonly Collision collision;
            public readonly float time;
            
            public bool isEnter => type == PhysicsEventType.Enter;
            public bool isExit => type == PhysicsEventType.Exit;
            public bool isStay => type == PhysicsEventType.Stay;

            public ContactEntry(PhysicsEventType type, Collider collider, Collision collision, float time)
            {
                this.type = type;
                this.collider = collider;
                this.collision = collision;
                this.time = time;
            }
        }
        
        public List<ContactEntry> entries = new List<ContactEntry>();
        
        public LayerMask layerMask;
        
        public readonly HashSet<Collider> colliders = new HashSet<Collider>();
        
        void OnCollisionEnter(Collision x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Add(x.collider);
            entries.Add(new ContactEntry(PhysicsEventType.Enter, x.collider, x, Time.fixedTime));
        }
        
        void OnCollisionExit(Collision x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Remove(x.collider);
            entries.Add(new ContactEntry(PhysicsEventType.Exit, x.collider, x, Time.fixedTime));
        }
        
        void OnCollisionStay(Collision x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            entries.Add(new ContactEntry(PhysicsEventType.Stay, x.collider, x, Time.fixedTime));
        }
        
        void OnTriggerEnter(Collider c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Add(c);
            entries.Add(new ContactEntry(PhysicsEventType.Enter, c, null, Time.fixedTime));
        }
        
        void OnTriggerExit(Collider c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Remove(c);
            entries.Add(new ContactEntry(PhysicsEventType.Exit, c, null, Time.fixedTime));
        }
        
        void OnTriggerStay(Collider c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            entries.Add(new ContactEntry(PhysicsEventType.Stay, c, null, Time.fixedTime));
        }
        
        void Update()
        {
            colliders.RemoveWhere(x => x == null);
        }
        
        
        void LateUpdate()
        {
            entries.Clear();
        }
        
        public bool HasContact<T>() where T: Collider => colliders.Any(x => x is T);
        
        public bool HasContact(Type t) => colliders.Any(x => x.GetType() == t);
        
        public bool HasContact(string layer) => colliders.Any(x => x.gameObject.layer == LayerMask.NameToLayer(layer));
        
        public Collider AnyContact<T>() where T: Collider => colliders.FirstOrDefault(x => x is T);
        
        public Collider AnyContact(Type t) => colliders.FirstOrDefault(x => x.GetType() == t);
        
        public Collider AnyContact(params string[] layer)
            => colliders.FirstOrDefault(x => layer.Contains(LayerMask.LayerToName(x.gameObject.layer)));
    }

    public static partial class UnityMethodExtensions
    {
        public static PhysicsContactRecorder PhysicsContactRecorder(this GameObject x)
        {
            return x.GetComponent<PhysicsContactRecorder>();
        }
    }


}
