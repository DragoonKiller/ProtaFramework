using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota
{
    [RequireComponent(typeof(Renderer))]
    [ExecuteAlways]
    public class AssignSortingOrder : MonoBehaviour
    {
        
        public Renderer rd => this.GetComponent<Renderer>();
        
        [SerializeField] int _layer;
        
        [SerializeField] int _orderInLayer;
        
        public int layer
        {
            get => _layer;
            set => _layer = rd.sortingLayerID = value;
        }
        
        public int orderInLayer
        {
            get => _orderInLayer;
            set => _orderInLayer = rd.sortingOrder = value;
        }
        
        void Awake()
        {
            layer = rd.sortingLayerID;
            orderInLayer = rd.sortingOrder;
        }
        
        void Update()
        {
            rd.sortingLayerID = layer;
            rd.sortingOrder = orderInLayer;
        }
        
        void OnDestroy()
        {
            if(Application.isPlaying)
            {
                if(this.TryGetComponent<Renderer>(out var renderer)) Destroy(renderer);
            }
        }
    }
    
}
