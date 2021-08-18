using UnityEngine;
using System;
using Prota.Unity;
using System.Collections.Generic;

namespace Prota.Data
{
    [Serializable]
    public abstract partial class DataBinding : ISerializable
    {
        [SerializeField]
        string _name;
        
        public string name
        {
            get => _name;
            private set => _name = value;
            
        }
        
        DataBlock _source;
        
        UnityEngine.Transform _transform;
        
        public DataBlock source => _source;
        public UnityEngine.Transform transform
        {
            get
            {
                if(_transform == null) _transform = source?.transform;
                return _transform;
            }
        }
        
        public void Bind(string name, DataBlock source)
        {
            _name = name;
            _source = source;
        }
        
        public void Clear()
        {
            _source = null;
            _transform = null;
        }

        public abstract void Deserialize(SerializedData s);
        public abstract void Serialize(SerializedData s);
        
        
        
        
        
        public static IReadOnlyDictionary<string, Type> types => _types;
        static readonly Dictionary<string, Type> _types = new Dictionary<string, Type>(); 
        
        static DataBinding()
        {
            foreach(var i in typeof(DataBinding).GetNestedTypes())
            {
                if(!typeof(DataBinding).IsAssignableFrom(i)) continue;
                _types.Add(i.Name, i);
            }
        }
        
    }
    
    
    
    
    [Serializable]
    public abstract class DataBindingBase<T> : DataBinding
    {
        [SerializeField]
        public T value;
    }
}