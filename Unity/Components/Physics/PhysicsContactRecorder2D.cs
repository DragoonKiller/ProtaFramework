using System.Collections;
using System.Collections.Generic;
using Prota;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UIElements;
using System.Runtime.Serialization;

namespace Prota.Unity
{
    public class PhysicsContactRecorder2D : MonoBehaviour
    {
        public readonly struct ContactEntry2D
        {
            [ThreadStatic] public static Collider2D[] colliderBuffer = new Collider2D[16];
            [ThreadStatic] public static Collider2D[] contactBuffer = new Collider2D[16];
            
            public readonly PhysicsEventType type;
            public readonly Collision2D collision;
            public readonly Collider2D collider;
            public readonly Collider2D selfCollider;
            public readonly float time;
            public readonly Vector2 relativeVelocity;
            public readonly Vector2 normal;
            public readonly Vector2 contactPoint;
            public readonly bool isTriggerContact;
            
            public bool isCollisionContact => !isTriggerContact;
            public bool isValid => 0 < (int)type && (int)type <= 3;
            public bool isEnter => type == PhysicsEventType.Enter;
            public bool isExit => type == PhysicsEventType.Exit;
            public bool isStay => type == PhysicsEventType.Stay;
            
            public ContactEntry2D(PhysicsEventType type, Collision2D c, float time)
            {
                this.type = type;
                collider = c.collider;
                collision = c;
                selfCollider = c.otherCollider;
                this.time = time;
                this.relativeVelocity = c.relativeVelocity;
                this.normal = c.contactCount == 0 ? Vector2.zero : c.contacts[0].normal;
                this.contactPoint = c.contactCount == 0 ? Vector2.zero : c.contacts[0].point;
                this.isTriggerContact = false;
                
                #if UNITY_EDITOR
                isValid.Assert();
                c.AssertNotNull();
                collider.AssertNotNull();
                selfCollider.AssertNotNull();
                #endif
            }
            
            public ContactEntry2D(PhysicsEventType type, GameObject self, Collider2D c, float time)
            {
                this.type = type;
                collider = c;
                collision = null;
                this.time = time;
                this.relativeVelocity = Vector2.zero;
                this.isTriggerContact = true;
                
                self.AssertNotNull();
                c.AssertNotNull();
                
                GetColliderAndRigidbody(self, out selfCollider, out var selfHasCollider, out var selfRigid, out var selfHasRigid);
                GetColliderAndRigidbody(c.gameObject, out var otherCollider, out var otherHasCollider, out var otherRigid, out var otherHasRigid);
                
                var v1 = selfHasRigid ? selfRigid.velocity : Vector2.zero;
                var v2 = otherHasRigid ? otherRigid.velocity : Vector2.zero;
                this.relativeVelocity = v2 - v1;
                this.normal = Vector2.zero;
                this.contactPoint = Vector2.zero;
            }
            
            
            static void GetColliderAndRigidbody(GameObject self,
                out Collider2D c, out bool hasCollider,
                out Rigidbody2D r, out bool hasRigid)
            {
                hasCollider = self.TryGetComponent<Collider2D>(out c);
                hasRigid = self.TryGetComponent<Rigidbody2D>(out r);
                if(hasRigid) return;
                if(hasCollider)
                {
                    r = c.attachedRigidbody;
                    hasRigid = r != null;
                    return;
                }
                
                throw new Exception($"GameObject [{self}] has no Collider2D or Rigidbody2D.");
            }
        }
        
        // 有些时候, 例如修改 rigidbody layer 的时候, 会触发假的进入/退出事件.
        // 这个时候可以通过 disable 来禁用这个组件.
        public bool disable = true;
        
        public LayerMask layerMask = -1;
        
        // 记录当前帧发生的碰撞事件.
        // 由于碰撞进入和退出会在同一帧发生(多次fixedUpdate), 所以通过 colliders 可能搜不到碰撞.
        // 这个记录会在 lateUpdate 中删除; 建议在 Update 中处理这些数据.
        public readonly List<ContactEntry2D> events = new List<ContactEntry2D>();
        
        public readonly HashSet<Collider2D> colliders = new HashSet<Collider2D>();
        
            
        public event Action<Collision2D> onCollisionEnter;
        public event Action<Collision2D> onCollisionStay;
        public event Action<Collision2D> onCollisionExit;
        
        public event Action<Collider2D> onTriggerEnter;
        public event Action<Collider2D> onTriggerStay;
        public event Action<Collider2D> onTriggerExit;
        
        void OnValidate()
        {
            if(this.GetComponent<Collider2D>() == null && this.GetComponent<Rigidbody2D>() == null)
                Debug.LogWarning($"PhysicsContactRecorder2D on [{ this.gameObject }] requires a Collider2D or Rigidbody2D.");
            if(layerMask.value == 0)
                Debug.LogError($"PhysicsContactRecorder2D on [{ this.gameObject }] has no layerMask set.", this.gameObject);
        }
        
        void OnCollisionEnter2D(Collision2D x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            // Debug.LogError($"[{Time.fixedTime}] Enter { this.gameObject } <=> { x.collider.gameObject }");
            colliders.Add(x.collider);
            events.Add(new ContactEntry2D(PhysicsEventType.Enter, x, Time.fixedTime));
            onCollisionEnter?.Invoke(x);
        }
        
        void OnCollisionExit2D(Collision2D x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            // Debug.LogError($"[{Time.fixedTime}] Leave { this.gameObject } <=> { x.collider.gameObject }");
            colliders.Remove(x.collider);
            events.Add(new ContactEntry2D(PhysicsEventType.Exit, x, Time.fixedTime));
            onCollisionExit?.Invoke(x);
        }
        
        void OnCollisionStay2D(Collision2D x)
        {
            if(((1 << x.collider.gameObject.layer) & layerMask.value) == 0) return;
            events.Add(new ContactEntry2D(PhysicsEventType.Stay, x, Time.fixedTime));
            onCollisionStay?.Invoke(x);
        }
        
        void OnTriggerEnter2D(Collider2D c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            // Debug.LogError($"[{Time.fixedTime}] Enter { this.gameObject } <=> { c.gameObject }");
            colliders.Add(c);
            events.Add(new ContactEntry2D(PhysicsEventType.Enter, this.gameObject, c, Time.fixedTime));
            onTriggerEnter?.Invoke(c);
        }
        
        void OnTriggerExit2D(Collider2D c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            // Debug.LogError($"[{Time.fixedTime}] Leave { this.gameObject } <=> { c.gameObject }");
            colliders.Remove(c);
            events.Add(new ContactEntry2D(PhysicsEventType.Exit, this.gameObject, c, Time.fixedTime));
            onTriggerExit?.Invoke(c);
        }
        
        void OnTriggerStay2D(Collider2D c)
        {
            if(((1 << c.gameObject.layer) & layerMask.value) == 0) return;
            events.Add(new ContactEntry2D(PhysicsEventType.Stay, this.gameObject, c, Time.fixedTime));
            onTriggerStay?.Invoke(c);
        }
        
        void Update()
        {
            colliders.RemoveWhere(x => x == null);
        }
        
        void OnEnable()
        {
            // 需要吗?
            // colliders.Clear();
            // events.Clear();
        }
        
        void OnDisable()
        {
            colliders.Clear();
            events.Clear();
        }
        
        void LateUpdate()
        {
            events.Clear();
        }
        
        public bool HasContact() => colliders.Count > 0;
        
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
