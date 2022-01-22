using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using System.Collections;

namespace Prota.Data
{
    /// <summary>
    /// 简单数据存储结构.
    /// 相当于 ECS 中的 Component.
    /// </summary>
    [ExecuteInEditMode]
    public class DataBlock : MonoBehaviour
    {
        public DataCore core;
        
        void Awake()
        {
            Record();
            DetachAndAttach();
        }
        
        
        void OnDestroy()
        {
            Remove();
            Detach();
        }
        
        
        void OnTransformParentChanged() => DetachAndAttach();
        
        // 只有运行时有用.
        readonly static Dictionary<Type, HashSet<DataBlock>> blocks = new Dictionary<Type, HashSet<DataBlock>>();
        
        
        void Record()
        {
            blocks.GetOrCreate(this.GetType(), out var set);
            set.Add(this);
        }
        
        
        void Remove()
        {
            blocks.GetOrCreate(this.GetType(), out var set);
            set.Remove(this);
        }
        
        
        public void DetachAndAttach()
        {
            Detach();
            CheckAndAttach();
        }
        
        
        // 可以重复调用.
        public void CheckAndAttach()
        {
            var t = this.transform;
            while(t != null && (!t.TryGetComponent<DataCore>(out var c) || !c || c.destroying)) t = t.parent;
            if(t != null && t.TryGetComponent<DataCore>(out var core) && !(!core || core.destroying))
            {
                this.core = core;
                if(!core.data.Contains(this)) core.data.Add(this);
            }
        }
        
        
        public void Detach()
        {
            if(core != null)
            {
                core.data.Remove(this);
                core = null;
            }
        }
        
        
        public static IEnumerable<T> All<T>() where T : DataBlock
        {
            blocks.GetOrCreate(typeof(T), out var set);
            return set as IEnumerable<T>;
        }
        
        public static void Foreach<T>(Action<T> f) where T : DataBlock
        {
            foreach(var s in All<T>()) f(s);
        }
    }
}