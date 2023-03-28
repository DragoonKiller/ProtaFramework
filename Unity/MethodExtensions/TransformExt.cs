using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Prota.Unity
{
    public struct TransformAsList<T> : IReadOnlyList<T>
        where T : Component
    {
        Transform transform;
        
        public TransformAsList(Transform transform)
        {
            this.transform = transform;
        }
        
        public T this[int index] => transform.GetChild(index).GetComponent<T>(); 
        
        public int Count => transform.childCount;
        
        public int IndexOf(T t)
        {
            for(int i = 0; i < Count; i++) if(t == this[i]) return i;
            return -1;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < Count; i++) yield return this[i];
        }
        
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
    
    public static partial class UnityMethodExtensions
    {
        public static TransformAsList<T> AsList<T>(this Transform t) where T: Component
            => new TransformAsList<T>(t);
    }
    
    
    
    public static partial class UnityMethodExtensions
    {
        public static void ForeachChild(this Transform t, Action<Transform> f)
        {
            for(int i = 0; i < t.childCount; i++)
            {
                var child = t.GetChild(i);
                f(child);
            }
        }
        
        public static void ForeachParent(this Transform t, Action<Transform> f)
        {
            while(true)
            {
                t = t.parent;
                if(t == null) return;
                f(t);
            }
        }
        
        private static Action<Transform> currentRecursiveTransformOp;
        private static void ForeachTransformRecursivelyInternal(Transform t)
        {
            currentRecursiveTransformOp(t);
            ForeachChild(t, ForeachTransformRecursivelyInternal);
        }
        public static void ForeachTransformRecursively(this Transform t, Action<Transform> f)
        {
            if(t == null || f == null) return;
            currentRecursiveTransformOp = f;
            ForeachTransformRecursivelyInternal(t);
        }
        
        
        public static int GetDepth(this Transform t)
        {
            int d = 0;
            while(t != null)
            {
                t = t.parent;
                d++;
            }
            return d;
        }
        
        public static string GetNamePath(this Transform t)
        {
            var sb = new StringBuilder();
            while(t != null)
            {
                sb.Append("/");
                sb.Append(t.gameObject.name);
                t = t.parent;
            }
            return sb.ToString();
        }
        
        public static T GetOrCreate<T>(this Transform t) where T : Component
        {
            if(t.TryGetComponent<T>(out var res)) return res;
            var r = t.gameObject.AddComponent<T>();
            return r;
        }
        
        public static Transform ClearSub(this Transform x)
        {
            for(int i = x.childCount - 1; i >= 0; i--) x.GetChild(i).gameObject.ActiveDestroy();
            return x;
        }
        
        public static Transform SetIdentity(this Transform tr)
        {
            tr.position = Vector3.zero;
            tr.rotation = Quaternion.identity;
            tr.localScale = Vector3.one;
            return tr;
        }
        
        public static bool IsPrefab(this Transform tr) => tr.gameObject.IsPrefab();
        
        public static Transform SetParent(this Transform tr, Transform parent)
        {
            tr.parent = parent;
            return tr;
        }
        
        public static Transform SetPosition(this Transform tr, Vector3 position)
        {
            tr.position = position;
            return tr;
        }
        
        public static Transform SetRotation(this Transform tr, Quaternion rotation)
        {
            tr.rotation = rotation;
            return tr;
        }
        
        public static Transform SetRotation(this Transform tr, Vector3 euler)
        {
            tr.rotation = Quaternion.Euler(euler);
            return tr;
        }
        
        public static Transform SetLocalPosition(this Transform tr, Vector3 position)
        {
            tr.localPosition = position;
            return tr;
        }
        
        public static Transform SetLocalRotation(this Transform tr, Quaternion rotation)
        {
            tr.localRotation = rotation;
            return tr;
        }
        
        public static Transform SetLocalRotation(this Transform tr, Vector3 euler)
        {
            tr.localRotation = Quaternion.Euler(euler);
            return tr;
        }
        
        public static Transform SetLocalScale(this Transform tr, Vector3 scale)
        {
            tr.localScale = scale;
            return tr;
        }
        
        public static Transform ScaleLocallyByPivot(this Transform tr, Vector3 scale, Vector3 pivot)
        {
            // original position related to center:
            var pp = tr.localPosition - pivot;
            // scale the position.
            pp = Vector3.Scale(pp, scale);
            // move tr to the final position.
            tr.localPosition = pivot + pp;
            
            // scale others normally.
            tr.localScale = Vector3.Scale(tr.localScale, scale);
            
            return tr;
        }
        
    }
}
