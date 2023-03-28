using System.Collections.Generic;
namespace Prota.Tween
{
    public class BindingList
    {
        public int count;
        public Dictionary<TweenId, TweenHandle> bindings = new Dictionary<TweenId, TweenHandle>();
        public TweenHandle this[TweenId tid]
        {
            get => tid.isNone ? TweenHandle.none : bindings[tid];
            set
            {
                if(tid.isNone) return;      // no recording if no id.
                if(this.bindings.TryGetValue(tid, out var original))
                {
                    if(original.isNone && !value.isNone) count++;
                    if(!original.isNone && value.isNone) count--;
                }
                bindings[tid] = value;
            }
        }
    }
}
