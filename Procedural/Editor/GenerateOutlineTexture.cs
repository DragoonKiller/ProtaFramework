using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota;
using Prota.Unity;
using System.Threading.Tasks;
using Unity.Collections;
using System.IO;
namespace Prota.Editor
{
    public static partial class GenerateTextureArgs
    {
        public static float outlineWidth
        {
            get => EditorPrefs.GetFloat("prota::outlineWidth", 2);
            set => EditorPrefs.SetFloat("prota::outlineWidth", value);
        }
        
        public static Color32 outlineColor
        {
            get => EditorPrefs.GetString("prota::outlineColor", "000000FF").ToColor();
            set => EditorPrefs.SetString("prota::outlineColor", ((Color)value).ToWebString());
        }
        
        public static float outlineAlphaWidth
        {
            get => EditorPrefs.GetFloat("prota::outlineAlphaWidth", 1);
            set => EditorPrefs.SetFloat("prota::outlineAlphaWidth", value);
        }
        
        // 大于等于这个值的像素才会被认为是有效像素, 用于去除透明边缘.
        public static float alphaThreshold
        {
            get => EditorPrefs.GetFloat("prota::alphaThreshold", 0.1f);
            set => EditorPrefs.SetFloat("prota::alphaThreshold", value);
        }
        
        public static List<Texture2D> GetCurrentSelectedTextures()
        {
            var tex = Selection.objects
                .Select(x => x as Texture2D)
                .Where(x => x != null && !AssetDatabase.GetAssetPath(x).NullOrEmpty())
                .ToList();
            return tex;
        }
    }
    
    public static partial class ProtaEditorCommands
    {
        public static void GenerateOutlineTexture(this List<Texture2D> tex)
        {
            var outlineWidth = GenerateTextureArgs.outlineWidth;
            var outlineColor = GenerateTextureArgs.outlineColor;
            var outlineAlphaWidth = GenerateTextureArgs.outlineAlphaWidth;
            var alphaThreshold = GenerateTextureArgs.alphaThreshold;
            
            var pathDict = tex.ToDictionary(x => x, x => AssetDatabase.GetAssetPath(x));
            var dataDict = tex.ToDictionary(x => x, x => x.GetPixelData<Color32>(0));
            var sizeDict = tex.ToDictionary<Texture2D, Texture2D, (int width, int height)>(x => x, x => (x.width, x.height));
            var isDataRGBDict = tex.ToDictionary(x => x, x => x.isDataSRGB);
            var newDataDict = new Dictionary<Texture2D, NativeArray<Color32>>();
            foreach(var t in tex) newDataDict.Add(t, new NativeArray<Color32>(dataDict[t], Allocator.Persistent));
            
            Parallel.ForEach(tex, t => {
                var path = pathDict[t];
                var size = sizeDict[t];
                var oriData = dataDict[t];
                var newData = newDataDict[t];
                var dataView = oriData.View2D(size.width, size.height);
                var newDataView = newData.View2D(size.width, size.height);
                Parallel.For(0, dataView.h, l => {
                    for(int c = 0; c < dataView.w; c++)
                    {
                        // 找到距离边界的最短距离.
                        float? distanceToVisible = null;
                        float? distanceToInvisible = null;
                        for(int dl = -outlineWidth.FloorToInt() - 1, dlx = outlineWidth.CeilToInt() + 1; dl <= dlx; dl++)
                        for(int dc = -outlineWidth.FloorToInt() - 1, dcx = outlineWidth.CeilToInt() + 1; dc <= dcx; dc++)
                        {
                            var curl = l + dl;
                            var curc = c + dc;
                            if(!dataView.Valid(curl, curc)) continue;
                            var curd = new Vector2(dl, dc).magnitude;
                            if(alphaThreshold * 255 > dataView[curl, curc].a)       // 不可见像素.
                            {
                                if(distanceToInvisible == null || curd < distanceToInvisible) distanceToInvisible = curd;
                            }
                            else        // 可见像素.
                            {
                                if(distanceToVisible == null || curd < distanceToVisible) distanceToVisible = curd;
                            }
                        }
                        
                        if(distanceToVisible == null || distanceToInvisible == null)
                        {
                            newDataView[l, c] = dataView[l, c];
                            continue;
                        }
                        
                        var distanceToEdge = Mathf.Max(distanceToVisible.Value + distanceToInvisible.Value);
                        
                        var outlineRatio = distanceToEdge.Terp(-1, 0, outlineWidth - outlineAlphaWidth, outlineWidth);
                        var srcColor = (Color)outlineColor;
                        srcColor.a *= outlineRatio;
                        var dstColor = (Color)dataView[l, c];
                        
                        // Normal blend mode.
                        var finalColor = srcColor * srcColor.a + dstColor * dstColor.a * (1 - srcColor.a);
                        var finalAlpha = dstColor.a + (1 - dstColor.a) * srcColor.a;
                        
                        newDataView[l, c] = (Color32)finalColor.WithA(finalAlpha);
                    }
                });
            });
            
            foreach(var t in tex)
            {
                var path = pathDict[t];
                var texFile = path.AsFileInfo();
                var newFile = texFile.Directory.SubFile("Outline." + texFile.Name);
                var newTex = new Texture2D(t.width, t.height, TextureFormat.RGBA32, 0, t.isDataSRGB, true);
                newTex.SetPixelData(newDataDict[t], 0);
                UnityEngine.Debug.Log($"{ texFile.FullName } => { newFile.FullName }");
                File.WriteAllBytes(newFile.FullName, newTex.EncodeToPNG());
            }
            
            foreach(var d in newDataDict) d.Value.Dispose();
            
            // AssetDatabase.Refresh();
        }
        
        [MenuItem("Assets/ProtaFramework/贴图/生成描边", false)]
        public static void GenerateOutlineTexture()
        {
            var tex = GenerateTextureArgs.GetCurrentSelectedTextures();
            
            if(tex.Count == 0)
            {
                Debug.Log("未选择贴图, 跳过.");
                return;
            }
            
            GenerateOutlineTexture(tex);
            
            AssetDatabase.Refresh();
            
        }
    }
    
}
