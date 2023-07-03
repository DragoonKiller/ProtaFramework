using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota.Unity
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
    }
    
        // ============================================================================================================
        // ============================================================================================================
    
    public partial class UnityMethodExtensions
    {
        public static SortingOrderController SortingOrderController(this GameObject x)
            => x.GetOrCreate<SortingOrderController>();
            
        public static SortingOrderController SortingOrderController(this Component x)
            => x.GetOrCreate<SortingOrderController>();
        
        public static void SetSortingLayer(this GameObject x, int? layerId = null, int? orderInLayer = null)
        {
            var a = x.SortingOrderController();
            if(layerId.HasValue)  a.layer = layerId.Value;
            if(orderInLayer.HasValue) a.layer = orderInLayer.Value;
        }
        
        public static void SetSortingLayer(this Component x, int? layerId = null, int? orderInLayer = null)
            => x.gameObject.SetSortingLayer(layerId, orderInLayer);
    }
}
