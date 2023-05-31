


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prota.Unity
{
    [CreateAssetMenu(fileName = "ResourcesList", menuName = "Prota Framework/ResourcesCollection", order = 1)]
    public class ResourceCollection : ScriptableObject
    {
        public ResourceList[] lists = new ResourceList[0];
        public Dictionary<string, ResourceList> cache = new Dictionary<string, ResourceList>();
        Dictionary<string, string> toLowerCache = new Dictionary<string, string>();
        
        string CachedToLower(string name)
        {
            if(!toLowerCache.TryGetValue(name, out var x)) return name.ToLower();
            return x;
        }
        
        void OnEnable()
        {
            if(lists == null || lists.Length == 0)
            {
                Debug.LogWarning("ResourceCollection lists is null");
                return;
            }
            // Debug.Log($"ResourceList find[{ name }] list[{ lists.Select(x => x.name).ToStringJoined(",") }]");
            toLowerCache = lists.ToDictionary(x => x.name, x => x.name.ToLower());
            // Debug.Log($"ResourceList find[{ name }] list[{ toLowerCache.Keys.ToStringJoined(",") }]");
            cache = lists.ToDictionary(x => CachedToLower(x.name), x => x);
            // Debug.Log($"ResourceList find[{ name }] list[{ cache.Keys.ToStringJoined(",") }]");
        }
        
        public ResourceList this[string name]
        {
            get
            {
                // Debug.Log($"ResourceList find[{ name }] list[{ cache.Keys.ToStringJoined(",") }]");
                if(!cache.TryGetValue(CachedToLower(name), out var x))
                {
                    Debug.LogError($"ResourceList [{ name }] not found");
                    return null;
                }
                return x;
            }
        }
        
        static ResourceCollection _instance;
        public static ResourceCollection instance
        {
            get
            {
                if(_instance == null) _instance = Resources.Load<ResourceCollection>("ResourceCollection");
                _instance.AssertNotNull();
                return _instance;
            }
        }
        
    }
    
}
