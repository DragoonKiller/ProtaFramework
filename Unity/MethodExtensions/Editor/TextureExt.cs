using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prota.Unity;

namespace Prota.Editor
{
    public static class TextureExt
    {
        public static bool Same(this Texture2D a, Texture2D b)
        {
            if(a == b) return true;
            var w = a.width;
            var h = a.height;
            if(w != b.width) return false;
            if(h != b.height) return false;
            var adata = a.GetRawTextureData();
            var bdata = b.GetRawTextureData();
            if(adata.Length != bdata.Length) return false;
            for(int i = 0; i < adata.Length; i++) if(adata[i] != bdata[i]) return false;
            return true;
        }
        
        public static bool AlmostSame(this Texture2D a, Texture2D b)
        {
            if(Same(a, b)) return true;
            var acs = a.GetPixels(0, 0, a.width, a.height, 0);
            var bcs = b.GetPixels(0, 0, b.width, b.height, 0);
            Debug.Assert(acs.Length == bcs.Length);
            var totalDiff = 0.0f;
            for(int i = 0; i < acs.Length; i++)
            {
                var ac = acs[i];
                var bc = bcs[i];
                var diff = (ac.a - bc.a).Abs() + (ac.r - bc.r).Abs() + (ac.g - bc.g).Abs() + (ac.b - bc.b).Abs();
                if(diff > 1f) return false;                     // 两幅图像如果有一个像素差值过大, 那么判定为不一致.
                totalDiff += diff;
            }
            return totalDiff / (a.width * a.height) > 0.01;     // 两幅图像如果有 1% 内容不一致, 那么就判定为不一致.
        }
        
        
        public static List<(int x, int y, Color c1, Color c2)> Compare(this Texture2D a, Texture2D b)
        {
            if(a.width != b.width || a.height != b.height) return null;
            var res = new List<(int, int, Color, Color)>();
            var acs = a.GetPixels(0, 0, a.width, a.height, 0);
            var bcs = b.GetPixels(0, 0, b.width, b.height, 0);
            Debug.Assert(acs.Length == bcs.Length);
            for(int i = 0; i < acs.Length; i++)
            {
                var ac = acs[i].ToVec4();
                var bc = bcs[i].ToVec4();
                var d = ac - bc;
                if(d.magnitude >= 1e-9f) res.Add((i / acs.Length, i % acs.Length, ac, bc));
            }
            return res;
        }
    }
}
