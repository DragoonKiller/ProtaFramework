using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prota.Unity
{
    [Serializable]
    public abstract class ESystem
    {
        [SerializeField, Readonly] string name;
        
        
        AsyncControl _beforeFixedUpdate;
        AsyncControl _afterFixedUpdate;
        AsyncControl _beforeUpdate;
        AsyncControl _afterUpdate;
        AsyncControl _beforeLateUpdate;
        AsyncControl _afterLateUpdate;
        
        public AsyncControl beforeFixedUpdate => _beforeFixedUpdate ?? (_beforeFixedUpdate = new AsyncControl());
        public AsyncControl afterFixedUpdate => _afterFixedUpdate ?? (_afterFixedUpdate = new AsyncControl());
        public AsyncControl beforeUpdate => _beforeUpdate ?? (_beforeUpdate = new AsyncControl());
        public AsyncControl afterUpdate => _afterUpdate ?? (_afterUpdate = new AsyncControl());
        public AsyncControl beforeLateUpdate => _beforeLateUpdate ?? (_beforeLateUpdate = new AsyncControl());
        public AsyncControl afterLateUpdate => _afterLateUpdate ?? (_afterLateUpdate = new AsyncControl());
        
        [SerializeReference]
        public List<ESubSystem> subSystems = new List<ESubSystem>();
        
        public bool disabled = false;
        
        public void InvokeFixedUpdate()
        {
            if(disabled) return;
            _beforeFixedUpdate?.Step();
            DoSystemFixedUpdate();
            foreach(var subSystem in subSystems) subSystem.DoSystemFixedUpdate();
            _afterFixedUpdate?.Step();
        }
        
        public void InvokeUpdate()
        {
            if(disabled) return;
            _beforeUpdate?.Step();
            DoSystemUpdate();
            foreach(var subSystem in subSystems) subSystem.DoSystemUpdate();
            _afterUpdate?.Step();
        }
        
        public void InvokeLateUpdate()
        {
            if(disabled) return;
            _beforeLateUpdate?.Step();
            DoSystemLateUpdate();
            foreach(var subSystem in subSystems) subSystem.DoSystemLateUpdate();
            _afterLateUpdate?.Step();
        }
        
        public void OnSystemCreate()
        {
            foreach(var subSystem in subSystems) subSystem.Init();
            DoSystemInit();
        }
        
        
        protected virtual void DoSystemInit() { }
        protected virtual void DoSystemFixedUpdate() { }
        protected virtual void DoSystemUpdate() { }
        protected virtual void DoSystemLateUpdate() { }
    }
    
}
