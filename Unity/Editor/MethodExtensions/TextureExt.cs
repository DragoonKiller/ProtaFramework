using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prota.Unity;
using System.Threading.Tasks;
using System.Threading;
using System.Buffers;
using System;

namespace Prota.Editor
{
    public static partial class UnityMethodExtensions
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
            var sa = new Span<byte>(adata);
            var sb = new Span<byte>(bdata);
            var res = sa.SequenceEqual(sb);
            return res;
        }
        
        public static bool AlmostSame(this Texture2D a, Texture2D b)
        {
            if(Same(a, b)) return true;
            if(a.texelSize != b.texelSize) return false;
            var acs = a.GetPixels(0, 0, a.width, a.height, 0);
            var bcs = b.GetPixels(0, 0, b.width, b.height, 0);
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
            var acs = a.GetPixels32(0);
            var bcs = b.GetPixels32(0);
            Debug.Assert(acs.Length == bcs.Length);
            for(int i = 0; i < acs.Length; i++)
            {
                var ac = acs[i];
                var bc = bcs[i];
                var dist = (ac, bc).Diff();
                if(dist <= 3) res.Add((i / acs.Length, i % acs.Length, ac, bc));
            }
            return res;
        }
        
        // 边界提取. 以 alpha = 0 和 alpha != 0 为界. 返回一串边界的坐标点.
        // src: 输入图片. dst: 边界图片.
        public static List<Vector2> EdgeExtraction(this Texture2D src)
        {
            var res = new List<Vector2>();
            var s = src.GetPixels32(0);
            for(int i = 0; i < src.height; i++)
            for(int j = 0; j < src.width; j++)
            {
                var c = s[j + i * src.width];
                if(i != 0)
                {
                    var d = s[j + (i - 1) * src.width];
                    if((c.a == 0) != (d.a == 0)) res.Add(new Vector2(i - 0.5f, j));
                }
                
                if(j != 0)
                {
                    var d = s[j - 1 + i * src.width];
                    if((c.a == 0) != (d.a == 0)) res.Add(new Vector2(i, j - 0.5f));
                }
            }
            return res;
        }
        
        public static Texture2D ClearContent(this Texture2D t)
        {
            var c = new Color32[t.width * t.height];        // 全 0.
            t.SetPixels32(c, 0);
            return t;
        }
        
        public static Texture2D DrawPointsToTexture(this Texture2D t, List<Vector2> points)
        {
            foreach(var p in points)
            {
                var lb = p.FloorToInt();
                var rt = p.FloorToInt();
                var lt = new Vector2Int(lb.x, rt.y);
                var rb = new Vector2Int(rt.x, lb.y);
                var wl = 1 - (p.x - lb.x);
                var wr = p.x - lb.x;
                var wb = 1 - (p.y - lb.y);
                var wt = p.y - lb.y;
                var wlb = wl * wb;
                var wrt = wr * wt;
                var wlt = wl * wt;
                var wrb = wr * wb;
                UnityEngine.Debug.Assert((wlb + wrt + wlt + wrb - 1).Abs() <= 1e-6);
                t.SetPixel(lb.x, lb.y, new Color(1, 1, 1, wlb));
                t.SetPixel(rb.x, rb.y, new Color(1, 1, 1, wrb));
                t.SetPixel(lt.x, lt.y, new Color(1, 1, 1, wlt));
                t.SetPixel(rt.x, rt.y, new Color(1, 1, 1, wrt));
            }
            return t;
        }
        
        public static void Destroy(this Texture2D t)
        {
            UnityEngine.Object.Destroy(t);
        }
        
    }
}
