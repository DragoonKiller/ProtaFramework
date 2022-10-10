using System;
using UnityEngine;

namespace Prota
{
    public static partial class UnityMethodExtensions
    {
        public static Color Add(this Color a, Color b) => new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        
        public static Color Sub(this Color p, Color q) => new Color(p.r - q.r, p.g - q.g, p.b - q.b, p.a - q.a);
    }
}