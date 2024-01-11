using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Prota;
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
            if(s == 0)
            {
                r = g = b = l;
            }
            else
            {
                float q = l < 0.5f ? l * (1.0f + s) : l + s - l * s;
                float p = 2.0f * l - q;
                r = Hue2RGB(p, q, h + 1.0f / 3.0f);
                g = Hue2RGB(p, q, h);
                b = Hue2RGB(p, q, h - 1.0f / 3.0f);
            }
        }
        
        static float Hue2RGB(float p, float q, float t)
        {
            if(t < 0.0f)
            {
                t += 1.0f;
            }
            else if(t > 1.0f)
            {
                t -= 1.0f;
            }
            
            if(t < 1.0f / 6.0f)
            {
                return p + (q - p) * 6.0f * t;
            }
            else if(t < 1.0f / 2.0f)
            {
                return q;
            }
            else if(t < 2.0f / 3.0f)
            {
                return p + (q - p) * (2.0f / 3.0f - t) * 6.0f;
            }
            else
            {
                return p;
            }
        }
        
        public static void RGB2HSL(float r, float g, float b, out float h, out float s, out float l)
        {
            float max = Mathf.Max(r, g, b);
            float min = Mathf.Min(r, g, b);
            float delta = max - min;
            
            l = (max + min) / 2.0f;
            
            if (delta == 0.0f)
            {
                h = 0.0f;
                s = 0.0f;
            }
            else
            {
                s = delta / (1.0f - (2.0f * l - 1.0f).Abs());
                
                if (r == max)
                {
                    h = (g - b) / delta;
                }
                else if (g == max)
                {
                    h = 2.0f + (b - r) / delta;
                }
                else
                {
                    h = 4.0f + (r - g) / delta;
                }
                
                h /= 6.0f;
                if (h < 0.0f)
                {
                    h += 1.0f;
                }
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
