using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Prota.Unity
{
    public abstract class EComponent : MonoBehaviour
    {
        public ERoot entity { get; private set; }
        public static HashMapSet<Type, EComponent> instances = new HashMapSet<Type, EComponent>();
        
        public static IEnumerable<T> Instances<T>() where T : EComponent => instances[typeof(T)].Cast<T>();
        
        protected virtual void Awake()
        {
            UpdateEntity();
            entity.AssertNotNull();
            if(this.gameObject.scene != null)
            {
                instances.AddElement(this.GetType(), this);
            }
        }
        
        protected virtual void OnValidate() => UpdateEntity();
        
        public void UpdateEntity()
        {
            entity = GetComponentInParent<ERoot>();
            entity?.AttachEntityComponent(this);
        }
        
        protected virtual void OnDestroy()
        {
            entity?.DetachEntityComponent(this);
            if(this.gameObject.scene != null)
            {
                instances.RemoveElement(this.GetType(), this);
            }
        }
        
        public T ComponentAside<T>() where T : EComponent => entity.GetEntityComponent<T>();
        
        public IEnumerable<T> ComponentsAside<T>() where T : EComponent => entity.GetEntityComponents<T>();
    }
}

