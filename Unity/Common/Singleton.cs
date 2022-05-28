using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public class Singleton : MonoBehaviour
    {
        public static Singleton instance;
        
        public static Dictionary<Type, MonoBehaviour> pool = new Dictionary<Type, MonoBehaviour>();
        
        public static Dictionary<Type, List<Action>> onInit = new Dictionary<Type, List<Action>>();
        
        Singleton()
        {
            instance = this;
        }
        
        void Awake()
        {
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
            var c = g.GetOrCreate(t) as MonoBehaviour;
            Debug.Assert(c != null);
            pool.Add(t, c);
            
            onInit.GetOrCreate(t, out var list);
            foreach(var f in list) f();
            list.Clear();
            
            return c;
        }
        
        public static void Deregister<T>()
        {
            Deregister(typeof(T));
        }
        
        public static void Deregister(Type t)
        {
            if(!pool.TryGetValue(t, out var component)) return;
            component.gameObject.Destroy();
            pool.Remove(t);
            onInit.GetOrCreate(t, out var list);
            list.Clear();
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
    
}   