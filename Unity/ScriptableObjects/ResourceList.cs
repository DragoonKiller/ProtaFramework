


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
                return null;
            }
        }
        public bool Has(string name) => resourceList.HasResource(name);
    }
    
    
    [CreateAssetMenu(fileName = "ResourcesList", menuName = "Prota Framework/ResourcesList", order = 1)]
    public class ResourceList : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public string name;
            public UnityEngine.Object target;
        }
        
        public List<Entry> resources;
        Dictionary<string, UnityEngine.Object> cache;
        Dictionary<string, string> toLowerCache;
        
        public void InvalidateCache()
        {
            cache = null;
            toLowerCache = null;
        }
        
        void SetupCache()
        {
            if(cache != null) return;
            cache = new Dictionary<string, UnityEngine.Object>();
            toLowerCache = new Dictionary<string, string>();
            foreach(var r in resources)
            {
                cache.Add(r.name.ToLower(), r.target);
                toLowerCache.Add(r.name, r.name.ToLower());
            }
        }
        
        public bool HasResource(string name)
        {
            SetupCache();
            return cache.ContainsKey(CachedToLower(name));
        }
        
        public UnityEngine.Object Get(string name) => Get<UnityEngine.Object>(name);
        
        string CachedToLower(string name)
        {
            if(name == null) return null;
            if(!toLowerCache.TryGetValue(name, out var x)) return name.ToLower();
            return x;
        }
        
        public bool TryGet(string name, out UnityEngine.Object x)
        {
            x = null;
            if(name.NullOrEmpty()) return false;
            SetupCache();
            return cache.TryGetValue(CachedToLower(name), out x);
        }
        
        public bool TryGet<T>(string name, out T x) where T: UnityEngine.Object
        {
            x = null;
            if(name.NullOrEmpty()) return false;
            SetupCache();
            if(cache.TryGetValue(CachedToLower(name), out var y))
            {
                x = (T)y;
                return true;
            }
            x = null;
            return false;
        }
        
        public T Get<T>(string name) where T: UnityEngine.Object
        {
            SetupCache();
            if(name == null || !cache.TryGetValue(CachedToLower(name), out var x))
            {
                Debug.Log($"ResourceList find[{ name }||{ CachedToLower(name) }] list[{ cache.Keys.ToStringJoined(",") }]");
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
            
            InvalidateCache();
            SetupCache();
        }
        
    }
}
