using UnityEngine;

namespace Prota.Tween
{
    [CreateAssetMenu(menuName = "Prota Framework/TweenEaseCurves")]
    public class TweenEaseCurves : ScriptableObject
    {
        public AnimationCurve appearIn;
        public AnimationCurve appearOut;
    }
}
