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
    public class ProtaListView<T> : ScrollView
        where T: VisualElement, new()
    {
        Func<int> getLength = null;
        Func<int, T> onCreate = null;
        Action<int, T> onActive = null;
        Action<int, T> onDeactive = null;
        
        List<T> list = new List<T>();
        
        public ProtaListView<T> Update()
        {
            getLength.AssertNotNull();
            onCreate.AssertNotNull();
            onActive.AssertNotNull();
            onDeactive.AssertNotNull();
            
            var len = getLength();
            list.SyncData(len, onCreate, onActive, onDeactive);
            
            return this;
        }
        
        public ProtaListView<T> OnGetLength(Func<int> f)
        {
            getLength = f;
            return this;
        }
        
        public ProtaListView<T> OnCreate(Action<int, T> f)
        {
            onCreate = i => {
                var x = new T();
                f(i, x);
                list.Add(x);
                this.Add(x);
                return x;
            };
            return this;
        }
        
        public ProtaListView<T> OnActiveDdeactive(Action<int, T, bool> f)
        {
            onActive = (i, e) => f(i, e, true);
            onDeactive = (i, e) => f(i, e, false);
            return  this;
        }
        
        
    }
    
}
