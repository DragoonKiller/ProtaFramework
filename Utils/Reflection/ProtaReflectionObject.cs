using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
                else property.SetValue(target, value);
                return;
            }
            
            if(type.HasField(name))
            {
                var field = type.GetField(name);
                if(field.IsStatic) field.SetValue(null, value);
                else if(field.IsLiteral) throw new ProtaReflectionFailException("cannot set constant.");
                else field.SetValue(target, value);
                return;
            }
            
            throw new ProtaReflectionFailException($"member {name} not found");
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
                else property.SetValue(target, value, index);
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
        
        
    }
    
}
