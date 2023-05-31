using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Prota.Unity
{
    [ExecuteAlways]
    public abstract class EComponent : MonoBehaviour
    {
        public class TypeRecord
        {
            public Type type;
            public bool modified;
            public readonly List<EComponent> cache = new List<EComponent>();
            public readonly HashSet<EComponent> component = new HashSet<EComponent>();
        }
        
        public ERoot entity { get; private set; }
        public static readonly Dictionary<Type, TypeRecord> records = new Dictionary<Type, TypeRecord>();
        
        public static IEnumerable<T> Instances<T>() where T : EComponent
        {
            if(!records.TryGetValue(typeof(T), out var record)) return Enumerable.Empty<T>();
            if(record.modified)
            {
                record.cache.Clear();
                record.cache.AddRange(record.component);
                record.modified = false;
            }
            return record.cache.Where(x => x != null).Cast<T>();
        }
        
        public static T Instance<T>() where T : EComponent
        {
            var res = Instances<T>();
            if(res.Count() != 1) throw new Exception($"There are {res.Count()} instances of {typeof(T)}, { res.ToStringJoined() }");
            return res.First();
        }
        
        protected virtual void Awake()
        {
            if(this.gameObject.scene == null) return;
            InitAttachment();
            entity.AssertNotNull();
            AddToRecord();
        }
        
        protected virtual void OnEnable()
        {
            AddToRecord();
        }
        
        protected virtual void OnDisable()
        {
            RemoveFromRecord();
        }
        
        void InitAttachment()
        {
            entity = this.GetComponentInParent<ERoot>();
            entity?.AttachEntityComponent(this);
        }
        
        protected virtual void Update()
        {
            if(Application.isPlaying) return;
            InitAttachment();
        }
        
        
        protected virtual void OnDestroy()
        {
            if(this.gameObject.scene == null) return;
            entity?.DetachEntityComponent(this);
            RemoveFromRecord();
        }
        
        public T ComponentAside<T>() where T : EComponent => entity.GetEntityComponent<T>();
        
        public IEnumerable<T> ComponentsAside<T>() where T : EComponent => entity.GetEntityComponents<T>();
        
        public T ComponentAside<T>(string name) where T : EComponent
        {
            foreach(var comp in ComponentsAside<T>())
            {
                if(comp.name == name) return comp;
            }
            return null;
        }
        
        public bool ComponentAside<T>(out T res) where T : EComponent
        {
            res = entity.GetEntityComponent<T>();
            return res != null;
        }
        
        public bool ComponentsAside<T>(out IEnumerable<T> res) where T: EComponent
        {
            res = entity.GetEntityComponents<T>();
            return res != null;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void AddToRecord()
        {
            records.GetOrCreate(this.GetType(), out var record);
            record.component.Add(this);
            record.modified = true;
        }
        
        void RemoveFromRecord()
        {
            records.GetOrCreate(this.GetType(), out var record);
            record.component.Remove(this);
            record.modified = true;
        }
    }
}

