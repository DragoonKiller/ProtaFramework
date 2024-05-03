using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UI;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static Color SetColorWithoutA(this SpriteRenderer rd, Color color)
        {
            var c = rd.color;
            c.r = color.r;
            c.g = color.g;
            c.b = color.b;
            rd.material.color = c;
            return c;
        }
        
        public static Color SetColorWithoutA(this Image rd, Color color)
        {
            var c = rd.color;
            c.r = color.r;
            c.g = color.g;
            c.b = color.b;
            rd.color = c;
            return c;
        }
    }
}
