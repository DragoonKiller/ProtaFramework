using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.UI;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static Texture2D OrDefault(this Texture2D a)
        {
            if(!a) return Texture2D.whiteTexture;
            return a;
        }
        
        public static Texture OrDefault(this Texture a)
        {
            if(!a) return Texture2D.whiteTexture;
            return a;
        }
    }
}
