using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota
{
    [RequireComponent(typeof(Renderer))]
    [ExecuteAlways]
    public class SortingOrderController : MonoBehaviour
    {
        
        public Renderer rd => this.GetComponent<Renderer>();
        
        public int layer;
        
        public int orderInLayer;
        
        void OnValidate()
        {
            rd.sortingLayerID = layer;
            rd.sortingOrder = orderInLayer;
        }
        
        void Awake()
        {
            OnValidate();
        }
        
        void OnDestroy()
        {
            // Renderer 可能已经被删除.
            if(this.TryGetComponent<Renderer>(out var renderer)) Destroy(renderer);
        }
    }
    
}
