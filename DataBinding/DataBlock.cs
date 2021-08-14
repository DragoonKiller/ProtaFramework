using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using System.Collections;

namespace Prota.Data
{
    [Serializable]
    public class DataBlock : ProtaScript, IEnumerable<DataBinding>
    {
        [Serializable]
        class SerializedRecord
        {
            [SerializeField]
            public string name;
            
            [SerializeField]
            public string type;
            
            [SerializeField]
            public SerializedData data;
        }
        
        
        
        public string blockName;
        
        /// <summary>
        /// SerializedRecord 的 view. 自带缓存机制.
        /// </summary>
        Dictionary<string, DataBinding> bindings = new Dictionary<string, DataBinding>();
        
        
        [SerializeField]
        List<SerializedRecord> data = new List<SerializedRecord>();
        
        
        [NonSerialized]
        bool sync = false;
        
        
        
        public SubAccessor sub => new SubAccessor() { block = this };
        
        public DataBinding this[string name]
        {
            get
            {
                Sync();
                if(bindings.TryGetValue(name, out var v)) return v;
                return null;
            }
        }
        
        
        public IEnumerator<DataBinding> GetEnumerator()
        {
            Sync();
            foreach(var i in bindings) yield return i.Value;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        
        public void Add(string name, DataBinding x)
        {
            Sync();
            if(bindings.ContainsKey(name))
            {
                Debug.LogError("添加了同名的 DataBinding 对象");
                return;
            }
            x.Bind(name, this);
            var newRecord = new SerializedRecord() {
                name = name,
                type = x.GetType().Name,
                data = new SerializedData()
            };
            newRecord.data.Clear();
            x.Serialize(newRecord.data);
            data.Add(newRecord);
            sync = false;
        }
        
        public void Remove(string name)
        {
            Sync();
            var binding = bindings[name];
            Remove(binding);
        }
        
        public void Remove(DataBinding x)
        {
            Sync();
            data.RemoveAll(y => y.name == x.name);
            x.Clear();
            sync = false;
        }
        
        public void MakeDirty() => sync = false;
        
        /// <summary>
        /// 将序列化数据同步到 view 上.
        /// </summary>
        void Sync()
        {
            if(sync) return;
            ForceSync();
            sync = true;
        }
        
        /// <summary>
        /// 强制刷新序列化数据.
        /// </summary>
        void ForceSync()
        {
            bindings.Clear();
            foreach(var d in data)
            {
                var x = Activator.CreateInstance(DataBinding.types[d.type]) as DataBinding;
                x.Bind(d.name, this);
                bindings.Add(d.name, x);
            }
        }
        
        public void WriteData()
        {
            foreach(var d in data)
            {
                if(!bindings.TryGetValue(d.name, out var binding)) continue;
                d.data.Clear();
                binding.Serialize(d.data);
            }
        }
        

        public struct SubAccessor : IEnumerable<DataBlock>
        {
            public DataBlock block;
            
            [ThreadStatic] static List<DataBlock> subCache;
            
            public DataBinding this[string name]
            {
                get
                {
                    foreach(var sub in this)
                    {
                        var val = sub[name];
                        if(val != null) return val;
                    }
                    return null;
                }
            }

            public IEnumerator<DataBlock> GetEnumerator()
            {
                if(subCache == null) subCache = new List<DataBlock>();
                
                for(int i = 0; i < block.transform.childCount; i++)
                {
                    var child = block.transform.GetChild(i);
                    child.GetComponentsInChildren<DataBlock>(subCache);
                    foreach(var sub in subCache)
                    {
                        yield return sub;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
        
        
        
    }
    
    
}