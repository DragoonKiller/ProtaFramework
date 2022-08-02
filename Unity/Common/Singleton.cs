using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota
{
    public abstract class Singleton<T> : MonoBehaviour
        where T: Singleton<T> 
    {
        public static T instance => Get();
        
        static T _instance;
        public static T Get()
        {
            if(_instance != null) return _instance;
            var g = new GameObject("#" + typeof(T).Name);
            GameObject.DontDestroyOnLoad(g);
            return _instance = g.AddComponent<T>();
        }
        
        public static void EnsureExists() => Get();
        
        protected Singleton() => _instance = (T)this;
        
    }
    
}   
