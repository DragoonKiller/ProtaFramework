using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    
    public class ProtaTweeningManager : Singleton<ProtaTweeningManager>
    {
        public static long idGen = 0;
        
        public Dictionary<long, TweeningHandle> idMap = new Dictionary<long, TweeningHandle>();
        
        public Dictionary<UnityEngine.Object, BindingList> bindingMap = new Dictionary<UnityEngine.Object, BindingList>(); 
        
        public List<long> toBeRemoved = new List<long>();
        
        public ObjectPool<TweeningHandle> pool = new ObjectPool<TweeningHandle>(() => new TweeningHandle(),
             null,
             v => {
                v = null;
                v.binding = null;
                v.SetGuard(null);
             },
             null
        );
        
        public ObjectPool<BindingList> listPool = new ObjectPool<BindingList>(() => new BindingList()); // TweenType.Count
        
        void Update()
        {
            toBeRemoved.Clear();
            foreach(var (k, v) in idMap)
            {
                if(v.binding != null || v.guard == null || v.callback == null)
                {
                    toBeRemoved.Add(k);
                }
            }
            foreach(var k in toBeRemoved)
            {
                var v = idMap[k];
                if(bindingMap.TryGetValue(v.binding, out var bindingList) && bindingList[v.type] == v)
                {
                    bindingList[v.type] = null;
                    if(bindingList.count == 0)
                    {
                        listPool.Release(bindingList);
                        bindingMap.Remove(v.binding);
                    }
                }
                pool.Release(v);
                idMap.Remove(k);
            }
            toBeRemoved.Clear();
            
            
            foreach(var (k, v) in idMap)
            {
                v.callback(v, v.GetTimeLerp());
            }
        }
        
        public TweeningHandle New(TweeningType type, GameObject binding, ValueTweeningCallback callback)
        {
            Debug.Assert(binding != null);
            
            var v = pool.Get();
            v.id = ++idGen;
            v.binding = binding;
            v.SetGuard(binding);
            v.type = type;
            
            v.callback = callback;
            
            bindingMap.TryGetValue(binding, out var bindingList);
            if(bindingList == null)
            {
                bindingList = listPool.Get();
                bindingMap.Add(binding, bindingList);
            }
            
            var prev = bindingList[type];
            if(prev != null)
            {
                // remove.
                prev.binding = null;
                prev.callback = null;
            }
            
            bindingList[type] = v;
            
            return v;
        }
        
        
        public void Remove(TweeningHandle v)
        {
            if(idMap.ContainsKey(v.id)) v.callback = null;
        }
        
        public void Remove(GameObject binding, TweeningType type)
        {
            if(!bindingMap.TryGetValue(binding, out var list)) return;
            Remove(list[type]);
        }
        
        public void RemoveAll(GameObject binding)
        {
            if(!bindingMap.TryGetValue(binding, out var list)) return;
            foreach(var i in list.bindings) Remove(i);
        }
    }
    
    
}
