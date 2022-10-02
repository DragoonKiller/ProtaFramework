using System;

namespace Prota
{
    public static partial class MethodExtensions
    {
        public static float Sqr(this float x) => x * x;
        public static int Sqr(this int x) => x * x;
        public static double Sqr(this double x) => x * x;
        public static long Sqr(this long x) => x * x;
        
        public static float Cube(this float x) => x * x * x;
        public static int Cube(this int x) => x * x * x;
        public static double Cube(this double x) => x * x * x;
        public static long Cube(this long x) => x * x * x;
        
        public static int Abs(this int x) => Math.Abs(x);
        public static long Abs(this long x) => Math.Abs(x);
        public static double Abs(this double x) => Math.Abs(x);
        public static float Abs(this float x) => Math.Abs(x);
        
        public static double Sin(this double x) => Math.Sin(x);
        public static double Cos(this double x) => Math.Cos(x);
        public static double Tan(this double x) => Math.Tan(x);
        
        
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
        
        public static int ModSys(this int x, int y) => x % y < 0 ? x % y + Math.Abs(y) : x % y;
        public static long ModSys(this long x, long y) => x % y < 0 ? x % y + Math.Abs(y) : x % y;
        
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
        public static int FloorToInt(this double x) => (int)Math.Floor(x);
        public static int CeilToInt(this float x) => (int)Math.Ceiling(x);
        public static int CeilToInt(this double x) => (int)Math.Ceiling(x);
        public static int RoundToInt(this float x) => (int)Math.Round(x);
        public static int RoundToInt(this double x) => (int)Math.Round(x);
        
        public static double Clamp(this double x, double a, double b) => x < a ? a : x > b ? b : x;
        public static float Clamp(this float x, float a, float b) => x < a ? a : x > b ? b : x;
        public static long Clamp(this long x, long a, long b) => x < a ? a : x > b ? b : x;
        public static int Clamp(this int x, int a, int b) => x < a ? a : x > b ? b : x;
        
        
        
        public static float XMap(this float x, float a, float b) => (x - a) / (b - a);
        public static float XMap(this float x, float a, float b, float from, float to) => (x - a) / (b - a) * (to - from) + from;
        
        public static float Lerp(this (float a, float b) v, float x) => (v.b - v.a) * x + v.a;
        public static double Lerp(this (double a, double b) v, double x) => (v.b - v.a) * x + v.a;
        
        public static float InvLerp(this (float a, float b) v, float x) => (x - v.a) / (v.b - v.a);
        public static double InvLerp(this (double a, double b) v, double x) => (x - v.a) / (v.b - v.a);
        
        
    }
}
