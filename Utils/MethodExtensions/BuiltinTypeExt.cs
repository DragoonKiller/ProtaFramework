
using System;

#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

namespace Prota
{
    public static partial class BuiltinTypeExt
    {
        
        #region SetTo Span
        
        public static byte SetTo(this byte x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static sbyte SetTo(this sbyte x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static short SetTo(this short x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static ushort SetTo(this ushort x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static int SetTo(this int x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static uint SetTo(this uint x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static long SetTo(this long x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static ulong SetTo(this ulong x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static float SetTo(this float x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        public static double SetTo(this double x, Span<byte> target)
        {
            if(!BitConverter.TryWriteBytes(target, x)) throw new InvalidOperationException("out of range: " + target.Length);
            return x;
        }
        
        
        #if UNITY_2017_1_OR_NEWER
        public static Vector2 SetTo(this Vector2 x, Span<byte> target)
        {
            x.x.SetTo(target);
            x.y.SetTo(target.Slice(4));
            return x;
        }
        
        public static Vector3 SetTo(this Vector3 x, Span<byte> target)
        {
            x.x.SetTo(target);
            x.y.SetTo(target.Slice(4));
            x.z.SetTo(target.Slice(8));
            return x;
        }
        
        public static Vector4 SetTo(this Vector4 x, Span<byte> target)
        {
            x.x.SetTo(target);
            x.y.SetTo(target.Slice(4));
            x.z.SetTo(target.Slice(8));
            x.w.SetTo(target.Slice(12));
            return x;
        }
        
        public static Color SetTo(this Color x, Span<byte> target)
        {
            x.r.SetTo(target);
            x.g.SetTo(target.Slice(4));
            x.b.SetTo(target.Slice(8));
            x.a.SetTo(target.Slice(12));
            return x;
        }
        
        public static Color32 SetTo(this Color32 x, Span<byte> target)
        {
            x.r.SetTo(target);
            x.g.SetTo(target.Slice(1));
            x.b.SetTo(target.Slice(2));
            x.a.SetTo(target.Slice(3));
            return x;
        }
        
        #endif
        
        
        #endregion
        
        #region ReadFrom Span
        
        public static ref byte ReadFrom(this ref byte x, ReadOnlySpan<byte> target)
        {
            x = target[0];
            return ref x;
        }
        
        public static ref sbyte ReadFrom(this ref sbyte x, ReadOnlySpan<byte> target)
        {
            x = (sbyte)target[0];
            return ref x;
        }
        
        public static ref short ReadFrom(this ref short x, ReadOnlySpan<byte> target)
        {
            x = BitConverter.ToInt16(target);
            return ref x;
        }
        
        public static ref ushort ReadFrom(this ref ushort x, ReadOnlySpan<byte> target)
        {
            x = (ushort)BitConverter.ToInt16(target);
            return ref x;
        }
        
        public static ref int ReadFrom(this ref int x, ReadOnlySpan<byte> target)
        {
            x = BitConverter.ToInt32(target);
            return ref x;
        }
        
        public static ref uint ReadFrom(this ref uint x, ReadOnlySpan<byte> target)
        {
            x = (uint)BitConverter.ToInt32(target);
            return ref x;
        }
        
        public static ref long ReadFrom(this ref long x, ReadOnlySpan<byte> target)
        {
            x = BitConverter.ToInt64(target);
            return ref x;
        }
        
        public static ref ulong ReadFrom(this ref ulong x, ReadOnlySpan<byte> target)
        {
            x = (ulong)BitConverter.ToInt64(target);
            return ref x;
        }
        
        public static ref float ReadFrom(this ref float x, ReadOnlySpan<byte> target)
        {
            x = BitConverter.ToSingle(target);
            return ref x;
        }
        
        public static ref double ReadFrom(this ref double x, ReadOnlySpan<byte> target)
        {
            x = BitConverter.ToDouble(target);
            return ref x;
        }
        
        #if UNITY_2017_1_OR_NEWER
        public static ref Vector2 ReadFrom(this ref Vector2 x, Span<byte> target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom(target.Slice(4));
            return ref x;
        }
        
        public static ref Vector3 ReadFrom(this ref Vector3 x, Span<byte> target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom(target.Slice(4));
            x.z.ReadFrom(target.Slice(8));
            return ref x;
        }
        
        public static ref Vector4 ReadFrom(this ref Vector4 x, Span<byte> target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom(target.Slice(4));
            x.z.ReadFrom(target.Slice(8));
            x.w.ReadFrom(target.Slice(12));
            return ref x;
        }
        
        public static ref Color ReadFrom(this ref Color x, Span<byte> target)
        {
            x.r.ReadFrom(target);
            x.g.ReadFrom(target.Slice(4));
            x.b.ReadFrom(target.Slice(8));
            x.a.ReadFrom(target.Slice(12));
            return ref x;
        }
        
        public static ref Color32 ReadFrom(this ref Color32 x, Span<byte> target)
        {
            x.r.ReadFrom(target);
            x.g.ReadFrom(target.Slice(1));
            x.b.ReadFrom(target.Slice(2));
            x.a.ReadFrom(target.Slice(3));
            return ref x;
        }
        
        #endif
        
        #endregion
        
        #region SetTo IntPtr
        
        public static byte SetTo(this byte x, IntPtr target)
        {
            unsafe { *(byte*)target = x; }
            return x;
        }
        
        public static sbyte SetTo(this sbyte x, IntPtr target)
        {
            unsafe { *(sbyte*)target = x; }
            return x;
        }
        
        public static short SetTo(this short x, IntPtr target)
        {
            unsafe { *(short*)target = x; }
            return x;
        }
        
        public static ushort SetTo(this ushort x, IntPtr target)
        {
            unsafe { *(ushort*)target = x; }
            return x;
        }
        
        public static int SetTo(this int x, IntPtr target)
        {
            unsafe { *(int*)target = x; }
            return x;
        }
        
        public static uint SetTo(this uint x, IntPtr target)
        {
            unsafe { *(uint*)target = x; }
            return x;
        }
        
        public static long SetTo(this long x, IntPtr target)
        {
            unsafe { *(long*)target = x; }
            return x;
        }
        
        public static ulong SetTo(this ulong x, IntPtr target)
        {
            unsafe { *(ulong*)target = x; }
            return x;
        }
        
        public static float SetTo(this float x, IntPtr target)
        {
            unsafe { *(float*)target = x; }
            return x;
        }
        
        public static double SetTo(this double x, IntPtr target)
        {
            unsafe { *(double*)target = x; }
            return x;
        }
        
        #if UNITY_2017_1_OR_NEWER
        public static Vector2 SetTo(this Vector2 x, IntPtr target)
        {
            x.x.SetTo(target);
            x.y.SetTo(target + 4);
            return x;
        }
        
        public static Vector3 SetTo(this Vector3 x, IntPtr target)
        {
            x.x.SetTo(target);
            x.y.SetTo(target + 4);
            x.z.SetTo(target + 8);
            return x;
        }
        
        public static Vector4 SetTo(this Vector4 x, IntPtr target)
        {
            x.x.SetTo(target);
            x.y.SetTo(target + 4);
            x.z.SetTo(target + 8);
            x.w.SetTo(target + 12);
            return x;
        }
        
        public static Color SetTo(this Color x, IntPtr target)
        {
            x.r.SetTo(target);
            x.g.SetTo(target + 4);
            x.b.SetTo(target + 8);
            x.a.SetTo(target + 12);
            return x;
        }
        
        public static Color32 SetTo(this Color32 x, IntPtr target)
        {
            x.r.SetTo(target);
            x.g.SetTo(target + 1);
            x.b.SetTo(target + 2);
            x.a.SetTo(target + 3);
            return x;
        }
        
        #endif
        
        #endregion
        
        #region ReadFrom IntPtr
        
        public static ref byte ReadFrom(this ref byte x, IntPtr target)
        {
            unsafe { x = *(byte*)target; }
            return ref x;
        }
        
        public static ref sbyte ReadFrom(this ref sbyte x, IntPtr target)
        {
            unsafe { x = *(sbyte*)target; }
            return ref x;
        }
        
        public static ref short ReadFrom(this ref short x, IntPtr target)
        {
            unsafe { x = *(short*)target; }
            return ref x;
        }
        
        public static ref ushort ReadFrom(this ref ushort x, IntPtr target)
        {
            unsafe { x = *(ushort*)target; }
            return ref x;
        }
        
        public static ref int ReadFrom(this ref int x, IntPtr target)
        {
            unsafe { x = *(int*)target; }
            return ref x;
        }
        
        public static ref uint ReadFrom(this ref uint x, IntPtr target)
        {
            unsafe { x = *(uint*)target; }
            return ref x;
        }
        
        public static ref long ReadFrom(this ref long x, IntPtr target)
        {
            unsafe { x = *(long*)target; }
            return ref x;
        }
        
        public static ref ulong ReadFrom(this ref ulong x, IntPtr target)
        {
            unsafe { x = *(ulong*)target; }
            return ref x;
        }
        
        public static ref float ReadFrom(this ref float x, IntPtr target)
        {
            unsafe { x = *(float*)target; }
            return ref x;
        }
        
        public static ref double ReadFrom(this ref double x, IntPtr target)
        {
            unsafe { x = *(double*)target; }
            return ref x;
        }
        
        #if UNITY_2017_1_OR_NEWER
        public static unsafe ref Vector2 ReadFrom(this ref Vector2 x, IntPtr* target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom(target + 4);
            return ref x;
        }
        
        public static unsafe ref Vector3 ReadFrom(this ref Vector3 x, IntPtr* target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom(target + 4);
            x.z.ReadFrom(target + 8);
            return ref x;
        }
        
        public static unsafe ref Vector4 ReadFrom(this ref Vector4 x, IntPtr* target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom(target + 4);
            x.z.ReadFrom(target + 8);
            x.w.ReadFrom(target + 12);
            return ref x;
        }
        
        public static unsafe ref Color ReadFrom(this ref Color x, IntPtr* target)
        {
            x.r.ReadFrom(target);
            x.g.ReadFrom(target + 4);
            x.b.ReadFrom(target + 8);
            x.a.ReadFrom(target + 12);
            return ref x;
        }
        
        public static unsafe ref Color32 ReadFrom(this ref Color32 x, IntPtr target)
        {
            x.r.ReadFrom(target);
            x.g.ReadFrom(target + 1);
            x.b.ReadFrom(target + 2);
            x.a.ReadFrom(target + 3);
            return ref x;
        }
        
        #endif
        
        #endregion
        
        #region SetTo pointer
        
        
        public static unsafe byte SetTo(this byte x, void* target)
        {
            unsafe { *(byte*)target = x; }
            return x;
        }
        
        public static unsafe sbyte SetTo(this sbyte x, void* target)
        {
            unsafe { *(sbyte*)target = x; }
            return x;
        }
        
        public static unsafe short SetTo(this short x, void* target)
        {
            unsafe { *(short*)target = x; }
            return x;
        }
        
        public static unsafe ushort SetTo(this ushort x, void* target)
        {
            unsafe { *(ushort*)target = x; }
            return x;
        }
        
        public static unsafe int SetTo(this int x, void* target)
        {
            unsafe { *(int*)target = x; }
            return x;
        }
        
        public static unsafe uint SetTo(this uint x, void* target)
        {
            unsafe { *(uint*)target = x; }
            return x;
        }
        
        public static unsafe long SetTo(this long x, void* target)
        {
            unsafe { *(long*)target = x; }
            return x;
        }
        
        public static unsafe ulong SetTo(this ulong x, void* target)
        {
            unsafe { *(ulong*)target = x; }
            return x;
        }
        
        public static unsafe float SetTo(this float x, void* target)
        {
            unsafe { *(float*)target = x; }
            return x;
        }
        
        public static unsafe double SetTo(this double x, void* target)
        {
            unsafe { *(double*)target = x; }
            return x;
        }
        
        
        #if UNITY_2017_1_OR_NEWER
        public static unsafe Vector2 SetTo(this Vector2 x, void* target)
        {
            x.x.SetTo(target);
            x.y.SetTo((byte*)target + 4);
            return x;
        }
        
        public static unsafe Vector3 SetTo(this Vector3 x, void* target)
        {
            x.x.SetTo(target);
            x.y.SetTo((byte*)target + 4);
            x.z.SetTo((byte*)target + 8);
            return x;
        }
        
        public static unsafe Vector4 SetTo(this Vector4 x, void* target)
        {
            x.x.SetTo(target);
            x.y.SetTo((byte*)target + 4);
            x.z.SetTo((byte*)target + 8);
            x.w.SetTo((byte*)target + 12);
            return x;
        }
        
        public static unsafe Color SetTo(this Color x, void* target)
        {
            x.r.SetTo(target);
            x.g.SetTo((byte*)target + 4);
            x.b.SetTo((byte*)target + 8);
            x.a.SetTo((byte*)target + 12);
            return x;
        }
        
        public static unsafe Color32 SetTo(this Color32 x, void* target)
        {
            x.r.SetTo(target);
            x.g.SetTo((byte*)target + 1);
            x.b.SetTo((byte*)target + 2);
            x.a.SetTo((byte*)target + 3);
            return x;
        }
        
        #endif
        
        #endregion
        
        #region readfrom
        
        
        public static unsafe ref byte ReadFrom(this ref byte x, void* target)
        {
            unsafe { x = *(byte*)target; }
            return ref x;
        }
        
        public static unsafe ref sbyte ReadFrom(this ref sbyte x, void* target)
        {
            unsafe { x = *(sbyte*)target; }
            return ref x;
        }
        
        public static unsafe ref short ReadFrom(this ref short x, void* target)
        {
            unsafe { x = *(short*)target; }
            return ref x;
        }
        
        public static unsafe ref ushort ReadFrom(this ref ushort x, void* target)
        {
            unsafe { x = *(ushort*)target; }
            return ref x;
        }
        
        public static unsafe ref int ReadFrom(this ref int x, void* target)
        {
            unsafe { x = *(int*)target; }
            return ref x;
        }
        
        public static unsafe ref uint ReadFrom(this ref uint x, void* target)
        {
            unsafe { x = *(uint*)target; }
            return ref x;
        }
        
        public static unsafe ref long ReadFrom(this ref long x, void* target)
        {
            unsafe { x = *(long*)target; }
            return ref x;
        }
        
        public static unsafe ref ulong ReadFrom(this ref ulong x, void* target)
        {
            unsafe { x = *(ulong*)target; }
            return ref x;
        }
        
        public static unsafe ref float ReadFrom(this ref float x, void* target)
        {
            unsafe { x = *(float*)target; }
            return ref x;
        }
        
        public static unsafe ref double ReadFrom(this ref double x, void* target)
        {
            unsafe { x = *(double*)target; }
            return ref x;
        }
        
        #if UNITY_2017_1_OR_NEWER
        public static unsafe ref Vector2 ReadFrom(this ref Vector2 x, void* target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom((byte*)target + 4);
            return ref x;
        }
        
        public static unsafe ref Vector3 ReadFrom(this ref Vector3 x, void* target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom((byte*)target + 4);
            x.z.ReadFrom((byte*)target + 8);
            return ref x;
        }
        
        public static unsafe ref Vector4 ReadFrom(this ref Vector4 x, void* target)
        {
            x.x.ReadFrom(target);
            x.y.ReadFrom((byte*)target + 4);
            x.z.ReadFrom((byte*)target + 8);
            x.w.ReadFrom((byte*)target + 12);
            return ref x;
        }
        
        public static unsafe ref Color ReadFrom(this ref Color x, void* target)
        {
            x.r.ReadFrom(target);
            x.g.ReadFrom((byte*)target + 4);
            x.b.ReadFrom((byte*)target + 8);
            x.a.ReadFrom((byte*)target + 12);
            return ref x;
        }
        
        public static unsafe ref Color32 ReadFrom(this ref Color32 x, void* target)
        {
            x.r.ReadFrom(target);
            x.g.ReadFrom((byte*)target + 1);
            x.b.ReadFrom((byte*)target + 2);
            x.a.ReadFrom((byte*)target + 3);
            return ref x;
        }
        
        #endif
        
        #endregion
        
        
    }
}