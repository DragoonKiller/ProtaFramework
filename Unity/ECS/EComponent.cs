using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Prota.Unity
{
    [ExecuteAlways]
    public abstract class EComponent : MonoBehaviour
    {
        [NonSerialized] bool awaken = false;
        
        public class TypeRecord
        {
            public Type type;
            public readonly List<EComponent> components = new List<EComponent>();
        }
        
        public ERoot entity { get; private set; }
        
        int? indexInRecord = null;
        
        public static readonly Dictionary<Type, TypeRecord> records = new Dictionary<Type, TypeRecord>();
        
        public static IEnumerable<T> Instances<T>() where T : EComponent
        {
            if(!records.TryGetValue(typeof(T), out var record)) return Enumerable.Empty<T>();
            return record.components.Cast<T>();
        }
        
        public static T Instance<T>() where T : EComponent
        {
            if(!records.TryGetValue(typeof(T), out var record))
                throw new Exception($"There is no instance of {typeof(T)}");
            
            if(record.components.Count != 1)
                throw new Exception($"There are {record.components.Count} instances of {typeof(T)}");
            
            return (T)record.components[0];
        }
        
        protected virtual void Awake()
        {
            awaken = true;
            if(this.gameObject.scene == null) return;
            InitAttachment();
            entity.AssertNotNull();
            AddToRecord();
        }
        
        protected virtual void OnEnable()
        {
            if(awaken) AddToRecord();
            else Awake();
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
            if(!awaken)
            {
                Debug.LogError($"EComponent [{this.GetType()}] on [{this.GetNamePath()}] has not called Awake. Make sure base.Awake() is called when override.");
                return;
            }
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
        
        public bool HasComponentAside<T>() where T: EComponent => entity.HasEntityComponent<T>();
        
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
            return entity.TryGetEntityComponent<T>(out res);
        }
        
        public bool ComponentsAside<T>(out IEnumerable<T> res) where T: EComponent
        {
            return entity.TryGetEntityComponents<T>(out res);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void AddToRecord()
        {
            if(this.indexInRecord != null)
            {
                records.GetOrCreate(this.GetType(), out var r);
                if(r.components[indexInRecord.Value] == this) return;
                throw new Exception("Already added to record, but it is broken.");
            }
            
            records.GetOrCreate(this.GetType(), out var record);
            record.components.Add(this);
            this.indexInRecord = record.components.Count - 1;
        }
        
        void RemoveFromRecord()
        {
            if(this.indexInRecord == null)
            {
                records.GetOrCreate(this.GetType(), out var r);
                if(!r.components.Contains(this)) return;
                throw new Exception("Already removed from record, but it is broken.");
            }
            
            records.GetOrCreate(this.GetType(), out var record);
            if(record.components.Count == 1) // 只有自己.
            {
                record.components.RemoveLast();
            }
            else if(record.components.Last() == this) // 最后一个恰好是自己.
            {
                record.components.RemoveLast();
            }
            else // 和最后一个交换, 需要更改最后一个组件的下标记录.
            {
                var lastElement = record.components.Last();
                record.components[indexInRecord.Value] = lastElement;
                lastElement.indexInRecord = indexInRecord;
                record.components.RemoveLast();
            }
            
            this.indexInRecord = null;
        }
    }
}

