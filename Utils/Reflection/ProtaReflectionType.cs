using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Prota
{
    public class ProtaReflectionType
    {
        const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.NonPublic
            | BindingFlags.Instance | BindingFlags.Static
            | BindingFlags.FlattenHierarchy;
        
        public readonly Type type;
        
        ProtaReflectionType() { }
        
        public ProtaReflectionType(Type type)
        {
            this.type = type;
        }
        
        public bool HasBackingFieldOfProperty(string name)
        {
            if(!HasProperty(name)) throw new ProtaReflectionFailException($"property {name} not found");
            var backingFieldName = $"<{name}>k__BackingField";
            return HasField(backingFieldName);
        }
        
        public FieldInfo GetBackingFieldOfProperty(string name)
        {
            if(!HasProperty(name)) throw new ProtaReflectionFailException($"property {name} not found");
            var backingFieldName = $"<{name}>k__BackingField";
            return GetField(backingFieldName);
        }
        
        public static string BackingFieldNameOfProperty(string name) => $"<{name}>k__BackingField";
        
        public Attribute[] attributes => type.GetCustomAttributes(true).Cast<Attribute>().ToArray();
        
        public MemberInfo[] allMembers => type.GetMembers(BindingAttr);
        
        public PropertyInfo[] allProperties => type.GetProperties(BindingAttr);
        
        public FieldInfo[] allFields => type.GetFields(BindingAttr);
        
        public MethodInfo[] allMethods => type.GetMethods(BindingAttr);
        
        public MemberInfo[] allPublicMembers => allFields.Where(f => f.IsPublic).ToArray();
        
        public PropertyInfo[] allPublicProperties => allProperties.Where(p => p.GetGetMethod(true).IsPublic).ToArray();
        
        public FieldInfo[] allPublicFields => allFields.Where(f => f.IsPublic).ToArray();
        
        public MethodInfo[] allPublicMethods => allMethods.Where(m => m.IsPublic).ToArray();
        
        public MemberInfo[] allPrivateMembers => allFields.Where(f => f.IsPrivate).ToArray();
        
        public PropertyInfo[] allPrivateProperties => allProperties.Where(p => p.GetGetMethod(true).IsPrivate).ToArray();
        
        public FieldInfo[] allPrivateFields => allFields.Where(f => f.IsPrivate).ToArray();
        
        public MethodInfo[] allPrivateMethods => allMethods.Where(m => m.IsPrivate).ToArray();
        
        public string code => GetCSharpRepresentation(type);
        
        public string name => type.Name;
        
        public string fullName => type.FullName;
        
        public bool isStruct => type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        
        public bool isPrimitive => type.IsPrimitive;
        
        public bool isValueType => type.IsValueType;
        
        public bool isEnum => type.IsEnum;
        
        public bool isClass => type.IsClass;
        
        public bool isInterface => type.IsInterface;
        
        public bool isAbstract => type.IsAbstract;
        
        public bool isSealed => type.IsSealed;
        
        public bool isGenericType => type.IsGenericType;
        
        public bool isGenericTypeDefinition => type.IsGenericTypeDefinition;
        
        public bool isGenericParameter => type.IsGenericParameter;
        
        public bool isByRef => type.IsByRef;
        
        public bool isArray => type.IsArray;
        
        public bool isPointer => type.IsPointer;
        
        public bool isNested => type.IsNested;
        
        public bool isNestedPublic => type.IsNestedPublic;
        
        public bool isNestedPrivate => type.IsNestedPrivate;
        
        public bool isNestedFamily => type.IsNestedFamily;
        
        public bool isNestedAssembly => type.IsNestedAssembly;
        
        public bool isNestedFamANDAssem => type.IsNestedFamANDAssem;
        
        public bool isNestedFamORAssem => type.IsNestedFamORAssem;
        
        public bool isAutoLayout => type.IsAutoLayout;
        
        public bool isExplicitLayout => type.IsExplicitLayout;
        
        public bool isLayoutSequential => type.IsLayoutSequential;
        
        public bool isImport => type.IsImport;
        
        public bool isSerializable => type.IsSerializable;
        
        public bool isAnsiClass => type.IsAnsiClass;
        
        public Assembly assembly => type.Assembly;
        
        public Type baseType => type.BaseType;
        
        
        public object CreateObject(params object[] args) => Activator.CreateInstance(type, args);
        
        public bool ImplementsInterface(Type interfaceType) => interfaceType.IsAssignableFrom(type);
        
        public bool ImplementsInterface<T>() => typeof(T).IsAssignableFrom(type);
        
        public Type GetNestedType(string name) => AssertThis("Nested Type {0} not found", name, type.GetNestedType(name, BindingAttr));
        
        public PropertyInfo GetProperty(string name) => AssertThis("Property {0} not found", name, type.GetProperty(name, BindingAttr));
        
        public PropertyInfo GetIndexerProperty(params Type[] args) => AssertThis("Indexer Property {0} not found", args.ToStrings().Join(","), type.GetProperty("Item", args));
        
        public FieldInfo GetField(string name) => AssertThis("Field {0} not found", name, type.GetField(name, BindingAttr));
        
        public MethodInfo GetGenericMethod(string name, Type[] typeArgs, Type[] parameterTypes)
            => AssertThis("Generic Method {0} not found", name, type.GetMethod(name, typeArgs.Length, BindingAttr, null, parameterTypes, null));
        
        public MethodInfo GetGenericMethodInstantiated(string name, Type[] typeArgs, Type[] parameterTypes)
            => AssertThis("Generic Method {0} not found or unable to be created", name, type.GetMethod(name, typeArgs.Length, BindingAttr, null, parameterTypes, null).MakeGenericMethod(typeArgs));
        
        public MethodInfo GetMethod(string name, params Type[] args)
        {
            if(args == null || args.Length == 0)
            {
                try
                {
                    // method is not overloaded, this can be successful.
                    return type.GetMethod(name, BindingAttr);
                }
                catch(AmbiguousMatchException)
                {
                    // method is overloaded, find the overloaded version.
                    var x = type.GetMethod(name, BindingAttr, null, args, null);
                    AssertThis("Method {0} not found, or you must specify an overload", name, x);
                    return null;
                }
            }
            else
            {
                return AssertThis("Method {0} not found with type args", name, type.GetMethod(name, BindingAttr, null, args, null));
            }
        }
        
        public bool HasMethod(string name, params Type[] args)
        {
            if(args == null || args.Length == 0)
            {
                try
                {
                    // method is not overloaded, this can be successful.
                    return type.GetMethod(name, BindingAttr) != null;
                }
                catch(AmbiguousMatchException)
                {
                    // method is overloaded, find the overloaded version.
                    return type.GetMethod(name, BindingAttr, null, args, null) != null;
                }
            }
            else
            {
                return type.GetMethod(name, BindingAttr, null, args, null) != null;
            }
        }
        
        public bool IsMethodOverloaded(string name) => type.GetMethods(BindingAttr).Count(m => m.Name == name) > 1;
        
        public bool IsMethodAbstract(string name, params Type[] args) => GetMethod(name, args).IsAbstract;
        
        public bool IsMethodVirtual(string name, params Type[] args) => GetMethod(name, args).IsVirtual;
        
        public MemberInfo GetMember(string name) => AssertThis("Member {0} not found", name, type.GetMember(name, BindingAttr).FirstOrDefault());
        
        public bool HasProperty(string name) => type.GetProperty(name, BindingAttr) != null;
        
        public bool HasField(string name) => type.GetField(name, BindingAttr) != null;
        
        public bool HasMember(string name) => type.GetMember(name, BindingAttr).Length > 0;
        
        public bool HasIndexerProperty(params Type[] args) => type.GetProperty("Item", args) != null;
        
        public bool IsStatic(string name, params Type[] args)
        {
            if(HasProperty(name)) return GetProperty(name).GetGetMethod(true).IsStatic;
            if(HasField(name)) return GetField(name).IsStatic;
            if(HasMethod(name, args)) return GetMethod(name, args).IsStatic;
            throw new ProtaReflectionFailException($"Member { name } ({ args.ToStrings().Join(",") }) not found");
        }
        
        
        public bool IsPrivate(string name, params Type[] args)
        {
            if(HasProperty(name)) return GetProperty(name).GetGetMethod(true).IsPrivate;
            if(HasField(name)) return GetField(name).IsPrivate;
            if(HasMethod(name, args)) return GetMethod(name, args).IsPrivate;
            throw new ProtaReflectionFailException($"Member { name } ({ args.ToStrings().Join(",") }) not found");
        }
        
        public bool IsProtected(string name, params Type[] args)
        {
            if(HasProperty(name)) return GetProperty(name).GetGetMethod(true).IsFamily;
            if(HasField(name)) return GetField(name).IsFamily;
            if(HasMethod(name, args)) return GetMethod(name, args).IsFamily;
            throw new ProtaReflectionFailException($"Member { name } ({ args.ToStrings().Join(",") }) not found");
        }
        
        
        public bool IsPublic(string name, params Type[] args)
        {
            if(HasProperty(name)) return GetProperty(name).GetGetMethod(true).IsPublic;
            if(HasField(name)) return GetField(name).IsPublic;
            if(HasMethod(name, args)) return GetMethod(name, args).IsPublic;
            throw new ProtaReflectionFailException($"Member { name } ({ args.ToStrings().Join(",") }) not found");
        }
        
        public Attribute GetTypeAttribute(Type type) => attributes.FirstOrDefault(a => a.GetType() == type);
        
        public T GetTypeAttribute<T>() where T: Attribute => GetTypeAttribute(typeof(T)) as T;
        
        
        public Attribute[] GetAllAttributes(string name)
        {
            if(HasProperty(name)) GetProperty(name).GetCustomAttributes().ToArray();
            if(HasField(name)) GetField(name).GetCustomAttributes().ToArray();
            if(HasMethod(name)) GetMethod(name).GetCustomAttributes().ToArray();
            throw new ProtaReflectionFailException($"Member { name } not found");
        }
        
        
        public bool IsConstant(string name)
        {
            if(HasField(name)) return GetField(name).IsLiteral;
            throw new ProtaReflectionFailException($"Field { name } not found");
        }
        
        
        public Attribute GetAttribute<T>(string name, params Type[] args) where T : Attribute
        {
            if(HasProperty(name)) return GetProperty(name).GetCustomAttribute<T>();
            if(HasField(name)) return GetField(name).GetCustomAttribute<T>();
            if(HasMethod(name, args)) return GetMethod(name, args).GetCustomAttribute<T>();
            throw new ProtaReflectionFailException($"Member { name } ({ args.ToStrings().Join(",") }) not found");
        }
        
        public object Call(string name, params object[] args)
        {
            var method = GetMethod(name, args.Select(x => x.GetType()).ToArray());
            if (method == null) return null;
            if(!method.IsStatic) throw new ProtaReflectionFailException("You can only call a static method by ProtaReflectionType");
            return method.Invoke(null, args);
        }
        
        public void Set(string name, object value)
        {
            if(HasProperty(name))
            {
                var property = GetProperty(name);
                property.SetValue(null, value);
                return;
            }
            
            if(HasField(name))
            {
                var field = GetField(name);
                field.SetValue(null, value);
                return;
            }
            
            throw new ProtaReflectionFailException($"Member { name } not found");
        }
        
        public T Get<T>(string name) => (T)Get(name);
        
        public object Get(string name)
        {
            if(HasProperty(name))
            {
                var property = GetProperty(name);
                return property.GetValue(null);
            }
            
            if(HasField(name))
            {
                var field = GetField(name);
                return field.GetValue(null);
            }
            
            throw new ProtaReflectionFailException($"Member { name } not found");
        }
        
        T AssertThis<T>(string format, string acquire, T value) where T: class
        {
            if(value == null)
            {
                var message = string.Format(format, acquire);
                throw new ProtaReflectionFailException(message);
            }
            return value;
        }
        
        // ====================================================================================================
        // https://stackoverflow.com/questions/2579734/get-the-type-name
        // ====================================================================================================
        
        static string GetCSharpRepresentation(Type t)
        {
            return GetCSharpRepresentation(t, new Queue<Type>(t.GetGenericArguments()));
        }
        
        static string GetCSharpRepresentation(Type t, Queue<Type> availableArguments)
        {
            string value = t.Name;
            if (t.IsGenericParameter)
            {
                return value;
            }
            if (t.DeclaringType != null)
            {
                // This is a nested type, build the parent type first
                value = GetCSharpRepresentation(t.DeclaringType, availableArguments) + "." + value;
            }
            if (t.IsGenericType)
            {
                value = value.Split('`')[0];

                // Build the type arguments (if any)
                string argString = "";
                var thisTypeArgs = t.GetGenericArguments();
                for (int i = 0; i < thisTypeArgs.Length && availableArguments.Count > 0; i++)
                {
                    if (i != 0) argString += ",";

                    argString += GetCSharpRepresentation(availableArguments.Dequeue());
                }

                // If there are type arguments, add them with < >
                if (argString.Length > 0)
                {
                    value += "<" + argString + ">";
                }
            }

            return value;
        }
    }
    
}
