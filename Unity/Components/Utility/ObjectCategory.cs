using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prota.Unity
{
    [ExecuteAlways]
    public class ObjectCategory : MonoBehaviour
    {
        public static readonly HashMapSet<string, ObjectCategory> objects
            = new HashMapSet<string, ObjectCategory>();
        
        public string[] categories = Array.Empty<string>();
        
        List<string> registeredCategories = new List<string>();
        
        void OnValidate()
        {
            foreach(var category in registeredCategories)
            {
                objects.RemoveElement(category, this);
            }
            registeredCategories.Clear();
            
            foreach(var category in categories)
            {
                objects.AddElement(category, this);
                registeredCategories.Add(category);
            }
        }
        
        public void OnEnable()
        {
            foreach(var category in categories)
            {
                objects.AddElement(category, this);
                registeredCategories.Add(category);
            }
        }
        
        public void OnDisable()
        {
            foreach(var category in categories)
            {
                registeredCategories.Remove(category);
                objects.RemoveElement(category, this);
            }
        }
        
        public static ObjectCategory Instance(string category)
        {
            return objects[category].FirstElement();
        }
        
        public static ObjectCategory InstanceOrNone(string category)
        {
            if(!objects.TryGetElement(category, out var res)) return null;
            return res;
        }
        
        public static IEnumerable<ObjectCategory> Instances(string category)
        {
            return objects[category];
        }
    }
    
    
}
