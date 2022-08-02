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
        
        public Dictionary<UnityEngine.Object, BindingList> targetMap = new Dictionary<UnityEngine.Object, BindingList>(); 
        
        public List<long> toBeRemoved = new List<long>();
        
        public ObjectPool<TweeningHandle> pool = new ObjectPool<TweeningHandle>(() => new TweeningHandle(),
             null,
             v => {
                v = null;
                v.target = null;
                v.SetGuard(null);
             },
             null
        );
        
        public ObjectPool<BindingList> listPool = new ObjectPool<BindingList>(() => new BindingList()); // TweenType.Count
        
        void Update()
        {
            ActualDeleteAllTagged();
            
            foreach(var (k, v) in idMap)
            {
                v.callback(v, v.GetTimeLerp());
            }
        }
        
        void ActualDeleteAllTagged()
        {
            toBeRemoved.Clear();
            foreach(var (k, v) in idMap)
            {
                if(v.target != null || v.guard == null || v.callback == null)
                {
                    toBeRemoved.Add(k);
                }
            }
            foreach(var k in toBeRemoved)
            {
                var v = idMap[k];
                ActualDelete(k, v);
            }
            toBeRemoved.Clear();
        }
        
        void ActualDelete(long key, TweeningHandle handle)
        {
            Debug.Assert(idMap[key] == handle);
            if(targetMap.TryGetValue(handle.target, out var bindingList) && bindingList[handle.type] == handle)
            {
                bindingList[handle.type] = null;
                if(bindingList.count == 0)
                {
                    listPool.Release(bindingList);
                    targetMap.Remove(handle.target);
                }
            }
            handle.callback = null;
            handle.onFinish?.Invoke(handle);
            handle.onFinish = null;
            pool.Release(handle);
            idMap.Remove(key);
        }
        
        public TweeningHandle New(TweeningType type, UnityEngine.Object target, ValueTweeningCallback callback)
        {
            Debug.Assert(target != null);
            
            var v = pool.Get();
            v.id = ++idGen;
            v.target = target;
            v.SetGuard(target);
            v.type = type;
            
            v.callback = callback;
            
            targetMap.TryGetValue(target, out var bindingList);
            if(bindingList == null)
            {
                bindingList = listPool.Get();
                targetMap.Add(target, bindingList);
            }
            
            var prev = bindingList[type];
            Remove(prev);
            bindingList[type] = v;
            
            return v;
        }
        
        
        public bool Remove(TweeningHandle v)
        {
            if(v == null) return false;
            if(!idMap.ContainsKey(v.id)) return false;
            v.callback = null;
            return true;
        }
        
        public bool Remove(UnityEngine.Object target, TweeningType type)
        {
            if(!targetMap.TryGetValue(target, out var list)) return false;
            return Remove(list[type]);
        }
        
        public bool RemoveAll(UnityEngine.Object target)
        {
            if(!targetMap.TryGetValue(target, out var list)) return false;
            var res = false;
            foreach(var i in list.bindings) res |= Remove(i);
            return res;
        }
    }
    
    
}
