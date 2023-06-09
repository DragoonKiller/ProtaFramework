using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;

namespace Prota.Unity
{
    [Serializable]
    public abstract class ESystem
    {
        [SerializeField, Readonly] string name;
        
        public bool disabled = false;
        
        public void InvokeFixedUpdate()
        {
            if(disabled)
                return;
            DoSystemFixedUpdate();
        }
        
        public void InvokeUpdate()
        {
            if(disabled)
                return;
            DoSystemUpdate();
        }
        
        public void InvokeLateUpdate()
        {
            if(disabled)
                return;
            DoSystemLateUpdate();
        }
        
        protected virtual void DoSystemFixedUpdate() { }
        protected virtual void DoSystemUpdate() { }
        protected virtual void DoSystemLateUpdate() { }
    }
    
}
