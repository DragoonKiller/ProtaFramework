using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prota.Unity
{
    
    public class EntityRoot : MonoBehaviour
    {
        public int id;
        
        [SerializeField, Readonly] List<EntityComponent> components = new List<EntityComponent>();
        
        public T GetEntityComponent<T>() where T : EntityComponent
            => components.Where(x => x is T).FirstOrDefault() as T;
        
        void OnDestroy()
        {
            if(!gameObject.IsDestroyed()) throw new Exception("EntityRoot should be destroyed with GameObject.");
        }
        
        
        public void AttachEntityComponent(EntityComponent c)
            => components.AddNoDuplicate(c);
        
        
        public void DetachEntityComponent(EntityComponent c)
            => components.Remove(c);
    }
}

