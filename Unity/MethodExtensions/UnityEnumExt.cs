using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    [Flags]
    public enum PowerOfTwoEnum : uint
    {
        _0 = 1,
        _1 = 2,
        _2 = 4,
        _3 = 8,
        _4 = 16,
        _5 = 32,
        _6 = 64,
        _7 = 128,
        _8 = 256,
        _9 = 512,
        _10 = 1024,
        _11 = 2048,
        _12 = 4096,
        _13 = 8192,
        _14 = 16384,
        _15 = 32768,
        _16 = 65536,
        _17 = 131072,
        _18 = 262144,
        _19 = 524288,
        _20 = 1048576,
        _21 = 2097152,
        _22 = 4194304,
        _23 = 8388608,
        _24 = 16777216,
        _25 = 33554432,
        _26 = 67108864,
        _27 = 134217728,
        _28 = 268435456,
        _29 = 536870912,
        _30 = 1073741824,
        _31 = 2147483648,
        
        All = uint.MaxValue,
        None = 0,
    }
    
    [Flags]
    public enum PowerOfTwoEnumByte : byte
    {
        _0 = 1,
        _1 = 2,
        _2 = 4,
        _3 = 8,
        _4 = 16,
        _5 = 32,
        _6 = 64,
        _7 = 128,
        
        All = byte.MaxValue,
        None = 0,
    }
}
