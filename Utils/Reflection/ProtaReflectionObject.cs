using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;
using UnityEditor.Build.Content;

namespace Prota
{
    // 一个对任意对象的包装, 用来获取对象的属性/字段值, 以及设置属性/字段值.
    public struct ProtaReflectionObject
    {
        const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        public readonly object target;
        public ProtaReflectionType type => new ProtaReflectionType(target.GetType());
        
        public Type rawType => target.GetType();
        
        public ProtaReflectionObject(object target)
        {
            this.target = target;
        }
        
        public object this[string name]
        {
            get => Get(name);
            set => Set(name, value);
        }
        
        public T As<T>() => (T)target;
        
        public T Get<T>(string name) => (T)Get(name);
        
        public object Get(string name)
        {
            if(type.HasProperty(name)) return type.GetProperty(name).GetValue(target);
            if(type.HasField(name)) return type.GetField(name).GetValue(target);
            throw new Exception($"member {name} not found");
        }
        
        public bool TryGet<T>(string name, out T value)
        {
            try
            {
                value = Get<T>(name);
                return true;
            }
            catch(ProtaReflectionFailException)
            {
                value = default;
                return false;
            }
        }
        
        public bool TryGet(string name, out object value)
        {
            try
            {
                value = Get(name);
                return true;
            }
            catch(ProtaReflectionFailException)
            {
                value = null;
                return false;
            }
        }
        
        public bool TryGetAs<T>(string name, out object value)
        {
            try
            {
                GetAs<T>(name, out value);
                return true;
            }
            catch(ProtaReflectionFailException)
            {
                value = null;
                return false;
            }
        }
        
        public void GetAs<T>(string name, out object value)
        {
            var getType = typeof(T).ProtaReflection();
            if(getType.HasProperty(name))
            {
                var property = getType.GetProperty(name);
                if(property.GetGetMethod(true).IsStatic) value = property.GetValue(null);
                else value = property.GetValue(target);
                return;
            }
            
            if(getType.HasField(name))
            {
                var field = getType.GetField(name);
                if(field.IsStatic) value = field.GetValue(null);
                else value = field.GetValue(target);
                return;
            }
            
            throw new ProtaReflectionFailException($"member {name} as type { nameof(T) } not found");
        }
        
        public bool TrySet(string name, object value)
        {
            try
            {
                Set(name, value);
                return true;
            }
            catch(ProtaReflectionFailException) { return false; }
        }
        
        public void Set(string name, object value)
        {
            if(type.HasProperty(name))
            {
                var property = type.GetProperty(name);
                if(property.GetSetMethod(true).IsStatic) property.SetValue(null, value);
                else property.SetValue(target, DynamicCast(property.PropertyType, value));
                return;
            }
            
            if(type.HasField(name))
            {
                var field = type.GetField(name);
                if(field.IsStatic) field.SetValue(null, value);
                else if(field.IsLiteral) throw new ProtaReflectionFailException("cannot set constant.");
                else field.SetValue(target, DynamicCast(field.FieldType, value));
                return;
            }
            
            throw new ProtaReflectionFailException($"member {name} not found");
        }
        
        
        public void TrySetAs<T>(string name, object value)
        {
            try
            {
                SetAs<T>(name, value);
            }
            catch(ProtaReflectionFailException) { }
        }
        
        public void SetAs<T>(string name, object value)
        {
            var setType = typeof(T).ProtaReflection();
            if(setType.HasProperty(name))
            {
                var property = setType.GetProperty(name);
                if(property.GetSetMethod(true).IsStatic) property.SetValue(null, value);
                else property.SetValue(target, DynamicCast(property.PropertyType, value));
                return;
            }
            
            if(setType.HasField(name))
            {
                var field = setType.GetField(name);
                if(field.IsStatic) field.SetValue(null, value);
                else if(field.IsLiteral) throw new ProtaReflectionFailException("cannot set constant.");
                else field.SetValue(target, DynamicCast(field.FieldType, value));
                return;
            }
            
            throw new ProtaReflectionFailException($"member {name} as type { nameof(T) } not found");
        }
        
        public object GetIndexer(params object[] index)
        {
            var argTypes = index.Select(x => x.GetType()).ToArray();
            if(type.HasIndexerProperty(argTypes))
            {
                var property = type.GetIndexerProperty(argTypes);
                if(property.GetGetMethod(true).IsStatic) return property.GetValue(null, index);
                else return property.GetValue(target, index);
            }
            
            throw new ProtaReflectionFailException($"specific indexer [{ argTypes.ToStrings().Join(",") }] not found");
        }
        
        public void SetIndexer(object value, params object[] index)
        {
            var argTypes = index.Select(x => x.GetType()).ToArray();
            if(type.HasIndexerProperty(argTypes))
            {
                var property = type.GetIndexerProperty(argTypes);
                if(property.GetSetMethod(true).IsStatic) property.SetValue(null, value, index);
                else property.SetValue(target, DynamicCast(property.PropertyType, value), index);
                return;
            }
            
            throw new Exception($"specific indexer [{ argTypes.ToStrings().Join(",") }] not found");
        }
        
        
        public object Call(string name, params object[] args)
        {
            var argTypes = args.Select(x => x.GetType()).ToArray();
            if(type.HasMethod(name, argTypes))
            {
                var method = type.GetMethod(name, argTypes);
                if(method.IsStatic) return method.Invoke(null, args);
                else return method.Invoke(target, args);
            }
            
            throw new Exception($"specific method [{ name }({ argTypes.ToStrings().Join(",") })] not found");
        }
        
        public string AllPropertiesAndValuesToString()
        {
            var all = this.type.allFields;
            var sb = new StringBuilder();
            foreach(var f in all)
            {
                var value = f.GetValue(target);
                sb.AppendLine($"[{f.Name}]{value}");
            }
            return sb.ToString();
        }
        
        
        // https://stackoverflow.com/questions/18369681/using-setvalue-with-implicit-conversion
        object DynamicCast(Type fieldType, object value)
        {
            object valueToSet = null;

            if (value == null  || value == DBNull.Value)
            {
            valueToSet = null;
            }
            else
            {
                // assign enum
                if (fieldType.IsEnum) valueToSet = Enum.ToObject(fieldType, value);
                
                // support for nullable enum types
                else if (fieldType.IsValueType)
                {
                    Type underlyingType = Nullable.GetUnderlyingType(fieldType);
                    if(underlyingType != null)
                    {
                        valueToSet = underlyingType.IsEnum ? Enum.ToObject(underlyingType, value) : value;
                    }
                }
                else
                {
                    //we always need ChangeType, it will convert the value to the proper number type, for example.
                    valueToSet = Convert.ChangeType(value, fieldType);
                }
            }
            
            return valueToSet;
        }
    }
    
}
