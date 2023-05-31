using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prota.Unity;
using System.Reflection;
using System.Text;
using System.Runtime.Versioning;
using System.Linq;

namespace Prota.Tween
{
    public enum TweenEaseEnum
    {
        Linear,
        SinIn,
        SinOut,
        SinInOut,
        QuadIn,
        QuadOut,
        QuadInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        QuartIn,
        QuartOut,
        QuartInOut,
        QuintIn,
        QuintOut,
        QuintInOut,
        ExpoIn,
        ExpoOut,
        ExpoInOut,
        CircIn,
        CircOut,
        CircInOut,
        BackIn,
        BackOut,
        BackInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
        
        AppearIn,
        AppearOut,
    }
    
    [Serializable]
    public partial struct TweenEase
    {
        
        readonly Func<float, float> f;
        public bool valid => f != null;
        public TweenEase(Func<float, float> f) => this.f = f;
        public static implicit operator TweenEase(Func<float, float> f) => new TweenEase(f);
        public static implicit operator Func<float, float>(TweenEase f) => f.f;
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        static Dictionary<TweenEaseEnum, TweenEase> builtinEases = new Dictionary<TweenEaseEnum, TweenEase>();
        static TweenEase()
        {
            var bindingFlags = BindingFlags.Static | BindingFlags.Public;
            var fields = typeof(TweenEase).GetFields(bindingFlags);
            foreach (var f in fields) {
                if (f.FieldType != typeof(TweenEase)) continue;
                var name = f.Name.Substring(0, 1).ToUpper() + f.Name.Substring(1);
                var value = (TweenEase)f.GetValue(null);
                builtinEases.Add((TweenEaseEnum)Enum.Parse(typeof(TweenEaseEnum), name), value);
            }
        }
        
