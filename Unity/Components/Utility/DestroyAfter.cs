using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Prota.Unity
{
    public class DestroyAfter : MonoBehaviour
    {
        [Serializable]
        public enum DestroyAfterEvent
        {
            Start,
            
            ActiveDestroy,
        }
        
        
        public DestroyAfterEvent destroyAfterEvent = DestroyAfterEvent.Start;
        
        public float delay = 0;
        
        void Start()
        {
            if(destroyAfterEvent == DestroyAfterEvent.Start)
            {
                this.gameObject.NewTimer(delay, () => Destroy(this.gameObject));
            }
        }
        
        void OnActiveDestroy()
        {
            if(destroyAfterEvent == DestroyAfterEvent.ActiveDestroy)
            {
                this.gameObject.NewTimer(delay, () => Destroy(this.gameObject));
            }
        }
        
    }
}
