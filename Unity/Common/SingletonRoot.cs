using UnityEngine;
using System;
using System.Collections.Generic;
using Prota.Unity;

namespace Prota.Unity
{
    public class SingletonRoot : MonoBehaviour
    {
        public static SingletonRoot instance;
        
        public static Dictionary<Type, MonoBehaviour> pool = new Dictionary<Type, MonoBehaviour>();
        
        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        
        static MonoBehaviour Register<T>() where T : MonoBehaviour
        {
            return Register(typeof(T));
        }
        
        static MonoBehaviour Register(Type t)
        {
            if(!typeof(MonoBehaviour).IsAssignableFrom(t))
            {
                throw new ArgumentException("Singleton must be MonoBehaviour.");
            }
            
            GameObject g = null;
            
            // 先找有没有现成的.
            for(int i = 0; i < instance.transform.childCount; i++)
            {
                if(instance.transform.GetChild(i).gameObject.name == t.Name)
                {
                    // 找到了.
                    g = instance.transform.GetChild(i).gameObject;
                    break;
                }
            }
            
            if(g == null)
            {
                g = new GameObject(t.Name);
                g.transform.SetParent(instance.gameObject.transform, false);
            }
            
            g.transform.SetIdentity();
            var c = g.GetOrCreate(t);
            Debug.Assert(c as MonoBehaviour != null);
            pool.Add(t, c as MonoBehaviour);
            return c as MonoBehaviour;
        }
        
        public static void Deregister<T>()
        {
            if(!pool.TryGetValue(typeof(T), out var component)) return;
            component.gameObject.Destroy();
            pool.Remove(typeof(T));
        }
        
        public static T Get<T>() where T : MonoBehaviour
        {
            return Get(typeof(T)) as T;
        }
        
        public static Component Get(Type t)
        {
            if(pool.TryGetValue(t, out var component)) return component;
            var c = Register(t);
            return c;
        }
        
    }
    
    
    
    
    public static class Singleton
    {
        public static T Get<T>() where T: MonoBehaviour
        {
            return SingletonRoot.Get<T>();
        }
        
        public static void Clear<T>() where T: MonoBehaviour
        {
            SingletonRoot.Deregister<T>();
        }
        
        public static void Init(GameObject root)
        {
            SingletonRoot.instance = root.AddComponent<SingletonRoot>();
        }
        
        public static MonoBehaviour Get(Type t)
        {
            if(typeof(MonoBehaviour).IsAssignableFrom(t))
            {
                return SingletonRoot.Get(t) as MonoBehaviour;
            }
            else
            {
                throw new ArgumentException("Singleton must be MonoBehaviour.");
            }
        }
    }
    
}   