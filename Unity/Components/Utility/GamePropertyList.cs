
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Prota.Unity
{
    [DisallowMultipleComponent]
    public class GamePropertyList : MonoBehaviour, IEnumerable<GameProperty>
    {
        [Serializable] public class _List : SerializableDictionary<string, GameProperty> { }
        [SerializeField] _List properties = new _List();
        
        public GameProperty this[string name]
        {
            get
            {
                if(TryGet(name, out var value)) return value;
                throw new Exception($"Game Property [{name}] in [{this.gameObject.name}] not found.");
            }
        }
        
        public bool TryGet(string name, out GameProperty value)
        {
            return properties.TryGetValue(name, out value);
        }
        
        public GamePropertyList Add(string name, GameProperty property)
        {
            if(TryGet(name, out GameProperty value))
                throw new Exception($"Property [{name}] in [{this.gameObject.name}] already exists.");
            properties.Add(name, property);
            return this;
        }
        
        public GamePropertyList Add(string name, float value, GameProperty.Behaviour behaviour = GameProperty.Behaviour.Float)
        {
            if(TryGet(name, out GameProperty property))
                throw new Exception($"Property [{name}] in [{this.gameObject.name}] already exists.");
            property = new GameProperty(name, value);
            properties.Add(name, property);
            return this;
        }
        
        public GamePropertyList Remove(string name)
        {
            if(!TryGet(name, out GameProperty property))
                throw new Exception($"Property [{name}] in [{this.gameObject.name}] not found.");
            properties.Remove(name);
            return this;
        }
        
        public bool Get(string name, out GameProperty value)
        {
            if(TryGet(name, out value)) return true;
            throw new Exception($"Property [{name}] in [{this.gameObject.name}] not found.");
        }
        
        public void Clear()
        {
            properties.Clear();
        }

        public IEnumerator<GameProperty> GetEnumerator() => properties.Select(x => x.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        void OnValidate()
        {
            foreach(var property in properties)
            {
                property.Value.UpdateValue();
            }
        }
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
            value = null;
            return gameObject.GetComponent<GamePropertyList>()?.TryGet(name, out value) ?? false;
        }
        
        public static bool TryGetGameProperty(this Component component, string name, out GameProperty value)
        {
            value = null;
            return component.GetComponent<GamePropertyList>()?.TryGet(name, out value) ?? false;
        }
        
        public static GamePropertyList GetGamePropertyList(this GameObject gameObject)
        {
            return gameObject.GetComponent<GamePropertyList>();
        }
        
        public static GamePropertyList GetGamePropertyList(this Component component)
        {
            return component.GetComponent<GamePropertyList>();
        }
    }
    
    
}
