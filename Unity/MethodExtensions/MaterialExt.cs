using System;
using UnityEngine;

namespace Prota
{
    public static partial class UnityMethodExtensions
    {
        public static Material SetMainTex(this Material mat, Texture texture)
        {
            mat.SetTexture("_MainTex", texture);
            return mat;
        }
    }
    
}