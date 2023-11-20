using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Prota.Unity
{
    [DisallowMultipleComponent]
    public class DestroyAfter : MonoBehaviour
    {
        [Serializable]
        public enum DestroyAfterEvent
        {
            Start,
            Manually,
        }
        
        
        public DestroyAfterEvent destroyAfterEvent = DestroyAfterEvent.Start;
        
        public float delay = 0;
        
        [Readonly] public bool destroyTriggered;
        
        void Start()
        {
            if(destroyAfterEvent == DestroyAfterEvent.Start)
            {
                DoDestroy();
            }
        }
        
        public void Trigger()
        {
            DoDestroy();
        }
        
        void DoDestroy()
        {
            if(destroyTriggered) return;
            destroyTriggered = true;
            this.gameObject.NewTimer(delay, () => this.gameObject.ActiveDestroy());
        }
    }
}
