


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    public class ResourceListSpecialized<T> where T: UnityEngine.Object
    {
        public ResourceList resourceList;

        public ResourceListSpecialized(ResourceList resourceList)
        {
            this.resourceList = resourceList;
        }
        
        public T this[string name]
        {
            get
            {
                if(resourceList.TryGet<T>(name, out var res)) return res;
                throw new Exception($"ResourceList [{ resourceList.name }] has no resource named [{ name }]");
            }
        }
        public bool Has(string name) => resourceList.HasResource(name);
    }
    
    
    [CreateAssetMenu(fileName = "ResourcesList", menuName = "Prota Framework/ResourcesList", order = 1)]
    public class ResourceList : ScriptableObject
    {
        [Serializable]
        public class _Entry : SerializableDictionary<string, UnityEngine.Object> { }
        [SerializeField] public _Entry resources;
        
        public bool HasResource(string name)
        {
            return resources.ContainsKey(name.GetLowerStr());
        }
        
        public UnityEngine.Object Get(string name) => Get<UnityEngine.Object>(name);
        
        public bool TryGet(string name, out UnityEngine.Object x)
        {
            x = null;
            if(name.NullOrEmpty()) return false;
            return resources.TryGetValue(name.GetLowerStr(), out x);
        }
        
        public bool TryGet<T>(string name, out T x) where T: UnityEngine.Object
        {
            x = null;
            if(name.NullOrEmpty()) return false;
            if(resources.TryGetValue(name.GetLowerStr(), out var y))
            {
                x = (T)y;
                return true;
            }
            x = null;
            return false;
        }
        
        public T Get<T>(string name) where T: UnityEngine.Object
        {
            if(name == null || !resources.TryGetValue(name.GetLowerStr(), out var x))
            {
                Debug.Log($"ResourceList find[{ name }||{ name.GetLowerStr() }] list[{ resources.Keys.ToStringJoined(",") }]");
                Debug.LogError($"Resource in [{ this.name }] with name [{name}] not found");
                return null;
            }
            return (T)x;
        }
        
        void OnEnable()
        {
            if(resources == null || resources.Count == 0)
            {
                Debug.LogWarning($"ResourceList [{this.name}] is null");
                return;
            }
        }
        
    }
}
