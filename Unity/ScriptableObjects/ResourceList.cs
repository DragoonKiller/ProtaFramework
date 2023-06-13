


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
        
        static Dictionary<string, string> toLowerCache = new Dictionary<string, string>();
        
        string CachedToLower(string name)
        {
            if(name == null) return null;
            if(toLowerCache.TryGetValue(name, out var x)) return x;
            x = name.ToLower();
            toLowerCache.Add(name, x);
            return x;
        }
        
        public bool HasResource(string name)
        {
            return resources.ContainsKey(CachedToLower(name));
        }
        
        public UnityEngine.Object Get(string name) => Get<UnityEngine.Object>(name);
        
        public bool TryGet(string name, out UnityEngine.Object x)
        {
            x = null;
            if(name.NullOrEmpty()) return false;
            return resources.TryGetValue(CachedToLower(name), out x);
        }
        
        public bool TryGet<T>(string name, out T x) where T: UnityEngine.Object
        {
            x = null;
            if(name.NullOrEmpty()) return false;
            if(resources.TryGetValue(CachedToLower(name), out var y))
            {
                x = (T)y;
                return true;
            }
            x = null;
            return false;
        }
        
        public T Get<T>(string name) where T: UnityEngine.Object
        {
            if(name == null || !resources.TryGetValue(CachedToLower(name), out var x))
            {
                Debug.Log($"ResourceList find[{ name }||{ CachedToLower(name) }] list[{ resources.Keys.ToStringJoined(",") }]");
                Debug.LogError($"Resource in [{ this.name }] with name [{name}] not found");
                return null;
            }
            return (T)x;
        }
        
        void OnEnable()
        {
            if(resources == null || resources.Count == 0)
            {
                Debug.LogWarning("ResourceList resources is null");
                return;
            }
        }
        
    }
}
