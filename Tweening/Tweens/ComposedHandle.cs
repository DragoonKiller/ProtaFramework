using System;
using Prota.Unity;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Prota.Tween
{
    public struct ComposedHandle
    {
        List<(string name, TweenHandle handle)> handles;

        public int anchor { get; private set; }      // 注册事件的时候注册哪个. 默认是 0.
        
        public int count => handles.Count;
        
        public TweenHandle this[string x]
        {
            get
            {
                foreach (var (name, handle) in handles)
                {
                    if (name == x) return handle;
                }
                return TweenHandle.none;
            }
        }
        
        public TweenHandle this[int x]
        {
            get
            {
                if (x < 0 || x >= handles.Count) return TweenHandle.none;
                return handles[x].handle;
            }
        }
        
        public ComposedHandle SetAnchor(int x)
        {
            anchor = x;
            return this;
        }
        
        public ComposedHandle SetAnchor(string x)
        {
            for (int i = 0; i < handles.Count; i++)
            {
                if (handles[i].name != x) continue;
                anchor = i;
                break;
            }
            return this;
        }
        
        public ComposedHandle Get(string x, out TweenHandle handle)
        {
            handle = this[x];
            return this;
        }
        
        public ComposedHandle Get(int x, out TweenHandle handle)
        {
            handle = this[x];
            return this;
        }
        
        public ComposedHandle(List<(string name, TweenHandle handle)> handles, int anchor = 0)
        {
            this.handles = handles;
            this.anchor = anchor;
        }
        
        // ====================================================================================================
        // 设置单一 Handle 的函数.
        // ====================================================================================================
        
        public ComposedHandle OnFinish(Action<TweenHandle> action)
        {
            handles[anchor].handle.OnFinish(action);
            return this;
        }
        
        public ComposedHandle OnInterrupted(Action<TweenHandle> action)
        {
            handles[anchor].handle.OnInterrupted(action);
            return this;
        }
        
        public ComposedHandle OnRemove(Action<TweenHandle> action)
        {
            handles[anchor].handle.OnRemove(action);
            return this;
        }
        
        // ====================================================================================================
        // 设置所有 Handle 的函数.
        // ====================================================================================================
        
        public ComposedHandle SetFromTo(Vector3? from, Vector3? to)
        {
            handles[0].handle.SetFromTo(from?.x, to?.x);
            if(count <= 1) return this;
            handles[1].handle.SetFromTo(from?.y, to?.y);
            if(count <= 2) return this;
            handles[2].handle.SetFromTo(from?.z, to?.z);
            return this;
        }
        
        public ComposedHandle SetFromTo(Vector2? from, Vector2? to)
        {
            handles[0].handle.SetFromTo(from?.x, to?.x);
            if(count <= 1) return this;
            handles[1].handle.SetFromTo(from?.y, to?.y);
            return this;
        }
        
        public ComposedHandle SetFromTo(Vector4? from, Vector4? to)
        {
            handles[0].handle.SetFromTo(from?.x, to?.x);
            if(count <= 1) return this;
            handles[1].handle.SetFromTo(from?.y, to?.y);
            if(count <= 2) return this;
            handles[2].handle.SetFromTo(from?.z, to?.z);
            if(count <= 3) return this;
            handles[3].handle.SetFromTo(from?.w, to?.w);
            return this;
        }
        
        
        
        public ComposedHandle SetFromTo(Color? from, Color? to)
        {
            handles[0].handle.SetFromTo(from?.r, to?.r);
            if(count <= 1) return this;
            handles[1].handle.SetFromTo(from?.g, to?.g);
            if(count <= 2) return this;
            handles[2].handle.SetFromTo(from?.b, to?.b);
            if(count <= 3) return this;
            handles[3].handle.SetFromTo(from?.a, to?.a);
            return this;
        }
        
        public ComposedHandle SetFromTo(float? from, float? to)
        {
            foreach(var (name, handle) in handles) handle.SetFromTo(from, to);
            return this;
        }
        
        
        public ComposedHandle Clear()
        {
            foreach(var (name, handle) in handles) handle.Clear();
            return this;
        }
        
        public ComposedHandle Kill()
        {
            foreach(var (name, handle) in handles) handle.Kill();
            return this;
        }
        
        public ComposedHandle Start(float duration, bool realtime = false)
        {
            foreach(var (name, handle) in handles) handle.Start(duration, realtime);
            return this;
        }
        
        public ComposedHandle SetLoop(bool loop, bool reverseOnLoopFinish = false)
        {
            foreach(var (name, handle) in handles) handle.SetLoop(loop, reverseOnLoopFinish);
            return this;
        }
        
        public ComposedHandle Restart()
        {
            foreach(var (name, handle) in handles) handle.Restart();
            return this;
        }
        
        public ComposedHandle SetEase(TweenEase ease)
        {
            foreach(var (name, handle) in handles) handle.SetEase(ease);
            return this;
        }
        
        public ComposedHandle SetEase(TweenEaseEnum ease)
        {
            foreach(var (name, handle) in handles) handle.SetEase(ease);
            return this;
        }
        
        public ComposedHandle SetEase(AnimationCurve curve)
        {
            foreach(var (name, handle) in handles) handle.SetEase(curve);
            return this;
        }
        
        public ComposedHandle SetGuard(LifeSpan guard)
        {
            foreach(var (name, handle) in handles) handle.SetGuard(guard);
            return this;
        }
        
        public ComposedHandle SetGuard(GameObject guard)
        {
            foreach(var (name, handle) in handles) handle.SetGuard(guard.LifeSpan());
            return this;
        }
        
        public ComposedHandle SetCurrentRatio(float ratio)
        {
            foreach(var (name, handle) in handles) handle.SetCurrentRatio(ratio);
            return this;
        }
        
        // ====================================================================================================
        // 获取信息.
        // ====================================================================================================
        
        public float duration => handles[anchor].handle.duration;
        
        public float timeFrom => handles[anchor].handle.timeFrom;
        
        public float timeTo => handles[anchor].handle.timeTo;
        
        public bool realtime => handles[anchor].handle.realtime;
        
        public LifeSpan guard => handles[anchor].handle.guard;
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        
        public static ComposedHandle Make(
            string nameA, TweenHandle a, 
            string nameB, TweenHandle b, 
            string nameC, TweenHandle c, 
            string nameD, TweenHandle d)
        {
            var handles = new List<(string name, TweenHandle handle)>(4);
            handles.Add((nameA, a));
            handles.Add((nameB, b));
            handles.Add((nameC, c));
            handles.Add((nameD, d));
            return new ComposedHandle(handles);
        }
        
        public static ComposedHandle Make(
            string nameA, TweenHandle a, 
            string nameB, TweenHandle b, 
            string nameC, TweenHandle c)
        {
            var handles = new List<(string name, TweenHandle handle)>(3);
            handles.Add((nameA, a));
            handles.Add((nameB, b));
            handles.Add((nameC, c));
            return new ComposedHandle(handles);
        }
        
        public static ComposedHandle Make(
            string nameA, TweenHandle a, 
            string nameB, TweenHandle b)
        {
            var handles = new List<(string name, TweenHandle handle)>(2);
            handles.Add((nameA, a));
            handles.Add((nameB, b));
            return new ComposedHandle(handles);
        }
        
        public static ComposedHandle Make(
            TweenHandle a,
            TweenHandle b,
            TweenHandle c,
            TweenHandle d)
        {
            var handles = new List<(string name, TweenHandle handle)>(4);
            handles.Add(("", a));
            handles.Add(("", b));
            handles.Add(("", c));
            handles.Add(("", d));
            return new ComposedHandle(handles);
        }
        
        public static ComposedHandle Make(
            TweenHandle a,
            TweenHandle b,
            TweenHandle c)
        {
            var handles = new List<(string name, TweenHandle handle)>(3);
            handles.Add(("", a));
            handles.Add(("", b));
            handles.Add(("", c));
            return new ComposedHandle(handles);
        }
        
        public static ComposedHandle Make(
            TweenHandle a,
            TweenHandle b)
        {
            var handles = new List<(string name, TweenHandle handle)>(2);
            handles.Add(("", a));
            handles.Add(("", b));
            return new ComposedHandle(handles);
        }
        
        
        
    }
    
}
