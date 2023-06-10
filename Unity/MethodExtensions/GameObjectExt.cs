using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static T GetOrCreate<T>(this GameObject g) where T : Component
        {
            if(g.TryGetComponent<T>(out var res)) return res;
            var r = g.AddComponent<T>();
            return r;
        }
        
        
        public static Component GetOrCreate(this GameObject g, Type t)
        {
            if(!typeof(Component).IsAssignableFrom(t)) return null;
            if(g.TryGetComponent(t, out var res)) return res;
            var r = g.AddComponent(t);
            return r;
        }
        
        
        public static string GetNamePath(this GameObject g) => g.transform.GetNamePath();
        
        public static GameObject Activate(this GameObject g, bool active = true)
        {
            g.SetActive(active);
            return g;
        }
        
        public static void ActiveDestroy(this GameObject g)
        {
            // 主动销毁的时候, 发送 OnActiveDestroy 事件.
            // 避免 gameObject 被 Unity 自动销毁的时候创建额外的对象.
            if(g == null) return;
            g.BroadcastMessage("OnActiveDestroy", null, SendMessageOptions.DontRequireReceiver);
            GameObject.Destroy(g);
        }
        
        public static void ActiveDestroy(this GameObject g, object args)
        {
            // 主动销毁的时候, 发送 OnActiveDestroy 事件.
            // 避免 gameObject 被 Unity 自动销毁的时候创建额外的对象.
            if(g == null) return;
            g.BroadcastMessage("OnActiveDestroy", args, SendMessageOptions.DontRequireReceiver);
            GameObject.Destroy(g);
        }
        
        public static GameObject SetIdentity(this GameObject g)
        {
            g.transform.SetIdentity();
            return g;
        }
        
        public static GameObject ClearSub(this GameObject x)
        {
            x.transform.ClearSub();
            return x;
        }
        
        public static bool IsPrefab(this GameObject g) => !g.scene.IsValid();
        
        public static GameObject Clone(this GameObject g, Transform parent = null)
        {
            // 父级: 优先参数 parent, 其次是 g 的 parent, 其次是 null.
            return GameObject.Instantiate(g, parent ?? (g.scene == null ? null : g.transform.parent) ?? null, false);
        }
        
        public static GameObject CloneAsTemplate(this GameObject g, Transform parent = null)
        {
            g.SetActive(false);
            var x = g.Clone(parent);
            x.SetActive(true);
            return x;
        }
        
        
        public static GameObject SetParent(this GameObject g, Transform x = null, bool worldPositionStays = false)
        {
            g.transform.SetParent(x, worldPositionStays);
            return g;
        }
        
        public static RectTransform RectTransform(this GameObject g) => g.transform as RectTransform;
        
        public static IEnumerable<Component> EnumerateComponents(this GameObject g)
        {
            using var _ = TempList<Component>.Get(out var t);
            g.GetComponents(typeof(Component), t);
            foreach(var x in t) yield return x;
        }
        
        public static bool HasDuplicatedComponent(this GameObject g)
        {
            var e = g.EnumerateComponents();
            return e.Count() != new HashSet<Component>(e).Count;
        }
        
        public static F SyncData<F>(this F l, int n, GameObject template, Action<int, GameObject> onEnable)
            where F: List<GameObject>
        {
            for(int i = 0; i < n; i++)
            {
                if(i >= l.Count)
                {
                    l.Add(template.CloneAsTemplate());
                }
                
                onEnable(i, l[i]);
                l[i].SetActive(true);
            }
            
            for(int i = n; i < l.Count; i++) l[i].SetActive(false);
            return l;
        }
        
        public static bool SetToDontDestroyScene(this GameObject x)
        {
            if(x.scene.name == "DontDestroyOnLoad") return false;
            x.transform.SetParent(null);
            GameObject.DontDestroyOnLoad(x);
            return true;
        }
        
        public static bool IsInDontDestroyScene(this GameObject x)
        {
            return x.scene.name == "DontDestroyOnLoad";
        }
        
        public static bool TryGetComponentInParent<T>(this GameObject x, out T v) where T : Component
        {
            v = x.GetComponentInParent<T>(true);
            return v != null;
        }
    }
}
