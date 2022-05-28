using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Prota
{
    public struct ProtaReflection
    {
        static object[] emptyArgs = new object[0];
        
        Type type;
        
        object target;
        
        public const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        
        public ProtaReflection(Type type)
        {
            this.type = type;
            this.target = null;
        }
        
        public ProtaReflection(object target)
        {
            this.target = target;
            this.type = target.GetType();
        }
        
        public ProtaReflection(object target, Type type)
        {
            this.target = target;
            this.type = type;
            if(target.GetType() != type) throw new Exception();
        }
        
        public static ProtaReflection Get<T>() => new ProtaReflection(typeof(T));
        
        public MethodInfo Method(string name) => this.type.GetMethod(name, flags);
        
        public MethodInfo[] Methods() => this.type.GetMethods(flags);
        
        public MethodInfo[] AllMethod(string name) => this.type.GetMethods();
        
        public Type NestedType(string name) => this.type.GetNestedType(name);
        
        public bool isGeneric => this.type.IsConstructedGenericType;
        
        public bool isGenericTemplate => this.type.IsGenericTypeDefinition;
        
        public ProtaReflection WithTarget(object target)
        {
            this.target = target;
            if(target.GetType() != type) throw new Exception();
            return this;
        }
        
        // ========================================================================================
        // 类型信息
        // ========================================================================================
        
        public bool TryGetGenericDefinition(out Type typedef)
        {
            typedef = null;
            if(!isGeneric) return false;
            typedef = type.GetGenericTypeDefinition();
            return true; 
        }
        
        public bool TryGetGenericArgs(out Type[] args)
        {
            args = null;
            if(!isGeneric) return false;
            if(isGenericTemplate) return false;
            args = type.GetGenericArguments();
            return true;
        }
        
        public bool TryCall(string name, out object ret, params object[] args)
        {
            ret = null;
            var method = Method(name);
            if(method == null) return false;
            if(method.IsStatic)
            {
                ret = method.Invoke(null, args);
            }
            else
            {
                ret = method.Invoke(target, args);
            }
            return true;
        }
        
        public object Call(string name, params object[] args)
        {
            var method = Methods().Where(x => {
                var p = x.GetParameters();
                if(p.Length != args.Length) return false;       // 参数个数不一致.
                bool match = true;
                for(int i = 0; i < p.Length; i++)
                {
                    var arg = args[i];
                    if(arg == null && p[i].ParameterType.IsClass) continue;     // 没有类型判别, 什么类型都可以.
                    if(p[i].IsOut) continue;                                    // out 变量填 null 就行.
                    if(p[i].ParameterType != arg.GetType())                     // 类型不匹配.
                    {
                        match = false;
                        break;
                    }
                }
                return match;
            }).FirstOrDefault();
            
            if(method == null) throw new Exception($"Method { name } not found in { type.ToString() }");
            if(method.IsStatic)
            {
                return method.Invoke(null, args);
            }
            else
            {
                return method.Invoke(target, args);
            }
            
        }
        
        public bool ImplInterface<T>() => ImplInterface(typeof(T));
        
        public bool ImplInterface(Type t) => this.type.GetInterfaces().Any(x => x == t);
        
        public bool ImplGenericInterface(Type t, out Type[] args)
        {
            var res = this.type
                .GetInterfaces()
                .Where(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == t)
                .FirstOrDefault();
                
            args = null;
            if(res == null) return false;
            
            args = res.GetGenericArguments();
            return true;
        }
        
        public bool IsSubOf<T>() => IsSubOf(typeof(T));
        
        public bool IsSubOf(Type t) => t.IsAssignableFrom(type);
        
        public bool IsGenericOf(Type t)
            => (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == t)
            || (type.GetInterfaces().Any(x => x.IsConstructedGenericType && x.GetGenericTypeDefinition() == t));
        
        // ========================================================================================
        // 成员对象和数据
        // ========================================================================================
        
        public bool TryGet(string name, out object val)
        {
            var field = type.GetField(name, flags);
            if(field != null)
            {
                val = field.GetValue(target);
                return true;
            }
            
            var prop = type.GetProperty(name, flags);
            if(prop != null && prop.GetMethod != null)
            {
                val = prop.GetValue(target);
                return true;
            }
            
            val = null;
            return false;
        }
        
        public bool TrySet(string name, object val)
        {
            var field = type.GetField(name, flags);
            if(field != null)
            {
                field.SetValue(target, val);
                return true;
            }
            
            var prop = type.GetProperty(name, flags);
            if(prop != null && prop.SetMethod != null)
            {
                prop.SetValue(target, val);
                return true;
            }
            
            return false;
        }
        
        public string[] AllFieldNames()
        {
            return type.GetFields(flags).Select(x => x.Name).ToArray();
        }
        
        public bool TryGetIndex(params object[] args)
        {
            var prop = type.GetProperty("Item", flags);
            if(prop != null)
            {
                prop.GetValue(target, args);
                return true;
            }
            
            return false;
        }
        
        public bool TrySetIndex(object val, params object[] args)
        {
            var prop = type.GetProperty("Item", flags);
            if(prop != null)
            {
                prop.SetValue(target, val, args);
                return true;
            }
            
            return false;
        }
        
        
        
        // ========================================================================================
        // 一些不泛用的函数
        // ========================================================================================
        // 主要有两种类型, 一种是 Array, 一种是 k-v map.
        
        
        // 支持: 各种 Array 和 List. 只要有 Count 和 Length 的就行.
        public bool TryCount(out int count)
        {
            // 所有 t[], Array.
            if(target is Array array)
            {
                count = array.Length;
                return true;
            }
            
            // IList, IDictionary, IReadonlyList, IReadonlyDictionary.
            if(target is ICollection collection)
            {
                count = collection.Count;
                return true;
            }
            
            count = -1;
            return false;
        }
        
        // 支持所有 IEnumerable 和 IEnumerable<T>
        public bool TryEnumerate(out IEnumerable<(object key, object value)> e)
        {
            // 所有 t[], Array
            if(target is Array array)
            {
                IEnumerable<(object key, object value)> F()
                {
                    for(int i = 0; i < array.Length; i++) yield return (i, array.GetValue(i));
                }
                e = F();
                return true;
            }
            
            // 所有 List<T>
            if(target is IList list)
            {
                IEnumerable<(object key, object value)> F()
                {
                    for(int i = 0; i < list.Count; i++) yield return (i, list[i]);
                }
                e = F();
                return true;
            }
            
            // Dictionary<K, V> SortedList<K, V> 等.
            // 注: 自定义的结构如果想要序列化, 推荐定义 IDictionary 这个接口.
            if(target is IDictionary dict)
            {
                IEnumerable<(object key, object value)> F()
                {
                    foreach(var key in dict.Keys) yield return (key, dict[key]);
                }
                e = F();
                return true;
            }
            
            // HashSet<T>, HashSet, ICollection. 没有 key, 直接用 value 来表示.
            if(target is ICollection collection)
            {
                IEnumerable<(object key, object value)> F()
                {
                    foreach(var value in collection) yield return (value, value);
                }
                e = F();
                return true;
            }
            
            e = null;
            return false;
        }
    }
    
}