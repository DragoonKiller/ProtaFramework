using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Prota.Unity
{
    // 通过明明规则实现数据绑定.
    // 如果gameObject名字以featureCharacter开头, 则它会被DataBinding记录.
    [ExecuteAlways]
    public class DataBinding : MonoBehaviour
    {
        public bool includeSelf = false;
        
        [Serializable]
        public class _Data : SerializableDictionary<string, GameObject> {}
        [SerializeField] _Data data = new _Data();
        
        public char featureCharacter = '$';
        
        void OnValidate()
        {
            using var _ = TempHashSet.Get<string>(out var g);
            
            data.Clear();
            
            transform.ForeachTransformRecursively(t =>
            {
                if(t.name.StartsWith(featureCharacter)
                    && (includeSelf || this.transform != t))
                {
                    var s = t.name.Substring(1);
                    data.Add(s, t.gameObject);
                    if(g.Contains(s)) Debug.LogError($"DataBinding[{ this.GetNamePath() }] 有重复的名字 { s }");
                    g.Add(s);
                }
                if(t.GetComponent<DataBinding>().PassValue(out var tt) != null && tt != this) return false;
                return true;
            });
        }
        
        void Update()
        {
            if(Application.isPlaying) return;
        }
        
        public GameObject this[string name] => Get(name);
        
        public GameObject Get(string name)
        {
            if(!data.TryGetValue(name, out var res))
                throw new Exception($"DataBinding[{ this.GetNamePath() }] 找不到 GameObject { name }");
            return res.gameObject;
        }
        
        public T Get<T>(string name)
        {
            if(!data.TryGetValue(name, out var res))
                throw new Exception($"DataBinding[{ this.GetNamePath() }] 找不到 GameObject { name }");
            if(!res.TryGetComponent<T>(out var c))
                throw new Exception($"DataBinding[{ this.GetNamePath() }] 找到了GameObject { name } 但是找不到组件 { typeof(T).Name }");
            return c;
        }
        
        public bool TryGet(string name, out GameObject res)
        {
            res = null;
            if(!data.TryGetValue(name, out var t)) return false;
            res = t.gameObject;
            return true;
        }
        
        public bool TryGet<T>(string name, out T res)
        {
            res = default;
            if(!data.TryGetValue(name, out var t)) return false;
            res = t.GetComponent<T>();
            if(res == null) return false;
            return true;
        }
        
        public IEnumerable<GameObject> All()
        {
            return data.Values.Select(t => t.gameObject);
        }
        
        public IEnumerable<(string name, GameObject g)> all
        {
            get
            {
                return data.Select(kv => (kv.Key, kv.Value.gameObject));
            }
        }
    }
    
    public static partial class UnityMethodExtensions
    {
        public static DataBinding DataBinding(this GameObject self)
            => self.GetComponent<DataBinding>();
        
        public static DataBinding DataBinding(this Component self)
            => self.GetComponent<DataBinding>();
        
        public static GameObject GetBinding(this GameObject self, string name)
            => self.DataBinding().Get(name);
        
        public static GameObject GetBinding(this Component self, string name)
            => self.gameObject.GetBinding(name);
        
        public static T GetBinding<T>(this GameObject self, string name)
            => self.DataBinding().Get<T>(name);
        
        public static T GetBinding<T>(this Component self, string name)
            => self.gameObject.GetBinding<T>(name);
        
        public static bool TryGetBinding(this GameObject self, string name, out GameObject res)
        {
            res = null;
            var dataBinding = self.gameObject.DataBinding();
            if(dataBinding == null) return false;
            if(dataBinding.TryGet(name, out res)) return true;
            return false;
        }
        
        public static bool TryGetBinding(this Component self, string name, out GameObject res)
            => self.gameObject.TryGetBinding(name, out res);
        
        public static bool TryGetBinding<T>(this GameObject self, string name, out T res)
        {
            res = default;
            var dataBinding = self.gameObject.DataBinding();
            if(dataBinding == null) return false;
            if(dataBinding.TryGet(name, out res)) return true;
            return false;
        }
        
        public static bool TryGetBinding<T>(this Component self, string name, out T res)
            => self.gameObject.TryGetBinding(name, out res);
    }
    
    
}
