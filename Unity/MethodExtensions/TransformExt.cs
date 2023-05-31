using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Prota.Unity
{
    // T: 使用什么组件来控制. 不知道填什么就填 Transform.
    public struct TransformAsList<T> : IReadOnlyList<T>
        where T : Component
    {
        Transform root;
        
        public TransformAsList(Transform transform)
        {
            this.root = transform;
        }
        
        public T this[int index] => root.GetChild(index).GetComponent<T>(); 
        
        public T this[params string[] name]
        {
            get
            {
                var cur = root;
                for(int i = 0; i < name.Length; i++)
                {
                    cur = cur.Find(name[i]);
                    if(cur == null) throw new Exception($"{ name.ToStringJoined("/") } 找不到 { name[i] }");
                }
                var c = cur.GetComponent<T>();
                if(c == null) throw new Exception($"{ name.ToStringJoined("/") } 找到了GameObject但是找不到组件 { nameof(T) }");
                return c;
            }
        }
        
        public int Count => root.childCount;
        
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
        
        public TransformAsList<T> Replace(int index, T t)
        {
            var old = this[index];
            if(old == t) return this;
            t.transform.SetParent(root, false);
            t.transform.SetSiblingIndex(index);
            GameObject.Destroy(old.gameObject);
            return this;
        }
        
        public TransformAsList<T> CloneAddWithTemplate(T template)
        {
            var clone = template.CloneAsTemplate(root);
            clone.transform.SetAsLastSibling();
            return this;
        }
        
        // public void Add(T t)
        public TransformAsList<T> CloneAdd(T t)
        {
            var clone = t.Clone(root);
            clone.transform.SetAsLastSibling();
            return this;
        }
        
        public TransformAsList<T> MoveAdd(T t)
        {
            t.transform.SetParent(root, false);
            t.transform.SetAsLastSibling();
            return this;
        }
        
        public TransformAsList<T> CloneInsert(int index, T t)
        {
            var clone = t.Clone(root);
            clone.transform.SetSiblingIndex(index);
            return this;
        }
        
        public TransformAsList<T> MoveInsert(int index, T t)
        {
            t.transform.SetParent(root, false);
            t.transform.SetSiblingIndex(index);
            return this;
        }
        
        public TransformAsList<T> RemoveAt(int index)
        {
            var t = this[index];
            GameObject.Destroy(t.gameObject);
            return this;
        }
        
        public TransformAsList<T> Remove(T t)
        {
            var index = IndexOf(t);
            if(index < 0) return this;
            return RemoveAt(index);
        }
        
        public void EnsureCountWithCloneAdd(int count, T prefab)
        {
            while(Count < count) CloneAdd(prefab);
        }
        
        public void EnsureCountWithCloneAddTemplate(int count, T prefab)
        {
            while(Count < count) CloneAddWithTemplate(prefab);
        }
        
        public void SetSync(int n, T template, Action<int, T> onActivate = null, Action<int, T> onDeactivate = null)
        {
            this.EnsureCountWithCloneAddTemplate(n, template);
            for(int i = 0; i < Count; i++)
            {
                var t = this[i];
                if(i < n)
                {
                    if(onActivate != null) onActivate.Invoke(i, t);
                    else t.SetActive(true);
                }
                else
                {
                    if(onDeactivate != null) onDeactivate.Invoke(i, t);
                    else t.SetActive(false);
                }
            }
        }
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
        
        [ThreadStatic] static Func<Transform, bool> currentRecursiveTransformOpX;
        static void ForeachTransformRecursivelyInternalX(Transform t)
        {
            if(!currentRecursiveTransformOpX(t)) return;
            ForeachChild(t, ForeachTransformRecursivelyInternalX);
        }
        
        [ThreadStatic] static Action<Transform> currentRecursiveTransformOp;
        static void ForeachTransformRecursivelyInternal(Transform t)
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
        
        // 返回值: 是否要递归搜索.
        public static void ForeachTransformRecursively(this Transform t, Func<Transform, bool> f)
        {
            if(t == null || f == null) return;
            currentRecursiveTransformOpX = f;
            ForeachTransformRecursivelyInternalX(t);
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
            var sb = new List<string>();
            while(t != null)
            {
                sb.Add(t.gameObject.name);
                t = t.parent;
            }
            sb.Reverse();
            return sb.ToStringJoined("/");
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
        
        public static Vector3 PositionTo(this Transform a, Transform b)
            => b.position - a.position;
        
        public static Vector3 PositionTo(this Transform a, Vector3 b)
            => b - a.position;
    }
}
