using System;
using UnityEngine;

namespace Prota
{
    public static partial class UnityMethodExtensions
    {
        public static T GetOrCreate<T>(this GameObject g) where T : Component
        {
            if(g.TryGetComponent<T>(out var res)) return res;
            var r = g.AddComponent<T>();
            return r;
        }
        
        
        public static Component GetOrCreate(this GameObject g, Type t)
        {
            if(!typeof(Component).IsAssignableFrom(t)) return null;
            if(g.TryGetComponent(t, out var res)) return res;
            var r = g.AddComponent(t);
            return r;
        }
        
        
        public static string GetNamePath(this GameObject g) => g.transform.GetNamePath();
        
        
        public static void Destroy(this GameObject g) => GameObject.Destroy(g);
        
        
        public static GameObject SetIdentity(this GameObject g)
        {
            g.transform.SetIdentity();
            return g;
        }
        
        public static GameObject ClearSub(this GameObject x)
        {
            x.transform.ClearSub();
            return x;
        }
        
        public static bool IsPrefab(this GameObject g) => !g.scene.IsValid();
        
        
        public static GameObject Clone(this GameObject g, Transform parent = null)
        {
            return GameObject.Instantiate(g, parent == null ? g.transform : parent, false);
        }
        
        
        public static GameObject SetParent(this GameObject g, Transform x = null, bool worldPositionStays = false)
        {
            g.transform.SetParent(x, worldPositionStays);
            return g;
        }
    }
}