        public static TweenEase GetFromEnum(TweenEaseEnum easeEnum)
        {
            return builtinEases[easeEnum];
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public float Evaluate(float x)
        {
            #if UNITY_EDITOR
            Debug.Assert(0 <= x && x <= 1);
            #endif
            return f(x);
        }
        
        
        public static TweenEase Step(Func<float, float> f, int stepCount = 1)
        {
            return new TweenEase(x => f(Mathf.Ceil(x * stepCount)));
        }
        
        // ====================================================================================================
        // builtin ease by custom animation curve.
        // ====================================================================================================
        
        public static readonly TweenEaseCurves curves
            = Resources.LoadAll<TweenEaseCurves>("TweenEaseCurves")
                .FirstOrDefault()
                .AssertNotNull();
        
        public static readonly TweenEase appearIn = new TweenEase(x => curves.appearIn.Evaluate(x));
        public static readonly TweenEase appearOut = new TweenEase(x => curves.appearOut.Evaluate(x));
        
        // ====================================================================================================
        // builtin ease functions https://easings.net/#
        // ====================================================================================================
        
        const float c1 = 1.70158f;
        const float c2 = c1 * 1.525f;
        const float c3 = c1 + 1;
        const float c4 = (2 * Mathf.PI) / 3;
        const float c5 = (2 * Mathf.PI) / 4.5f;
        const float n1 = 7.5625f;
        const float d1 = 2.75f;
        
        public static readonly TweenEase linear = new TweenEase(x => x);
        public static readonly TweenEase sinIn = new TweenEase(x => 1 - Mathf.Cos(x * Mathf.PI / 2));
        public static readonly TweenEase sinOut = new TweenEase(x => (x * Mathf.PI / 2).Sin());
        public static readonly TweenEase sinInOut = new TweenEase(x => -(Mathf.Cos(Mathf.PI * x) - 1) / 2);
        public static readonly TweenEase quadIn = new TweenEase(x => x * x);
        public static readonly TweenEase quadOut = new TweenEase(x => 1 - (1 - x) * (1 - x));
        public static readonly TweenEase quadInOut = new TweenEase(x => x < 0.5f ? 2 * x * x : 1 - (-2 * x + 2).Sqr() / 2);
        public static readonly TweenEase cubicIn = new TweenEase(x => x * x * x);
        public static readonly TweenEase cubicOut = new TweenEase(x => 1 - (1 - x).Cube());
        public static readonly TweenEase cubicInOut = new TweenEase(x => x < 0.5f ? 4 * x * x * x : 1 - (-2 * x + 2).Cube() / 2);
        public static readonly TweenEase quartIn = new TweenEase(x => x.Sqr().Sqr());
        public static readonly TweenEase quartOut = new TweenEase(x => 1 - (1 - x).Sqr().Sqr());
        public static readonly TweenEase quartInOut = new TweenEase(x => x < 0.5f ? 8 * x * x * x * x : 1 - (-2 * x + 2).Sqr().Sqr() / 2);
        public static readonly TweenEase quintIn = new TweenEase(x => x.Pow(5));
        public static readonly TweenEase quintOut = new TweenEase(x => 1 - (1 - x).Pow(5));
        public static readonly TweenEase quintInOut = new TweenEase(x => x < 0.5f ? 16 * x * x * x * x * x : 1 - (-2 * x + 2).Pow(5) / 2);
        public static readonly TweenEase expoIn = new TweenEase(x => x <= 0 ? 0 : (2f).Pow(10 * x - 10));
        public static readonly TweenEase expoOut = new TweenEase(x => x >= 1 ? 1 : 1 - (2f).Pow(-10 * x));
        public static readonly TweenEase expoInOut = new TweenEase(x => 
            x <= 0 ? 0
            : x >= 1 ? 1
            : x < 0.5f ? (2f).Pow(20 * x - 10) / 2
            : (2 - (2f).Pow(-20 * x + 10)) / 2
        );
        public static readonly TweenEase circIn = new TweenEase(x => 1 - (1 - x.Sqr()).Sqrt());
        public static readonly TweenEase circOut = new TweenEase(x => (1 - (x - 1).Sqr()).Sqrt());
        public static readonly TweenEase circInOut = new TweenEase(x =>
            x < 0.5 ? (1 - (1 - (2 * x).Sqr()).Sqrt()) / 2
            : ((1 - (-2 * x + 2).Sqr()).Sqrt() + 1) / 2
        );
        
        public static readonly TweenEase backIn = new TweenEase(x => c3 * x * x * x - c1 * x * x);
        public static readonly TweenEase backOut = new TweenEase(x => 1 + c3 * (x - 1).Pow(3) + c1 * (x - 1).Pow(2));
        public static readonly TweenEase backInOut = new TweenEase(x =>
            x < 0.5 ? ((2 * x).Sqr() * ((3.59491f + 1) * 2 * x - 2.59491f)) / 2
            : ((2 * x - 2).Sqr() * (3.59491f * (x * 2 - 2) + 2.59491f) + 2) / 2
        );
        
        
        
        public static readonly TweenEase elasticIn = new TweenEase(x => x == 0 ? 0 : x == 1 ? 1 : (2f).Pow(10 * x - 10) * (10 * x - 10.75f).Sin() * c4);
        public static readonly TweenEase elasticOut = new TweenEase(x => x == 0 ? 0 : x == 1 ? 1 : (2f).Pow(-10 * x) * (10 * x - 0.75f).Sin() * c4 + 1);
        public static readonly TweenEase elasticInOut = new TweenEase(x =>
            x < 0.5
            ? ((2 * x).Pow(2) * ((c2 + 1) * 2 * x - c2)) / 2
            : ((2 * x - 2).Pow(2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2
        );
        
        public static readonly TweenEase bounceIn = new TweenEase(x => 1 - bounceOut.f(x - 1));
        public static readonly TweenEase bounceOut = new TweenEase(x => {

            if (x < 1 / d1) {
                return n1 * x * x;
            } else if (x < 2f / d1) {
                x -= 1.5f / d1;
                return n1 * x * x + 0.75f;
            } else if (x < 2.5f / d1) {
                x -= 2.25f / d1;
                return n1 * x * x + 0.9375f;
            } else {
                x -= 2.625f / d1;
                return n1 * x * x + 0.984375f;
            }
        });
        public static readonly TweenEase bounceInOut = new TweenEase(x => 
            x < 0.5f
            ? (1 - bounceIn.f(1 - 2 * x)) / 2
            : (1 + bounceOut.f(2 * x - 1)) / 2
        );
        
    }
}
