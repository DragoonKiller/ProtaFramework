using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;


namespace Prota.Unity
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ProtaSerialzieAttribute : Attribute { }
    
    
    public static class ProtaScriptSerializer
    {
        
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        
        class Serializer
        {
            public Type type;
            public List<FieldInfo> fields;
            public List<PropertyInfo> properties;
            
            public Serializer(Type type)
            {
                this.type = type;
                fields = type
                    .GetFields(bindingFlags)
                    .Where(x => x.CustomAttributes
                        .Where(attr => attr.AttributeType == typeof(ProtaSerialzieAttribute))
                        .Count() != 0
                    ).ToList();
                properties = type
                    .GetProperties(bindingFlags)
                    .Where(x => x.CustomAttributes
                        .Where(attr => attr.AttributeType == typeof(ProtaSerialzieAttribute))
                        .Count() != 0
                    ).Where(x => x.GetMethod != null && x.SetMethod != null)
                    .ToList();
            }
            
            public void Serialize(ProtaScript script)
            {
                Debug.Assert(type == script.GetType());
                foreach(var f in fields)
                {
                    
                }
                foreach(var p in properties)
                {
                    
                }
            }
        }
        
        static Dictionary<Type, Serializer> serializer = new Dictionary<Type, Serializer>();
        
        static void Serialize(ProtaScript script)
        {
            
        }
        
        static void Deserialize(ProtaScript script)
        {
            
        }
    }
}