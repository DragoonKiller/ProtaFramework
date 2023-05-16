using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prota.Unity
{
    
    public class ERoot : MonoBehaviour
    {
        public static HashSet<ERoot> entities = new HashSet<ERoot>();
        
        [SerializeField, Readonly] List<EComponent> components = new List<EComponent>();
        
        public T GetEntityComponent<T>() where T : EComponent
            => components.Where(x => x is T).FirstOrDefault() as T;
        
        public EComponent GetEntityComponent(Type t)
            => components.Where(x => x.GetType() == t).FirstOrDefault();
        
        public bool HasEntityComponent<T>() where T : EComponent
            => components.Any(x => x is T);
        
        public bool HasEntityComponent(Type t)
            => components.Any(x => x.GetType() == t);
        
        public IEnumerable<T> GetEntityComponents<T>() where T : EComponent
            => components.Select(x => x as T).Where(x => x != null);
            
        public IEnumerable<EComponent> GetEntityComponents(Type t)
            => components.Where(x => x.GetType() == t);
        
        void Awake()
        {
            entities.Add(this);
        }
        
        void ActiveDestroy()
        {
            if(!gameObject.IsDestroyed()) throw new Exception("EntityRoot should be destroyed with GameObject.");
        }
        
        void OnDestroy()
        {
            entities.Remove(this);
        }
        
        
        public void AttachEntityComponent(EComponent c)
            => components.AddNoDuplicate(c);
        
        
        public void DetachEntityComponent(EComponent c)
            => components.Remove(c);
    }
}

