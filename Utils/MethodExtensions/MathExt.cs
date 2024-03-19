using System;
using System.Collections.Generic;

namespace Prota
{
    public static partial class MethodExtensions
    {
        // x: inited.
        // 用法: if(!a.inited.NeedInit()) return;
        public static bool NeedInit(this ref bool x)
        {
            if(x) return false;
            x = true;
            return true;
        }
        
        // 如果不一样就返回 false. 否则返回 true.
        // 最终的值会设置为 current.
        public static bool CompareAndReplace<T>(this ref T record, T current) where T: struct
        {
            var res = EqualityComparer<T>.Default.Equals(record, current);
            record = current;
            return res;
        }
        
        public static T CompareChanged<T>(this T record, T current, out bool changed)
        {
            changed = EqualityComparer<T>.Default.Equals(record, current);
            return changed ? current : record;
        }
        
        public static float LinearStep(this float x, float a, float b) => ((x - a) / (b - a)).Clamp(0, 1);
        
        public static double LinearStep(this double x, double a, double b) => ((x - a) / (b - a)).Clamp(0, 1);
        
        public static float LinearStepRev(this float x, float a, float b) => 1 - ((x - a) / (b - a)).Clamp(0, 1);
        
        public static double LinearStepRev(this double x, double a, double b) => 1 - ((x - a) / (b - a)).Clamp(0, 1);
        
        public static float SmoothStep(this float x, float a, float b)
        {
            var t = ((x - a) / (b - a)).Clamp(0, 1);
            return t * t * (3 - 2 * t);
        }
        
        public static double SmoothStep(this double x, double a, double b)
        {
            var t = ((x - a) / (b - a)).Clamp(0, 1);
            return t * t * (3 - 2 * t);
        }
        
        public static bool ApproximatelyEqual(this float x, float y) => Math.Abs(x - y) < 1e-6f;
        
        public static int ToInt(this float x) => (int)x;
        public static int ToInt(this double x) => (int)x;
        
        public static float ToFloat(this int x) => (float)x;
        public static float ToFloat(this double x) => (float)x;
        
        public static double ToDouble(this int x) => (double)x;
        public static double ToDouble(this float x) => (double)x;
        
        
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
        public static float Round(this float x) => (float)Math.Round(x);
        
        public static int Repeat(this int x, int y)
        {
            var m = x % y;
            if(m < 0) m += y;
            return m;
        }
        
        public static long Repeat(this long x, long y)
        {
            var m = x % y;
            if(m < 0) m += y;
            return m;
        }
        
        public static float Repeat(this float x, float y)
        {
            var m = x % y;
            if(m < 0) m += y;
            return m;
        }
        
        public static double Repeat(this double x, double y)
        {
            var m = x % y;
            if(m < 0) m += y;
            return m;
        }
        
        public static bool In(this int x, int a, int b) => a <= x && x <= b;
        public static bool InExclusive(this int x, int a, int b) => a < x && x < b;
        public static bool In(this float x, float a, float b) => a <= x && x <= b;
        public static bool InExclusive(this float x, float a, float b) => a < x && x < b;
        public static bool In(this double x, double a, double b) => a <= x && x <= b;
        public static bool InExclusive(this double x, double a, double b) => a < x && x < b;
        
        public static bool Contains(this (int a, int b) v, int x) => v.a <= x && x <= v.b;
        public static bool ContainsExclusive(this (int a, int b) v, int x) => v.a < x && x < v.b;
        public static bool Contains(this (float a, float b) v, float x) => v.a <= x && x <= v.b;
        public static bool ContainsExclusive(this (float a, float b) v, float x) => v.a < x && x < v.b;
        public static bool Contains(this (double a, double b) v, double x) => v.a <= x && x <= v.b;
        public static bool ContainsExclusive(this (double a, double b) v, double x) => v.a < x && x < v.b;
        
        
        public static int FloorToInt(this float x) => (int)Math.Floor(x);
        public static int FloorToInt(this double x) => (int)Math.Floor(x);
        public static int CeilToInt(this float x) => (int)Math.Ceiling(x);
        public static int CeilToInt(this double x) => (int)Math.Ceiling(x);
        public static int RoundToInt(this float x) => (int)Math.Round(x);
        public static int RoundToInt(this double x) => (int)Math.Round(x);
        
        public static ref double SetClamp(this ref double x, double a, double b)
        {
            x = (x < a ? a : x > b ? b : x);
            return ref x;
        }
        public static ref float SetClamp(this ref float x, float a, float b)
        {
            x = (x < a ? a : x > b ? b : x);
            return ref x;
        }
        public static ref long SetClamp(this ref long x, long a, long b)
        {
            x = (x < a ? a : x > b ? b : x);
            return ref x;
        }
        public static ref int SetClamp(this ref int x, int a, int b)
        {
            x = (x < a ? a : x > b ? b : x);
            return ref x;
        }
        
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
        
