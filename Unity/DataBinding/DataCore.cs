using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using System.Collections;

namespace Prota.Data
{
    [ExecuteInEditMode]
    public class DataCore : MonoBehaviour
    {
        [SerializeField]
        public List<DataBlock> data = new List<DataBlock>();
        
        readonly Dictionary<string, DataBlock> content = new Dictionary<string, DataBlock>();
        
        // 如果结果是 null, 复杂度 O(n)
        // 如果结果有东西, 会被 cache, 复杂度 O(1)
        public DataBlock this[string name]
        {
            get
            {
                // 已经在缓存里了.
                if(content.TryGetValue(name, out var res))
                {
                    // 是这个缓存.
                    if(res != null && res.core == this) return res;
                    
                    // 已经换了位置, 缓存失效.
                    content.Remove(name);
                }
                
                // 遍历查找.
                foreach(var d in data) if(d.gameObject.name == name)
                {
                    // 找到了先放进缓存.
                    content.Add(name, d);
                    return d;
                }
                
                // 找不到.
                return null;
            }
        }
        
        public DataBlock Get(string name) => Get<DataBlock>(name);
        
        public T Get<T>(string name) where T : DataBlock => this[name] as T;
        
        public T Get<T>() where T : DataBlock
        {
            foreach(var d in data) if(d is T res) return res;
            return null;
        }
        
        public DataBlock Get(string name, out DataBlock x) => x = Get<DataBlock>(name);
        
        public T Get<T>(string name, out T x) where T : DataBlock => x = this[name] as T;
        
        public T Get<T>(out T x) where T : DataBlock
        {
            foreach(var d in data) if(d is T res) return x = res;
            return x = null;
        }
    }
}