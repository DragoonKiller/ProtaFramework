using System.Collections;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using Prota.Unity;
using System.Linq;
using System;
using UnityEngine.DedicatedServer;

namespace Prota.Unity
{

    public class PhysicsContactRecorder2D : MonoBehaviour
    {
        public struct ContactEntry2D
        {
            public readonly PhysicsEventType type;
            public readonly Collider2D collider;
            public readonly Collision2D collision;
            public readonly float time;
            
            public bool isEnter => type == PhysicsEventType.Enter;
            public bool isExit => type == PhysicsEventType.Exit;
            public bool isStay => type == PhysicsEventType.Stay;
            
            public ContactEntry2D(PhysicsEventType type, Collider2D collider, Collision2D collision, float time)
            {
                this.type = type;
                this.collider = collider;
                this.collision = collision;
                this.time = time;
            }
            
        }
        
        public LayerMask layerMask;
        
        // 记录当前帧发生的碰撞事件.
        // 由于碰撞进入和退出会在同一帧发生(多次fixedUpdate), 所以通过 colliders 可能搜不到碰撞.
        // 这个记录会在 lateUpdate 中删除; 建议在 Update 中处理这些数据.
        public readonly List<ContactEntry2D> events = new List<ContactEntry2D>();
        
        public readonly HashSet<Collider2D> colliders = new HashSet<Collider2D>();
        
        void OnCollisionEnter2D(Collision2D x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Add(x.collider);
            events.Add(new ContactEntry2D(PhysicsEventType.Enter, x.collider, x, Time.fixedTime));
        }
        
        void OnCollisionExit2D(Collision2D x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Remove(x.collider);
            events.Add(new ContactEntry2D(PhysicsEventType.Exit, x.collider, x, Time.fixedTime));
        }
        
        void OnCollisionStay2D(Collision2D x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            events.Add(new ContactEntry2D(PhysicsEventType.Stay, x.collider, x, Time.fixedTime));
        }
        
        void OnTriggerEnter2D(Collider2D c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Add(c);
            events.Add(new ContactEntry2D(PhysicsEventType.Enter, c, null, Time.fixedTime));
        }
        
        void OnTriggerExit2D(Collider2D c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            colliders.Remove(c);
            events.Add(new ContactEntry2D(PhysicsEventType.Stay, c, null, Time.fixedTime));
        }
        
        void OnTriggerStay2D(Collider2D c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            events.Add(new ContactEntry2D(PhysicsEventType.Exit, c, null, Time.fixedTime));
        }
        
        void Update()
        {
            colliders.RemoveWhere(x => x == null);
        }
        
        void LateUpdate()
        {
            events.Clear();
        }
        
        public bool HasContact<T>() where T: Collider2D => colliders.Any(x => x is T);
        
        public bool HasContact(Type t) => colliders.Any(x => x.GetType() == t);
        
        public bool HasContact(string layer) => colliders.Any(x => x.gameObject.layer == LayerMask.NameToLayer(layer));
        
        public Collider2D AnytContact<T>() where T: Collider2D => colliders.FirstOrDefault(x => x is T);
        
        public Collider2D AnyContact(Type t) => colliders.FirstOrDefault(x => x.GetType() == t);
        
        public Collider2D AnyContact(params string[] layer)
            => colliders.FirstOrDefault(x => layer.Contains(LayerMask.LayerToName(x.gameObject.layer)));
    }

    public static partial class UnityMethodExtensions
    {
        public static PhysicsContactRecorder2D PhysicsContactRecorder2D(this GameObject x)
            => x.GetComponent<PhysicsContactRecorder2D>();
    }


}
