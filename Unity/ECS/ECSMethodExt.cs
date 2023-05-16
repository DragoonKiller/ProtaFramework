
using UnityEngine;

namespace Prota.Unity
{

    public static partial class UnityMethodExtensions
    {
        public static ERoot EntityRoot(this GameObject go)
            => go.GetComponentInParent<ERoot>();
        public static ERoot EntityRoot(this Component g)
            => g.GetComponentInParent<ERoot>();
    }
    
    
}
