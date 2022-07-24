using System;
using UnityEngine;

namespace Prota
{
    public static partial class UnityMethodExtensions
    {
        public static MaterialHandler GetMaterialHandler(this Component s, bool unique = true)
        {
            if(s.TryGetComponent<MaterialHandler>(out var res)) return res;
            return s.gameObject.AddComponent<MaterialHandler>();
        }
        
        public static Component MakeMaterialUnique(this Component s, bool unique = true)
        {
            var handler = s.GetMaterialHandler();
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
            var handler = s.GetMaterialHandler();
            Debug.Assert(handler);
            s.MakeMaterialUnique(unique);
            return handler.mat;
        }
        
        public static Material[] GetMaterialInstances(this Component s, bool unique = true)
        {
            var handler = s.GetMaterialHandler();
            Debug.Assert(handler);
            s.MakeMaterialUnique(unique);
            return handler.mats;
        }
        
        public static void SetMaterial(this Component s, Material material, int index = 0)
        {
            var handler = s.GetMaterialHandler();
            Debug.Assert(handler);
            handler.SetMaterialTemplate(index, material);
        }
    }
    
}