
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

namespace Prota
{


    
    // class / (object)struct => IDictionary<string, object>
    public class ClassAdapter : IDictionary<string, object>
    {
        object d;
        Type type;
        ProtaReflection pref;
        public ClassAdapter(object x)
        {
            d = x;
            type = d.GetType();
            pref = new ProtaReflection(d);
        }

        public object this[string key]
        {
            get
            {
                if(pref.TryGet(key, out object val)) return val;
                return null;
            }
            
            set
            {
                pref.TrySet(key, value);
            }
        }

        public ICollection<string> Keys => type.GetFields(ProtaReflection.flags).Select(x => x.Name)
            .Concat(type.GetProperties(ProtaReflection.flags).Select(x => x.Name))
            .ToArray();
        
        public IReadOnlyList<string> fields => type.GetFields(ProtaReflection.flags).Select(x => x.Name).ToArray();
        
        public IReadOnlyList<string> properties => type.GetFields(ProtaReflection.flags).Select(x => x.Name).ToArray();
        
        public IReadOnlyList<string> instanceFields => type.GetFields(ProtaReflection.flags & ~(BindingFlags.Static)).Select(x => x.Name).ToArray();
        
        public IReadOnlyList<string> instanceProperties => type.GetFields(ProtaReflection.flags & ~(BindingFlags.Static)).Select(x => x.Name).ToArray();
        
        public object rawValue => d;
        
        public T GetAs<T>() => (T)d;
        
        public ICollection<object> Values => throw new NotSupportedException();

        public int Count
        {
            get
            {
                if(pref.TryCount(out int count)) return count;
                return 0;
            }
        }

        public bool IsReadOnly => true;

        public void Add(string key, object value) => throw new NotSupportedException();

        public void Add(KeyValuePair<string, object> item) => throw new NotSupportedException();
        public void Clear() { }

        public bool Contains(KeyValuePair<string, object> item) => throw new NotSupportedException();

        public bool ContainsKey(string key) => pref.TryGet(key, out _);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach(var key in Keys)
            {
                yield return new KeyValuePair<string, object>(key, this[key]);
            }
        }

        public bool Remove(string key) => throw new NotSupportedException();

        public bool Remove(KeyValuePair<string, object> item) => throw new NotSupportedException();

        public bool TryGetValue(string key, out object value) => pref.TryGet(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public static void UnitTest()
        {
            #if UNITY_2017_1_OR_NEWER
            var obj = new Vector3() { x = 1, y = 2, z = 3 };
            var pref = new ProtaReflection(obj);
            var col = new ClassAdapter(obj);
            Debug.Log(string.Join(",", col.Keys));
            Debug.Log(string.Join(",", col.fields));
            Debug.Log(string.Join(",", col.instanceFields));
            Debug.Log(string.Join(",", col.properties));
            Debug.Log(string.Join(",", col.instanceProperties));
            
            Debug.Log(col["x"].GetType().FullName + " " + col["x"]);
            Debug.Log(col["y"].GetType().FullName + " " + col["y"]);
            Debug.Log(col["z"].GetType().FullName + " " + col["z"]);
            
            col["x"] = 12;
            Debug.Log(col["x"]);
            Debug.Log(obj);         // { 1, 2, 3 }
            Debug.Log((Vector3)col.rawValue);
            Debug.Log(col.GetAs<Vector3>());
            #endif
        }
        
    }

}