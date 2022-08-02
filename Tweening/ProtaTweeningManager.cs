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
        
        public Dictionary<long, TweenHandle> idMap = new Dictionary<long, TweenHandle>();
        
        public Dictionary<UnityEngine.Object, BindingList> targetMap = new Dictionary<UnityEngine.Object, BindingList>(); 
        
        public List<(long, TweenHandle)> toBeRemoved = new List<(long, TweenHandle)>();
        
        public ObjectPool<TweenHandle> pool = new ObjectPool<TweenHandle>(() => new TweenHandle(),
             null,
             v => {
                v.customData = null;
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
                v.update(v, v.GetTimeLerp());
            }
        }
        
        void ActualDeleteAllTagged()
        {
            toBeRemoved.Clear();
            foreach(var (k, v) in idMap)
            {
                if(v.target == null || v.guard == null || v.update == null || v.isTimeout)
                {
                    toBeRemoved.Add((k, v));
                }
            }
            
            foreach(var (k, v) in toBeRemoved)
            {
                if(v.isTimeout && !(v.target == null || v.guard == null || v.update == null))
                {
                    v.update(v, 1);
                }
            }
            
            foreach(var (k, v) in toBeRemoved)
            {
                ActualDelete(k, v);
            }
            
            toBeRemoved.Clear();
        }
        
        void ActualDelete(long key, TweenHandle handle)
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
            handle.update = null;
            handle.onFinish?.Invoke(handle);
            handle.onFinish = null;
            pool.Release(handle);
            idMap.Remove(key);
        }
        
        public TweenHandle New(TweeningType type, UnityEngine.Object target, ValueTweeningUpdate onUpdate)
        {
            Debug.Assert(target != null);
            
            var v = pool.Get();
            v.id = ++idGen;
            idMap.Add(v.id, v);
            
            v.target = target;
            v.SetGuard(target);
            v.type = type;
            
            v.update = onUpdate;
            
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
        
        
        public bool Remove(TweenHandle v)
        {
            if(v == null) return false;
            if(!idMap.ContainsKey(v.id)) return false;
            v.update = null;
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
