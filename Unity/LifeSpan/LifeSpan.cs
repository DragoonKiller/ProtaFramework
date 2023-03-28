using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    // 一个用来标记和查询生命周期的对象.
    // 对象创建时生命周期开始; Kill() 函数调用时生命周期结束.
    public class LifeSpan
    {
        // 该对象是否处于生命周期中.
        public bool alive { get; protected set; } = true;
        
        protected List<Action> callbacks = null;
        
        public LifeSpan() { }
        
        public virtual void Kill()
        {
            if(!alive) return;
            alive = false;
            callbacks?.InvokeAll();
            callbacks = null;
        }
        
        // 当对象处于生命周期中时, 给它注册一个在生命周期结束时执行的回调.
        // 当对象已经死亡时, 直接执行函数.
        public virtual void OnKill(Action onKilled)
        {
            if(alive)
            {
                callbacks ??= new List<Action>();
                callbacks.Add(onKilled);
                return;
            }
            
            onKilled();
        }
    }
    
    // 当任意 LifeSpan 终止时, 该 Span 终止.
    public class AnyLifeSpan : LifeSpan
    {
        public AnyLifeSpan(params LifeSpan[] spans)
        {
            Action onSubComplete = () => {
                if(!alive) return;
                base.Kill();
            };
            foreach(var f in spans) f.OnKill(onSubComplete);
        }
        
        public override void Kill() => throw new NotSupportedException();
    }
    
    // 当所有 LifeSpan 终止时, 该Span终止.
    public class UnionLifeSpan : LifeSpan
    {
        public readonly int needToCompleteCount = 0;
        public int completeCount { get; private set; } = 0;
        
        public UnionLifeSpan(params LifeSpan[] spans)
        {
            needToCompleteCount = spans.Length;
            Action onSubComplete = () => {
                completeCount += 1;
                if(completeCount == needToCompleteCount) base.Kill();
            };
            foreach(var f in spans) f.OnKill(onSubComplete);
        }
        
        public override void Kill() => throw new NotSupportedException();
        
    }
}
