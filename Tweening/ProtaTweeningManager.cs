using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tween
{
    
    public class ProtaTweenManager : Singleton<ProtaTweenManager>
    {
        public readonly ArrayLinkedList<TweenData> data = new ArrayLinkedList<TweenData>();
        
        public readonly Dictionary<UnityEngine.Object, BindingList> targetMap = new Dictionary<UnityEngine.Object, BindingList>(); 
        
        public readonly List<ArrayLinkedListKey> toBeRemoved = new List<ArrayLinkedListKey>();
        
        public readonly ObjectPool<BindingList> listPool = new ObjectPool<BindingList>(() => new BindingList()); // TweenId.Count
        
        void Update()
        {
            ActualDeleteAllTagged();
            
            foreach(var key in data.EnumerateKey())
            {
                ref var v = ref data[key];
                v.update(v.handle, v.GetTimeLerp());
            }
        }
        
        void ActualDeleteAllTagged()
        {
            toBeRemoved.Clear();
            foreach(var key in data.EnumerateKey())
            {
                ref var v = ref data[key];
                if(v.invalid || v.isTimeout) toBeRemoved.Add(key);
            }
            
            foreach(var key in toBeRemoved)
            {
                ref var v = ref data[key];
                if(v.isTimeout && v.valid)          // valid but timeout, set to final position.
                {
                    v.update(v.handle, 1);
                    v.onFinish?.Invoke(v.handle);
                }
                else        // to be removed but not timeout, so it's invalid.
                {
                    // Debug.Assert(!v.isTimeout); // removed and timeout, any behaviour could be accepted.
                                                   // let's make it interuppted though.
                    Debug.Assert(v.invalid);
                    v.onInterrupted?.Invoke(v.handle);
                }
                
                v.onRemove?.Invoke(v.handle);
            }
            
            foreach(var key in toBeRemoved)
            {
                ActualDelete(key);
            }
            
            toBeRemoved.Clear();
        }
        
        // 删除被标记的 tween 对象.
        void ActualDelete(ArrayLinkedListKey key)
        {
            ref var d = ref data[key];
            if(targetMap.TryGetValue(d.target, out var bindingList) && bindingList[d.tid].key == key)
            {
                bindingList[d.tid] = TweenHandle.none;
                if(bindingList.count == 0)
                {
                    listPool.Release(bindingList);
                    targetMap.Remove(d.target);
                }
            }
            data.Release(key);
        }
        
        public TweenHandle New(TweenId tid, UnityEngine.Object target, Action<float> setter)
            => New(tid, target, (h, t) => setter(h.Evaluate(t)));
        
        public TweenHandle New(TweenId tid, UnityEngine.Object target, ValueTweeningUpdate onUpdate)
        {
            Debug.Assert(target != null);
            
            var key = data.Take();
            ref var v = ref data[key];
            v.target = target;
            v.tid = tid;
            v.update = onUpdate;
            v.SetHandle(new TweenHandle(key, data));
            
            targetMap.TryGetValue(target, out var bindingList);
            if(bindingList == null)
            {
                bindingList = listPool.Get();
                targetMap.Add(target, bindingList);
            }
            
            var newHandle = new TweenHandle(key, data);
            
            // 给上一个 tween 对象打上删除标记, 并覆盖 bindingList 中的词条.
            var prev = bindingList[tid];
            TagRemoved(prev);
            bindingList[tid] = newHandle;
            return newHandle;
        }
        
        public bool TagRemoved(TweenHandle v)
        {
            if(v.isNone) return false;
            if(!data.Valid(v.key)) return false;
            v.update = null;   // 只打标记.
            return true;
        }
        
        public bool Remove(UnityEngine.Object target, TweenId tid)
        {
            if(!targetMap.TryGetValue(target, out var list)) return false;
            return TagRemoved(list[tid]);
        }
        
        public bool RemoveAll(UnityEngine.Object target)
        {
            if(!targetMap.TryGetValue(target, out var list)) return false;
            var res = false;
            foreach(var i in list.bindings) res |= TagRemoved(i.Value);
            return res;
        }
    }
    
    
    
    public static class TweenExt
    {
        public static TweenHandle NewTween(this UnityEngine.Object g, TweenId tid, ValueTweeningUpdate onUpdate)
        {
            return ProtaTweenManager.instance.New(tid, g, onUpdate);
        }
    }
    
}
