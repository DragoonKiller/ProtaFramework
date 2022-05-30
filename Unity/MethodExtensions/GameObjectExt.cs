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
    }
}