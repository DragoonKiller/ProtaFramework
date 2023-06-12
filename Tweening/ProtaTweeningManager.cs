using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tween
{
    // h: Tween handle with all tweening information
    // t: current ratio, [0, 1]
    public delegate void ValueTweeningUpdate(TweenHandle h, float t);
    
    
    public class ProtaTweenManager : Singleton<ProtaTweenManager>
    {
        public readonly static ValueTweeningUpdate doNothingValueTweening = (h, t) => { };
        
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
                v.update(new TweenHandle(key, data), v.GetTimeLerp());
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
                var handle = new TweenHandle(key, data);
                
                // 如果是 tween 自己运行到终点, 更新到最后位置.
                if(v.isTimeout && v.valid)
                {
                    v.update(handle, 1);
                    v.onFinish?.Invoke(handle);
                    if(v.reverseOnLoopFinish) handle.SetReverse();
                    // Debug.Log($"Tween {v.tid} on {v.target} is finished.");
                }
                else    // tween 被其它事物终止, 调用另一个回调函数.
                {
                    Debug.Assert(v.invalid);
                    v.onInterrupted?.Invoke(handle);
                    // Debug.Log($"Tween {v.tid} on {v.target} is interrupted.");
                }
                
                if(v.loop) handle.Restart();
                
                // v.onFinish 或 v.onInterrupted 或 loop 可能会重新调整 tween 的起止时间, 这样就不认为它到时间了.
                // 这里需要重新检查一次.
                if(v.invalid || v.isTimeout)
                {
                    bool invalid = v.invalid, timeout = v.isTimeout;
                    v.onRemove?.Invoke(handle);
                    // onRemove 的时候不允许再改变 tween 的状态.
                    Debug.Assert(v.invalid == invalid && v.isTimeout == timeout);
                    
                    // Debug.Log($"Tween {v.tid} on {v.target} is removed.");
                    ActualDelete(key);
                }
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
            var handle = new TweenHandle(key, data);
            handle.Clear();     // 清理原有 tweening 数据.
            
            handle.target = target;
            handle.tid = tid;
            handle.update = onUpdate;
            
            targetMap.TryGetValue(target, out var bindingList);
            if(bindingList == null)
            {
                bindingList = listPool.Get();
                targetMap.Add(target, bindingList);
            }
            
            // 给上一个 tween 对象打上删除标记, 并覆盖 bindingList 中的词条.
            if(bindingList.bindings.TryGetValue(tid, out var prev)) TagRemoved(prev);
            bindingList[tid] = handle;
            return handle;
        }
        
        public bool TagRemoved(TweenHandle v)
        {
            if(v.isNone) return false;
            if(!data.ValidateKey(v.key)) return false;
            v.update = null;   // 只打标记.
            return true;
        }
        
        public bool Remove(UnityEngine.Object target, TweenId tid)
        {
            if(!targetMap.TryGetValue(target, out var list)) return false;
            return TagRemoved(list[tid]);
        }
        
        public void RemoveAll(UnityEngine.Object target)
        {
            if(!targetMap.TryGetValue(target, out var list)) return;
            foreach(var i in list.bindings) TagRemoved(i.Value);
        }
    }
    
    
    
    public static class TweenExt
    {
        public static TweenHandle NewTween(this UnityEngine.Object g, TweenId tid, ValueTweeningUpdate onUpdate)
        {
            return ProtaTweenManager.instance.New(tid, g, onUpdate);
        }
        
        public static TweenHandle TakeupTween(this UnityEngine.Object g, TweenId id)
        {
            return ProtaTweenManager.instance.New(id, g, ProtaTweenManager.doNothingValueTweening);
        }
        
        public static void TakeupTween(this UnityEngine.Object g, TweenId id, params TweenId[] xid)
        {
            g.TakeupTween(id);
            foreach(var i in xid) g.TakeupTween(i);
        }
        
        public static void ClearTween(this UnityEngine.Object g, TweenId id)
        {
            ProtaTweenManager.instance.Remove(g, id);
        }
        
        public static void ClearAllTween(this UnityEngine.Object g)
        {
            ProtaTweenManager.instance.RemoveAll(g);
        }
    }
    
}
