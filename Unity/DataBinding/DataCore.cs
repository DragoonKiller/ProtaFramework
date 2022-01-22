using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using System.Collections;





namespace Prota.Data
{
    // DataBlock 保证自己添加/删除/挪动位置的时候维持自身和任意一个 DataCore 的绑定.
    // DataCore 删除时, 会重新刷一遍它底下的所有 DataBlock, 让它们重新进行绑定.
    // DataCore 添加时, 会找到它的父级的 DataCore, 让父级 DataCore 重新刷所有子级绑定.
    //          注意 DataCore 不会主动刷新数据. 一般来说, 直接在上级创建一个 DataCore 并不会关连到任何 DataBlock
    //          因为 DataBlock 都有对应的绑定 DataCore 了. 在它下边添加 DataBlock 是 ok 的.
    // DataCore 移动时, 子集和自己所有脚本会刷新. 父级不用管.
    
    /// <summary>
    /// 数据集中访问接口.
    /// 相当于 ECS 中的 Entity.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class DataCore : MonoBehaviour
    {
        [SerializeField]
        public List<DataBlock> data = new List<DataBlock>();
        
        readonly Dictionary<string, DataBlock> content = new Dictionary<string, DataBlock>();
        
        [NonSerialized] public bool destroying = false;
        
        void Awake()
        {
            if(data != null)
            {
                // 列表不能有空值.
                foreach(var i in data) if(i == null)
                    Debug.LogError("DataCore 绑定了空物体.\n" + this.gameObject.GetNamePath());
                // 子项必须挂载到这个DataCore.
                foreach(var i in data) if(i.core != this)
                    Debug.LogError("DataCore 子项未正确绑定.\n" + this.gameObject.GetNamePath() + "\n" + i.gameObject.GetNamePath());
            }
            
            // 往上找到一个 DataCore, 对所有内容重新绑定.
            var upCore = this.transform.parent.GetComponentInParent<DataCore>();
            if(upCore != null)
            {
                upCore.Rebind();
            }
        }
        
        void OnDestroy()
        {
            destroying = true;
            Rebind();
        }
        
        void Rebind()
        {
            if(data != null)
            {
                foreach(var d in data.ToArray()) d.DetachAndAttach();
            }
        }
        
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