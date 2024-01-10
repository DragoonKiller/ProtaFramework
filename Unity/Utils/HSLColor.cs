using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Prota;
using Unity.Mathematics;
using UnityEngine;

namespace Prota.Unity
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct HSLColor
    {
        public float hue;
        
        public float saturation;
        
        public float lightness;
        
        public float h
        {
            get => hue;
            set => hue = value;
        }
        
        public float s
        {
            get => saturation;
            set => saturation = value;
        }
        
        public float l
        {
            get => lightness;
            set => lightness = value;
        }
        
        public HSLColor(float h, float s, float l)
        {
            hue = h;
            saturation = s;
            lightness = l;
        }
        
        public HSLColor(Color c)
        {
            RGB2HSL(c.r, c.g, c.b, out hue, out saturation, out lightness);
        }
        
        public HSLColor OffsetHue(float offset)
        {
            return new HSLColor((hue + offset).Repeat(1.0f), saturation, lightness);
        }
        
        public HSLColor OffsetSaturation(float offset)
        {
            var s = this.saturation;
            
            if(offset < 0) // 更接近0.
            {
                s *= 1 + offset;
            }
            else // 更接近1.
            {
                s = s + (1.0f - s) * offset;
            }
            s = s.Clamp(0.0f, 1.0f);
            
            return new HSLColor(hue, s, lightness);
        }
        
        public HSLColor OffsetLightness(float offset)
        {
            var l = this.lightness;
            if(offset < 0) // 更接近0.
            {
                l *= 1 + offset;
            }
            else // 更接近1.
            {
                l = l + (1.0f - l) * offset;
            }
            
            return new HSLColor(hue, saturation, lightness);
        }
        
        public HSLColor OffsetContrast(float offset)
        {
            var s = this.saturation;
            if(offset < 0) // 更接近0.5.
            {
                s = s + (0.5f - s) * -offset;
            }
            else  // 更接近0或1.
            {
                s = s + (1.0f - s) * offset;
                s = saturation.Clamp(0.0f, 1.0f);
            }
            
            return new HSLColor(hue, s, lightness);
        }
        
        
        
        public static explicit operator Color(HSLColor hsl)
        {
            HSL2RGB(hsl.hue, hsl.saturation, hsl.lightness, out float r, out float g, out float b);
            return new Color(r, g, b);
        }
        
        public static explicit operator HSLColor(Color c)
        {
            return new HSLColor(c);
        }
        
        public Color ToColor(float alpha = 1f)
        {
            HSL2RGB(hue, saturation, lightness, out float r, out float g, out float b);
            return new Color(r, g, b, alpha);
        }
        
        public override string ToString()
        {
            return $"HSL({hue}, {saturation}, {lightness})";
        }
        
        
        public static void HSL2RGB(float h, float s, float l, out float r, out float g, out float b)
        {
            h = h.Repeat(1.0f);
            s = s.Clamp(0.0f, 1.0f);
            l = l.Clamp(0.0f, 1.0f);
            
            float C = (1.0f - (2.0f * l - 1.0f).Abs()) * s;
            float X = C * (1.0f - ((h * 6.0f).Repeat(2.0f) - 1.0f)).Abs();
            float m = l - C / 2.0f;
            
            if (h < 1.0f / 6.0f)
            {
                r = C + m;
                g = X + m;
                b = m;
            }
            else if (h < 2.0f / 6.0f)
            {
                r = X + m;
                g = C + m;
                b = m;
            }
            else if (h < 3.0f / 6.0f)
            {
                r = m;
                g = C + m;
                b = X + m;
            }
            else if (h < 4.0f / 6.0f)
            {
                r = m;
                g = X + m;
                b = C + m;
            }
            else if (h < 5.0f / 6.0f)
            {
                r = X + m;
                g = m;
                b = C + m;
            }
            else if (h <= 1.0)
            {
                r = C + m;
                g = m;
                b = X + m;
            }
            else throw new Exception($"h=[{h}]");
        }
        
        public static void RGB2HSL(float r, float g, float b, out float h, out float s, out float l)
        {
            float M = r.Max(g).Max(b);
            float m = r.Min(g).Min(b);
            float C = M - m;
            
            if (C == 0.0f)
            {
                h = 0.0f;
            }
            else if (M == r)
            {
                h = ((g - b) / C).Repeat(6.0f);
            }
            else if (M == g)
            {
                h = ((b - r) / C + 2.0f);
            }
            else if (M == b)
            {
                h = ((r - g) / C + 4.0f);
            }
            else throw new Exception($"M=[{M}] r=[{r}] g=[{g}] b=[{b}]");
            h = h / 6.0f;
            
            l = (M + m) / 2.0f;
            
            if (C == 0.0f)
            {
                s = 0.0f;
            }
            else
            {
                s = C / (1.0f - (2.0f * l - 1.0f).Abs());
            }
        }
    }
    
    public static partial class UnityMethodExtensions
    {
        public static HSLColor ToHSL(this Color c)
        {
            return (HSLColor)c;
        }
    }
}
