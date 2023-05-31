using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prota.Unity
{
    [ExecuteAlways]
    public sealed class ERoot : MonoBehaviour
    {
        public static HashSet<ERoot> entities = new HashSet<ERoot>();
        
        [SerializeField, Readonly] HashMapList<Type, EComponent> components = new HashMapList<Type, EComponent>();
        
        public bool TryGetEntityComponent<T>(out T c) where T : EComponent
        {
            c = GetEntityComponent<T>();
            return c != null;
        }
        
        public bool TryGetEntityComponent(Type t, out EComponent c)
        {
            c = GetEntityComponent(t);
            return c != null;
        }
        
        public T GetEntityComponent<T>() where T : EComponent => GetEntityComponent(typeof(T)) as T;
        
        public EComponent GetEntityComponent(Type t)
            => components.FirstElement(t);
        
        public bool HasEntityComponent<T>() where T : EComponent
            => HasEntityComponent(typeof(T));
        
        public bool HasEntityComponent(Type t)
            => components.TryGetValue(t, out var res) && res.Count > 0;
        
        public IEnumerable<T> GetEntityComponents<T>() where T : EComponent
            => GetEntityComponents(typeof(T)).Cast<T>();
            
        public IEnumerable<EComponent> GetEntityComponents(Type t)
            => components[t];
        
        void Awake()
        {
            entities.Add(this);
        }
        
        void ActiveDestroy()
        {
            if(!gameObject.IsDestroyed()) throw new Exception("EntityRoot should be destroyed with GameObject.");
        }
        
        void Update()
        {
            if(Application.isPlaying) return;
            components.RemoveElement(x => x == null);
        }
        
        void OnDestroy()
        {
            entities.Remove(this);
        }
        
        
        public void AttachEntityComponent(EComponent c)
        {
            components.AddElementNoDuplicate(c.GetType(), c);
        }
        
        public void DetachEntityComponent(EComponent c)
        {
            components.RemoveElement(c.GetType(), c);
        }
    }
}

