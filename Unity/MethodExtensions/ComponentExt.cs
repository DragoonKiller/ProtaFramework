using System;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        
        public static T GetOrCreate<T>(this Component x) where T: Component
        {
            if(x.TryGetComponent<T>(out var res)) return res;
            return x.gameObject.AddComponent<T>();
        }
        
        // 新建时返回 false, 获取已有组件返回 true.
        public static bool GetOrCreate<T>(this Component x, out T res) where T: Component
        {
            if(x.TryGetComponent<T>(out res)) return true;
            res = x.gameObject.AddComponent<T>();
            return false;
        }
        
        public static T Clone<T>(this T g, Transform parent = null) where T: Component
        {
            return g.gameObject.Clone(parent).GetComponent<T>();
        }
        
        public static T CloneAsTemplate<T>(this T g, Transform parent = null) where T: Component
        {
            return g.gameObject.CloneAsTemplate(parent).GetComponent<T>();
        }
        
        // 设置 Component 对应的 Game Object 是否激活.
        public static T SetActive<T>(this T a, bool activate = true) where T: Component
        {
            a.gameObject.SetActive(activate);
            return a;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        public static RectTransform RectTransform(this Component x) => x.transform as RectTransform;
        
        
        // ============================================================================================================
        // ============================================================================================================
        
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
