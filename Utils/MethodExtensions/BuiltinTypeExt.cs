
using System;

namespace Prota
{
    public static partial class BuiltinTypeExt
    {
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
        
    }
}
