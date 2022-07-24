using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Prota.Procedural
{
    public partial class ProceduralTexture
    {
        static Texture2D Grid(int w, int h)
        {
            w += 1;
            h += 1;
            var res = new Texture2D(w, h, TextureFormat.RGBA32, 0, true);
            var data = new Color32[w * h];
            for(int i = 0; i < h; i++) for(int j = 0; j < w; j++)
            {
                var b = i * w + j;
                var ia = i % 32;
                var ib = j % 32;
                data[b] = ia == 0 || ib == 0 || ia == 1 || ib == 1 || ia == 31 || ib == 31
                    ? new Color32(0xFF, 0xFF, 0xFF, 0xFF)
                    : new Color32(0, 0, 0, 0xFF);
                    
            }
            res.SetPixelData(data, 0);
            res.filterMode = FilterMode.Point;
            res.Apply();
            return res;
        }
    }
}
