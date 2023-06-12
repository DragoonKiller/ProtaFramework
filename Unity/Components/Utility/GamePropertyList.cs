
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prota.Unity
{

    public class GamePropertyList : MonoBehaviour, IEnumerable<GameProperty>
    {
        [SerializeField] List<GameProperty> properties = new List<GameProperty>();
        
        public GameProperty this[string name]
        {
            get
            {
                foreach(var property in properties)
                {
                    if(property.name == name) return property;
                }
                throw new Exception($"Property [{name}] not found.");
            }
        }
        
        public GameProperty this[int index]
        {
            get
            {
                if(index < 0 || index >= properties.Count) throw new Exception($"Index [{index}] out of range.");
                return properties[index];
            }
        }
        
        public bool TryGet(string name, out GameProperty value)
        {
            foreach(var property in properties)
            {
                if (property.name != name) continue;
                value = property;
                return true;
            }
            value = null;
            return false;
        }
        
        public GamePropertyList Add(string name, float value)
        {
            if(TryGet(name, out GameProperty property)) throw new Exception($"Property [{name}] already exists.");
            property = new GameProperty(name, value);
            properties.Add(property);
            return this;
        }
        
        public bool Get(string name, out GameProperty value)
        {
            if(TryGet(name, out value)) return true;
            throw new Exception($"Property [{name}] not found.");
        }
        
        public void Clear()
        {
            properties.Clear();
        }

        public IEnumerator<GameProperty> GetEnumerator() => properties.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    
    
    public static partial class UnityMethodExtensions
    {
        public static GameProperty GetGameProperty(this GameObject gameObject, string name)
        {
            return gameObject.GetComponent<GamePropertyList>()[name];
        }
        
        public static GameProperty GetGameProperty(this Component component, string name)
        {
            return component.GetComponent<GamePropertyList>()[name];
        }
        
        public static void AddGameProperty(this GameObject gameObject, string name, float value)
        {
            gameObject.GetOrCreate<GamePropertyList>().Add(name, value);
        }
        
        public static void AddGameProperty(this Component component, string name, float value)
        {
            component.GetOrCreate<GamePropertyList>().Add(name, value);
        }
        
        public static bool HasGameProperty(this GameObject gameObject, string name)
        {
            return gameObject.GetComponent<GamePropertyList>().TryGet(name, out _);
        }
        
        public static bool HasGameProperty(this Component component, string name)
        {
            return component.GetComponent<GamePropertyList>().TryGet(name, out _);
        }
        
        public static bool TryGetGameProperty(this GameObject gameObject, string name, out GameProperty value)
        {
            return gameObject.GetComponent<GamePropertyList>().TryGet(name, out value);
        }
        
        public static bool TryGetGameProperty(this Component component, string name, out GameProperty value)
        {
            return component.GetComponent<GamePropertyList>().TryGet(name, out value);
        }
        
        public static IEnumerable<GameProperty> GetGamePropertyList(this GameObject gameObject)
        {
            return gameObject.GetComponent<GamePropertyList>();
        }
        
        public static IEnumerable<GameProperty> GetGamePropertyList(this Component component)
        {
            return component.GetComponent<GamePropertyList>();
        }
    }
    
    
}
