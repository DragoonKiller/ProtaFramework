using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prota.Unity
{
    // SubSystem 是由 ESystem 调用的小系统.
    // 一个 ESystem 可以有多个 SubSystem, 但是一个 SubSystem 只能属于一个 ESystem.
    // SubSystem 不会存储在 ECS 组件中, 而是存储在每个 ESystem 里.
    // Subsystem 在对应的 System 执行函数后面执行.
    [Serializable]
    public abstract class ESubSystem
    {
        public string name;
        
        public ESubSystem() => this.name = this.GetType().Name;
        
        public abstract Type systemType { get; }
        
        public ESystem system;
        
        public abstract void Init();
        public abstract void DoSystemUpdate();
        public abstract void DoSystemFixedUpdate();
        public abstract void DoSystemLateUpdate();
        
    }
    
}
