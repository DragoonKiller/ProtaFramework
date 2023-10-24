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
        public struct ContactEntry2D
        {
            [ThreadStatic] public static Collider2D[] colliderBuffer = new Collider2D[16];
            [ThreadStatic] public static Collider2D[] contactBuffer = new Collider2D[16];
            
            public readonly PhysicsEventType type;
            public readonly Collision2D collision;
            public readonly Collider2D collider;
            public readonly Collider2D selfCollider;
            public readonly float time;
            
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
                
                selfCollider = self.GetComponent<Collider2D>();
                if(selfCollider != null) return;
                
                var rigid = self.GetComponent<Rigidbody2D>();
                if(rigid == null) throw new Exception($"GameObject [{self}] has no Collider2D or Rigidbody2D.");
                
                for(int n = rigid.GetAttachedColliders(colliderBuffer), i = 0; i < n && selfCollider == null; i++)
                {
                    var cc = ContactEntry2D.colliderBuffer[i];
                    if(n == 1)
                    {
                        selfCollider = cc;
                        break;
                    }
                    
                    for(int j = cc.GetContacts(contactBuffer); j >= 0  && selfCollider == null; j--)
                    {
                        var cx = ContactEntry2D.contactBuffer[j];
                        if(cx == cc) selfCollider = cc;
                    }
                }
                
                self.AssertNotNull();
                c.AssertNotNull();
                selfCollider.AssertNotNull();
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
