using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using LiteNetLib;
using System.Linq;

namespace Prota.Net
{
    public static class ProtocolInfoCollector
    {
        public static void ValidateAllProtocols()
        {
            var arr = AppDomain.CurrentDomain
                .GetAssemblies()
                .Aggregate(new List<Type>(), (list, a) => { list.AddRange(a.GetTypes()); return list; })
                .Where(x => x.GetCustomAttribute<ProtaProtocol>() != null)
                .ToList();
            
            try
            {
                foreach(var type in arr) GetInfo(type);
            }
            finally { }
            
            // ids = new PairDictionary<Type, int>();
            // methods = new Dictionary<Type, DeliveryMethod>();
        }
        
        static PairDictionary<Type, int> ids = new PairDictionary<Type, int>();
        static Dictionary<Type, DeliveryMethod> methods = new Dictionary<Type, DeliveryMethod>();
        
        static void GetInfo(Type type)
        {
            if(ids.TryGetValue(type, out var id)) return;
            
            var a = type.GetCustomAttribute<ProtaProtocol>();
            if(a == null) throw new ArgumentException($"type { type.Name } does not have ProtocolId attribute");
            
            id = a.id;
            if(ids.TryGetKeyByValue(id, out var oriType)) throw new ArgumentException($"Protocol id duplicate: [{ type.Name }] and [{ oriType }] uses [{ id }]");
            
            ids.Add(type, id);
            methods.Add(type, a.method);
        }
        
        public static DeliveryMethod GetMethod(Type type)
        {
            if(methods.TryGetValue(type, out var res)) return res;
            GetInfo(type);
            return methods[type];
        }
        
        public static int GetId(Type type)
        {
            if(ids.TryGetValue(type, out var id)) return id;
            GetInfo(type);
            return ids[type];
        }
        
        public static int GetProtocolId(this Type type) => GetId(type);
        public static DeliveryMethod GetProtocolMethod(this Type type) => GetMethod(type);
    }
    
}