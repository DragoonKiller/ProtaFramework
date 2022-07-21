using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static int Abs(this int x) => Math.Abs(x);
        public static long Abs(this long x) => Math.Abs(x);
        public static double Abs(this double x) => Math.Abs(x);
        public static float Abs(this float x) => Math.Abs(x);
        
        public static int Max(this int a, int b) => Math.Max(a, b);
        public static long Max(this long a, long b) => Math.Max(a, b);
        public static double Max(this double a, double b) => Math.Max(a, b);
        public static float Max(this float a, float b) => Math.Max(a, b);
        
        public static int Min(this int a, int b) => Math.Min(a, b);
        public static long Min(this long a, long b) => Math.Min(a, b);
        public static double Min(this double a, double b) => Math.Min(a, b);
        public static float Min(this float a, float b) => Math.Min(a, b);
        
        public static int Sign(this int x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        public static int Sign(this long x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        public static int Sign(this double x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        public static int Sign(this float x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        
        public static float Exp(this float x) => (float)Math.Exp(x);
        public static float Pow(this float x, float y) => (float)Math.Pow(x, y);
        public static float Floor(this float x) => (float)Math.Floor(x);
        public static float Ceil(this float x) => (float)Math.Ceiling(x);
        
        public static int Mod(this int x, int y) => x - x / y * y;
        public static long Mod(this long x, long y) => x - x / y * y;
        public static float Mod(this float x, float y) => x - (x / y).Floor() * y;
        public static double Mod(this double x, double y) => x - Math.Floor(x / y) * y;
        
        
        public static float Div(this float x, float y)
        {
            return (x - x.Mod(y)) / y;
        }
        
        public static int Div(this int x, int y)
        {
            return (x - x.Mod(y)) / y;
        }
        
        public static bool In(this int x, int a, int b) => a <= x && x <= b;
        
        public static bool InExclusive(this int x, int a, int b) => a < x && x < b;
        
        public static bool In(this float x, float a, float b) => a <= x && x <= b;
        
        public static bool InExclusive(this float x, float a, float b) => a < x && x < b;
        
        public static int FloorToInt(this float x) => (int)Math.Floor(x);
        public static int CeilToInt(this float x) => (int)Math.Ceiling(x);
        
        public static float Clamp(this float x, float a, float b) => x < a ? a : x > b ? b : x;
        
        
        public static float XMap(this float x, float a, float b) => (x - a) / (b - a);
        public static float XMap(this float x, float a, float b, float from, float to) => (x - a) / (b - a) * (to - from) + from;
        
    }
}
