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
        
        public static T Clone<T>(this T g, Transform parent = null)
            where T: Component
        {
            return g.gameObject.Clone(parent).GetComponent<T>();
        }
        
        // 设置 Component 对应的 Game Object 是否激活.
        public static T SetActive<T>(this T a, bool activate = true) where T: Component
        {
            a.gameObject.SetActive(activate);
            return a;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        
        public static MaterialHandler MaterialHandler(this Component s)
            => s.GetOrCreate<MaterialHandler>();
        
        public static Component MakeMaterialUnique(this Component s)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            return s;
        }
        
        public static Material GetMaterialInstance(this Component s)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            return handler.rd.material;
        }
        
        public static Material[] GetMaterialInstances(this Component s)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            return handler.rd.materials;
        }
        
        public static void SetMaterial(this Component s, Material material, int index = 0)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            handler.rd.SetMaterial(material, index);
        }
        
        
        // ============================================================================================================
        // ============================================================================================================
        
        public static AssignSortingOrder AssignSortingOrder(this Component x)
            => x.GetOrCreate<AssignSortingOrder>();
        
        public static void SetSortingLayer(this Component x, int? layerId = null, int? orderInLayer = null)
        {
            var a = x.AssignSortingOrder();
            if(layerId.HasValue)  a.layer = layerId.Value;
            if(orderInLayer.HasValue) a.layer = orderInLayer.Value;
        }
        
    }
    
}
