using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    
    public class ProtaTweeningManager : Singleton<ProtaTweeningManager>
    {
        internal ArrayLinkedList<TweenData> data = new ArrayLinkedList<TweenData>();
        
        public Dictionary<UnityEngine.Object, BindingList> targetMap = new Dictionary<UnityEngine.Object, BindingList>(); 
        
        public List<ArrayLinkedListKey> toBeRemoved = new List<ArrayLinkedListKey>();
        
        public ObjectPool<BindingList> listPool = new ObjectPool<BindingList>(() => new BindingList()); // TweenType.Count
        
        void Update()
        {
            ActualDeleteAllTagged();
            
            foreach(var i in data.EnumerateIndex())
            {
                var v = data[i];
                v.update(new TweenHandle(i, data), v.GetTimeLerp());
            }
        }
        
        void ActualDeleteAllTagged()
        {
            toBeRemoved.Clear();
            foreach(var key in data.EnumerateIndex())
            {
                var v = data[key];
                if(v.invalid || v.isTimeout) toBeRemoved.Add(key);
            }
            
            foreach(var key in data.EnumerateIndex())
            {
                var v = data[key];
                if(v.isTimeout && v.valid) v.update(new TweenHandle(key, data), 1);
            }
            
            foreach(var key in toBeRemoved)
            {
                ActualDelete(key);
            }
            
            toBeRemoved.Clear();
        }
        
        void ActualDelete(ArrayLinkedListKey key)
        {
            var d = data[key];
            if(targetMap.TryGetValue(d.target, out var bindingList) && bindingList[d.type].key == key)
            {
                bindingList[d.type] = TweenHandle.none;
                if(bindingList.count == 0)
                {
                    listPool.Release(bindingList);
                    targetMap.Remove(d.target);
                }
            }
            data.Release(key);
        }
        
        public TweenHandle New(TweeningType type, UnityEngine.Object target, ValueTweeningUpdate onUpdate)
        {
            Debug.Assert(target != null);
            
            var id = data.Take();
            ref TweenData v = ref data[id];
            v.target = target;
            v.guard = target;
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
            
            var newHandle = new TweenHandle(id, data);
            bindingList[type] = newHandle;
            return newHandle;
        }
        
        
        public bool Remove(TweenHandle v)
        {
            if(v.isNone) return false;
            if(!data.Valid(v.key)) return false;
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
