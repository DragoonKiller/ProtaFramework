using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
namespace Prota.Tweening
{
    public delegate void ValueTweeningCallback(float t, float a, float b, AnimationCurve curve);
    
    public enum TweeningType
    {
        Custom = -1,  // does not counted into duplicate.
        
        MoveX = 1,
        MoveY,
        MoveZ,
        ScaleX,
        ScaleY,
        ScaleZ,
        RotateX,
        RotateY,
        RotateZ,
        ColorR,
        ColorG,
        ColorB,
        Transparency,
        
    }
    
    public class ValueTweeningHandle
    {
        internal long id;
        internal UnityEngine.Object binding;      // duplicated control. cannot be null.
        internal UnityEngine.Object guard;        // lifetime control. cannot be null.
        internal TweeningType type;
        public float from;
        public float to;
        public AnimationCurve curve;
        internal float timeFrom;
        internal float timeTo;
        internal bool realtime;
        internal ValueTweeningCallback callback;
    }
    
    
    public class BindingList
    {
        public int count = 0;
        public ValueTweeningHandle[] bindings = new ValueTweeningHandle[15];
        public ValueTweeningHandle this[TweeningType type]
        {
            get => type < 0 ? null : bindings[(int)type];
            set
            {
                
                if(type < 0) return;         // always 0 while type < 0
                var original = bindings[(int)type];
                if(original == null && value != null) count++;
                if(original != null && value == null) count--;
                bindings[(int)type] = value;
            }
        }
    }
    
    [Serializable]
    public class ValueTweening
    {
        public static long idGen = 0;
        
        public Dictionary<long, ValueTweeningHandle> idMap = new Dictionary<long, ValueTweeningHandle>();
        
        public Dictionary<UnityEngine.Object, BindingList> bindingMap = new Dictionary<UnityEngine.Object, BindingList>(); 
        
        public List<long> toBeRemoved = new List<long>();
        
        public ObjectPool<ValueTweeningHandle> pool = new ObjectPool<ValueTweeningHandle>(() => new ValueTweeningHandle());
        public ObjectPool<BindingList> listPool = new ObjectPool<BindingList>(() => new BindingList()); // TweenType.Count
        
        public void Update()
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
                var ratio = (v.timeFrom, v.timeTo).InvLerp(v.realtime ? Time.realtimeSinceStartup : Time.time);
                v.callback(ratio, v.from, v.to, v.curve);
            }
        }
        
        public ValueTweeningHandle Add(TweeningType type, GameObject binding, UnityEngine.Object guard, float from, float to, AnimationCurve curve, float duration, bool realtime, ValueTweeningCallback callback)
        {
            Debug.Assert(binding != null);
            Debug.Assert(guard != null);
            
            var timeFrom = realtime ? Time.realtimeSinceStartup : Time.time;
            var timeTo = timeFrom + duration;
            var v = pool.Get();
            v.id = ++idGen;
            v.binding = binding;
            v.guard = guard;
            v.type = type;
            v.from = from;
            v.to = to;
            v.curve = curve;
            v.timeFrom = timeFrom;
            v.timeTo = timeTo;
            v.realtime = realtime;
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
        
        
        public void Remove(ValueTweeningHandle v)
        {
            if(idMap.ContainsKey(v.id)) v.callback = null;
        }
    }
    
    
}