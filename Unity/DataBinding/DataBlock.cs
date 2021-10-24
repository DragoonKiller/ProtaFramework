using System;
using System.Collections.Generic;
using UnityEngine;
using Prota.Unity;
using System.Collections;

namespace Prota.Data
{
    /// <summary>
    /// 简单数据存储结构.
    /// </summary>
    [ExecuteInEditMode]
    public class DataBlock : MonoBehaviour
    {
        public DataCore core;
        
        void Awake() => DetachAndAttach();
        
        void OnDestroy() => Detach();
        
        void OnTransformParentChanged() => DetachAndAttach();
        
        public void DetachAndAttach()
        {
            Detach();
            CheckAndAttach();
        }
        
        // 可以重复调用.
        public void CheckAndAttach()
        {
            var t = this.transform.parent;
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
    }
}