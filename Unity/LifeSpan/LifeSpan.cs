using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public class LifeSpan
    {
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
        
        public virtual void OnComplete(Action onKilled)
        {
            if(alive)
            {
                callbacks = callbacks ?? new List<Action>();
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
            foreach(var f in spans) f.OnComplete(onSubComplete);
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
            foreach(var f in spans) f.OnComplete(onSubComplete);
        }
        
        public override void Kill() => throw new NotSupportedException();
        
    }
}