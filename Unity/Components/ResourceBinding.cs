using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    
    [ExecuteAlways]
    public class ResourceBinding : MonoBehaviour
    {
        [Serializable]
        struct Entry
        {
            public string name;
            public UnityEngine.Object target;
        }
        
        [SerializeField] List<Entry> data = new List<Entry>();
        
        readonly Dictionary<string, UnityEngine.Object> cache = new Dictionary<string, UnityEngine.Object>();
        
        public char featureCharacter = '$';
        
        void Awake()
        {
            if(Application.isPlaying) foreach(var x in data) cache.Add(x.name, x.target);
        }
        
        void OnValidate()
        {
            // data.RemoveAll(x => x.target == null);
            
            for(int i = 0; i < data.Count; i++)
            {
                if(data[i].target == null) continue;
                if(data[i].name == null || data[i].name.Length == 0)
                {
                    data[i] = new Entry { name = data[i].target.name, target = data[i].target };
                }
            }
        }
        
        void Update()
        {
            if(Application.isPlaying) return;
            OnValidate();
        }
        
        public T Get<T>(string name) where T: UnityEngine.Object
        {
            if(!cache.TryGetValue(name, out var res))
                throw new Exception($"ResourceBinding[{ this.gameObject.name }] 找不到  { name }");
            if(!(res is T t))
                throw new Exception($"ResourceBinding[{ this.gameObject.name }] 名为 { name } 的资源不是类型 { nameof(T) }");
            return t;
        }
        
        public bool TryGet<T>(string name, out T res) where T: UnityEngine.Object
        {
            res = default;
            if(!cache.TryGetValue(name, out var t)) return false;
            if(!(t is T tt)) return false;
            res = tt;
            return true;
        }
        
    }
    
    public static partial class UnityMethodExtensions
    {
        public static T GetResource<T>(this GameObject g, string name) where T: UnityEngine.Object
            => g.GetComponent<ResourceBinding>().Get<T>(name);
        
        public static bool TryGetResource<T>(this GameObject g, string name, out T res) where T: UnityEngine.Object
            => g.GetComponent<ResourceBinding>().TryGet<T>(name, out res);
        
        public static T GetResource<T>(this Component g, string name) where T: UnityEngine.Object
            => g.GetComponent<ResourceBinding>().Get<T>(name);
        
        public static bool TryGetResource<T>(this Component g, string name, out T res) where T: UnityEngine.Object
            => g.GetComponent<ResourceBinding>().TryGet<T>(name, out res);
        
        
    }
    
    
}
