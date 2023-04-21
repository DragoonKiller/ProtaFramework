using System;
using System.Collections.Generic;
using UnityEngine;


namespace Prota.Unity
{
    public abstract class EntityComponent : MonoBehaviour
    {
        public EntityRoot entity { get; private set; }
        public static HashMapSet<Type, EntityComponent> instances = new HashMapSet<Type, EntityComponent>();
        
        protected virtual void Awake()
        {
            UpdateEntity();
            if(this.gameObject.scene != null) instances.AddElement(this.GetType(), this);
        }
        
        public virtual void OnValidate() => UpdateEntity();
        
        public void UpdateEntity()
        {
            entity = GetComponentInParent<EntityRoot>();
            entity?.AttachEntityComponent(this);
        }
        
        protected virtual void OnDestroy()
        {
            entity?.DetachEntityComponent(this);
            if(this.gameObject.scene != null) instances.RemoveElement(this.GetType(), this);
        }
    }
}

