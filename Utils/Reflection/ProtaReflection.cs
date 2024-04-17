using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Prota
{
    public static class ProtaReflection
    {
        public static List<Type> GetTypesDerivedFrom(Type baseType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = new ConcurrentBag<Type>();
            Parallel.ForEach(assemblies, assembly => {
                var assemblyTypes = assembly.GetTypes();
                Parallel.ForEach(assemblyTypes, type => {
                    if(baseType.IsAssignableFrom(type) && type != baseType)
                    {
                        types.Add(type);
                    }
                });
            });
            return types.ToList();
        }
        
        public static List<Type> GetTypesDerivedFrom<T>()
        {
            return GetTypesDerivedFrom(typeof(T));
        }
    }
    
}