        // 梯形插值. trapzoid interpolation.
        public static float Terp(this float x, float start, float fullStart, float fullEnd, float end)
        {
            if (x < start) return 0;
            if (x > end) return 0;
            if(x < fullStart) return x.XMap(start, fullStart, 0, 1);
            if(x > fullEnd) return x.XMap(fullEnd, end, 1, 0);
            return 1;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public static int NextPowerOfTwo(this int x)
        {
            x |= x >> 16;
            x |= x >> 8;
            x |= x >> 4;
            x |= x >> 2;
            x |= x >> 1;
            return x + 1;
        }
        
        public static long NextPowerOfTwo(this long x)
        {
            x |= x >> 32;
            x |= x >> 16;
            x |= x >> 8;
            x |= x >> 4;
            x |= x >> 2;
            x |= x >> 1;
            return x + 1;
        }
        
        public static uint NextPowerOfTwo(this uint x)
        {
            x |= x >> 16;
            x |= x >> 8;
            x |= x >> 4;
            x |= x >> 2;
            x |= x >> 1;
            return x + 1;
        }
        
        public static ulong NextPowerOfTwo(this ulong x)
        {
            x |= x >> 32;
            x |= x >> 16;
            x |= x >> 8;
            x |= x >> 4;
            x |= x >> 2;
            x |= x >> 1;
            return x + 1;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public static bool IsPowerOfTwo(this int x) => unchecked(x & (x - 1)) == 0;
        public static bool IsPowerOfTwo(this long x) => unchecked(x & (x - 1)) == 0;
        public static bool IsPowerOfTwo(this uint x) => unchecked(x & (x - 1)) == 0;
        public static bool IsPowerOfTwo(this ulong x) => unchecked(x & (x - 1)) == 0;
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public static IEnumerable<(int f, int count)> Factorization(this int x)
        {
            for(int i = 2, sqrt = Math.Sqrt(x).FloorToInt(); i <= sqrt; i += 2)
            {
                int count = 0;
                for(; x % i == 0; count++, x /= i);
                if(count > 0) yield return (i, count);
            }
            if (x > 1) yield return (x, 1);          // x is a prime.
        }
        
        public static IEnumerable<(long f, int count)> Factorization(this long x)
        {
            for(long i = 2, sqrt = Math.Sqrt(x).FloorToInt(); i <= sqrt; i += 2)
            {
                int count = 0;
                for(; x % i == 0; count++, x /= i);
                if(count > 0) yield return (i, count);
            }
            if (x > 1) yield return (x, 1);          // x is a prime.
        }
        
        public static IEnumerable<(uint f, int count)> Factorization(this uint x)
        {
            for(uint i = 2, sqrt = (uint)Math.Sqrt(x); i <= sqrt; i += 2)
            {
                int count = 0;
                for(; x % i == 0; count++, x /= i);
                if(count > 0) yield return (i, count);
            }
            if (x > 1) yield return (x, 1);          // x is a prime.
        }
        
        public static IEnumerable<(ulong f, int count)> Factorization(this ulong x)
        {
            for(ulong i = 2, sqrt = (ulong)Math.Sqrt(x); i <= sqrt; i += 2)
            {
                int count = 0;
                for(; x % i == 0; count++, x /= i);
                if(count > 0) yield return (i, count);
            }
            if (x > 1) yield return (x, 1);          // x is a prime.
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        public static bool IsPrime(this int x)
        {
            if(x < 2) return false;
            for(int i = 2, sqrt = Math.Sqrt(x).FloorToInt(); i <= sqrt; i += 2) if(x % i == 0) return false;
            return true;
        }
        
        public static bool isPrime(this long x)
        {
            if(x < 2) return false;
            for(long i = 2, sqrt = Math.Sqrt(x).FloorToInt(); i <= sqrt; i += 2) if(x % i == 0) return false;
            return true;
        }
        
        public static bool IsPrime(this uint x)
        {
            if(x < 2) return false;
            for(uint i = 2, sqrt = (uint)Math.Sqrt(x); i <= sqrt; i += 2) if(x % i == 0) return false;
            return true;
        }
        
        public static bool IsPrime(this ulong x)
        {
            if(x < 2) return false;
            for(ulong i = 2, sqrt = (ulong)Math.Sqrt(x); i <= sqrt; i += 2) if(x % i == 0) return false;
            return true;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public static uint ToUInt(this int x) => unchecked((uint)x);
        public static int ToInt(this uint x) => unchecked((int)x);
        public static ulong ToULong(this long x) => unchecked((ulong)x);
        public static long ToLong(this ulong x) => unchecked((long)x);
        
    }
}
