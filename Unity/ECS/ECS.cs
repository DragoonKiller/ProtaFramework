using System;
using System.Collections.Generic;
using UnityEngine;


namespace Prota.Unity
{
    [RequireComponent(typeof(ESystem))]
    public sealed class ECS : Singleton<ECS>
    {
        ESystem root;
        
        void OnValidate()
        {
            root = GetComponent<ESystem>();
        }
        
        void Awake()
        {
            OnValidate();
        }
        
        void FixedUpdate()
        {
            root.InvokeSystemFixedUpdate();
        }
        
        void LateUpdate()
        {
            root.InvokeSystemLateUpdate();
        }
        
        void Update()
        {
            root.InvokeSystemUpdate();
        }
    }
}

