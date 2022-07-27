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
        
        // ============================================================================================================
        // ============================================================================================================
        
        
        
        public static MaterialHandler MaterialHandler(this Component s)
            => s.GetOrCreate<MaterialHandler>();
        
        public static Component MakeMaterialUnique(this Component s, bool unique = true)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            
            if(unique)
            {
                handler.MakeUnique();
            }
            else
            {
                handler.MakeShared();
            }
            
            return s;
        }
        
        public static Material GetMaterialInstance(this Component s, bool unique = true)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            s.MakeMaterialUnique(unique);
            return handler.mat;
        }
        
        public static Material[] GetMaterialInstances(this Component s, bool unique = true)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            s.MakeMaterialUnique(unique);
            return handler.mats;
        }
        
        public static void SetMaterial(this Component s, Material material, int index = 0)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            handler.SetMaterialTemplate(index, material);
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
