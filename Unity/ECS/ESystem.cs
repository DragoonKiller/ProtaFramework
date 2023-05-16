using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;

namespace Prota.Unity
{
    public abstract class ESystem : MonoBehaviour
    {
        [Serializable]
        public enum UpdateMode
        {
            None,
            First,
            Last,
        }
        
        // None: 不更新. First: 先于子节点更新. Last: 后于子节点更新.
        [SerializeField] public UpdateMode updateMode = UpdateMode.First;
        
        [SerializeField]
        public List<ESystem> subSystems = new List<ESystem>();
        
        void Awake()
        {
            UpdateHierarchy();
        }
        
        void UpdateHierarchy()
        {
            subSystems = this.transform.AsList<ESystem>()
                .Select(x => x.GetComponent<ESystem>())
                .Where(x => x != null)
                .ToList();
        }
        
        void OnValidate()
        {
            Update();
            UpdateHierarchy();
            this.transform.parent?.GetComponentInParent<ESystem>()?.OnValidate();
        }
        
        public virtual bool shouldUpdateWhenDisabled => false;
        
        public void InvokeSystemUpdate()
        {
            if(this.enabled == false && shouldUpdateWhenDisabled == false) return;
            if(updateMode == UpdateMode.First) DoSystemUpdate();
            foreach (var subSystem in subSystems) subSystem.InvokeSystemUpdate();
            if(updateMode == UpdateMode.Last) DoSystemUpdate();
        }
        
        public void InvokeSystemFixedUpdate()
        {
            if(this.enabled == false && shouldUpdateWhenDisabled == false) return;
            if(updateMode == UpdateMode.First) DoSystemFixedUpdate();
            foreach (var subSystem in subSystems) subSystem.InvokeSystemFixedUpdate();
            if(updateMode == UpdateMode.Last) DoSystemFixedUpdate();
        }
        
        public void InvokeSystemLateUpdate()
        {
            if(this.enabled == false && shouldUpdateWhenDisabled == false) return;
            if(updateMode == UpdateMode.First) DoSystemLateUpdate();
            foreach (var subSystem in subSystems) subSystem.InvokeSystemLateUpdate();
            if(updateMode == UpdateMode.Last) DoSystemLateUpdate();
        }
        
        protected virtual void DoSystemFixedUpdate() { }
        protected virtual void DoSystemUpdate() { }
        protected virtual void DoSystemLateUpdate() { }
        
        protected virtual void Update()
        {
            var name = this.GetType().Name;
            if(name == nameof(ESystem) || name == nameof(NodeSystem)) return;
            this.gameObject.name = name;
        }
    }
    
}
