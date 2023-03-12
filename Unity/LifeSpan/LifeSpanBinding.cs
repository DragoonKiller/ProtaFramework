using UnityEngine;

namespace Prota.Unity
{
    public class LifeSpanBinding : MonoBehaviour
    {
        public readonly LifeSpan span = new LifeSpan();
        
        public void OnDestroy() => span.Kill();
    }
    
    
    public static partial class LifeSpanExt
    {
        public static LifeSpanBinding LifeSpanBinding(this GameObject x) => x.GetOrCreate<LifeSpanBinding>();
        public static LifeSpanBinding LifeSpanBinding(this Component x) => x.GetOrCreate<LifeSpanBinding>();
    
        public static LifeSpan LifeSpan(this GameObject x) => x.LifeSpanBinding().span;
        public static LifeSpan LifeSpan(this Component x) => x.LifeSpanBinding().span;
    }
}
