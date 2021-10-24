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
        
        void Awake()
        {
            OnTransformParentChanged();
        }
        
        void OnDestroy()
        {
            Detach();
        }
        
        void OnTransformParentChanged()
        {
            Detach();
            
            var t = this.transform.parent;
            while(t != null && !t.TryGetComponent<DataCore>(out _)) t = t.parent;
            if(t != null && t.TryGetComponent<DataCore>(out var core))
            {
                this.core = core;
                if(!core.data.Contains(this)) core.data.Add(this);
            }
        }
        
        void Detach()
        {
            if(core != null)
            {
                core.data.Remove(this);
                core = null;
            }
        }
    }
}