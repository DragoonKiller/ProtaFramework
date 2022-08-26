using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

using Prota;
using Prota.Unity;
using System.Collections.Generic;

namespace Prota.Editor
{
    public class ProtaCollectionView<K, V, T> : ScrollView
        where T: VisualElement, new()
    {
        GetKVEnumerableFunc<K, V> getEnumerable = null;
        TryGetValueFunc<K, V> tryGetValue = null;
        Func<K, V, T> onCreate = null;
        Action<K, V, T> onUpdate = null;
        Action<K, T> onRemove = null;
        
        Dictionary<K, T> collection = new Dictionary<K, T>();
        
        public ProtaCollectionView<K, V, T> Update()
        {
            getEnumerable.AssertNotNull();
            tryGetValue.AssertNotNull();
            onCreate.AssertNotNull();
            onUpdate.AssertNotNull();
            onRemove.AssertNotNull();
            
            collection.SetSync<K, T, V, Dictionary<K, T>>(getEnumerable, tryGetValue, onCreate, onUpdate, onRemove);
            return this;
        }
        
        public ProtaCollectionView<K, V, T> OnGetEnumerable(GetKVEnumerableFunc<K, V> f)
        {
            this.getEnumerable = f;
            return this;
        }
        
        public ProtaCollectionView<K, V, T> OnGetValue(TryGetValueFunc<K, V> f)
        {
            this.tryGetValue = f;
            return this;
        }
        
        public ProtaCollectionView<K, V, T> OnCreate(Action<K, V, T> f)
        {
            onCreate = (k, v) => {
                var x = new T();
                f(k, v, x);
                collection.Add(k, x);
                this.Add(x);
                return x;
            };
            return this;
        }
        
        public ProtaCollectionView<K, V, T> OnUpdate(Action<K, V, T> f)
        {
            onUpdate = f;
            return this;
        }
        
        
        public ProtaCollectionView<K, V, T> OnRemove(Action<K, T> f)
        {
            onRemove = f;
            return this;
        }
        
        
        
    }
    
}