using System.Collections.Generic;
using Prota.Unity;
using UnityEngine;
using Prota;

namespace Prota.Animation
{
    public class CodeAnimationManager : Singleton<CodeAnimationManager>
    {
        public readonly List<Tweening> all = new List<Tweening>();
        
        public readonly HashSet<Tweening> toBeRemoved = new HashSet<Tweening>();
        
        public readonly Dictionary<UnityEngine.Object, List<Tweening>> bindings = new Dictionary<Object, List<Tweening>>();
        
        void FixedUpdate()
        {
            // Update.
            foreach(var tween in all)
            {
                if(tween.updateMode == UpdateMode.FixedUpdate)
                {
                    tween.Update(Time.fixedDeltaTime);
                }
            }
        }
        
        void Update()
        {
            // Remove.
            for(int i = 0; i < all.Count; i++)
            {
                if(toBeRemoved.Contains(all[i]) || !all[i].valid)
                {
                    all.RemoveBySwap(i);
                    i -= 1;
                }
            }
            toBeRemoved.Clear();
            
            // Update.
            foreach(var tween in all)
            {
                if(tween.updateMode == UpdateMode.Update)
                {
                    tween.Update(Time.deltaTime);
                }
            }
        }
        
        public void Remove(Tweening t) => toBeRemoved.Add(t);
        
        public void Add(Tweening t) => all.Add(t);
        
        public void Bind(UnityEngine.GameObject target, Tweening t)
        {
            bindings.GetOrCreate(target, out var list);
            list.Add(t);
        }
        
        public void KillAll(Component t)
        {
            KillAll(t.gameObject);
        }
        
        // 杀掉该 gameObject 的所有绑定.
        public void KillAll(GameObject t)
        {
            if(bindings.TryGetValue(t.gameObject, out var list)) foreach(var i in list) i.Dispose();
        }
    }
}