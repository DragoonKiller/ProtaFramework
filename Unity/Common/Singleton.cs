using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public abstract class Singleton<T> : MonoBehaviour
        where T: Singleton<T> 
    {
        public static T instance => Get();
        
        public static bool exists => _instance != null;
        static T _instance;
        public static T Get()
        {
            #if UNITY_EDITOR
            if(!Application.isPlaying) throw new InvalidOperationException("Cannot create singletion in edit mode!");
            #endif
            
            if(_instance != null) return _instance;
            if(_instance == null && !object.ReferenceEquals(null, _instance)) return null;
            var g = new GameObject("#" + typeof(T).Name);
            GameObject.DontDestroyOnLoad(g);
            return _instance = g.AddComponent<T>();
        }
        
        public static void EnsureExists() => Get();
        
        protected Singleton() => _instance = (T)this;
        
    }
    
}   
