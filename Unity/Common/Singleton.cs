using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota
{
    public abstract class Singleton<T> : MonoBehaviour
        where T: Singleton<T> 
    {
        public static T instance => Get(null as GameObject);
        
        static T _instance;
        
        public static T Get(Component guard = null) => Get(guard.gameObject);
        public static T Get(GameObject guard = null)
        {
            if(guard != null && !guard.scene.isLoaded) return null;
            if(_instance != null) return _instance;
            var g = new GameObject("#" + typeof(T).Name);
            GameObject.DontDestroyOnLoad(g);
            return _instance = g.AddComponent<T>();
        }
        
        public static void EnsureExists() => Get(null as GameObject);
        
        protected Singleton() => _instance = (T)this;
        
    }
    
}   
