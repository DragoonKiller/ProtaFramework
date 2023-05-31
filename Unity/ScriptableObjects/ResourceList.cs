


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
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
        Dictionary<string, UnityEngine.Object> cache = new Dictionary<string, UnityEngine.Object>();
        Dictionary<string, string> toLowerCache = new Dictionary<string, string>();
        
        public UnityEngine.Object Get(string name) => Get<UnityEngine.Object>(name);
        
        string CachedToLower(string name)
        {
            if(!toLowerCache.TryGetValue(name, out var x)) return name.ToLower();
            return x;
        }
        
        public T Get<T>(string name) where T: UnityEngine.Object
        {
            if(!cache.TryGetValue(CachedToLower(name), out var x))
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
            
            toLowerCache = resources.ToDictionary(x => x.name, x => x.name.ToLower());
            cache = resources.ToDictionary(x => x.name.ToLower(), x => x.target);
        }
        
    }
}
