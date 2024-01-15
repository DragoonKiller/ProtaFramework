using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.Rendering;
using Mono.Cecil;

namespace Prota.Unity
{
    /*
    Mesh 由三个部分组成
    1. 顶点, 由 RectTransform 定义
    2. texture uv, 由 sprite 和 flip 和 uvOffset 定义
    3. mask uv, 由 maskSprite 和 flip 和 uvOffset 定义
    */
    
    
    // 自定义 SpriteRenderer.
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(RectTransform))]
    public class ProtaSpriteRenderer : MonoBehaviour
    {
        static Shader _spriteShader;
        static LocalKeyword keywordUseLight;
        
        static Material _cachedMaterial;
        static Material cachedMaterial
        {
            get
            {
                if(_cachedMaterial == null)
                {
                    _cachedMaterial = new Material(shader);
                    _cachedMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return _cachedMaterial;
            }
        }
        
        static Shader shader
        {
            get
            {
                if(_spriteShader == null)
                {
                    _spriteShader = Shader.Find("Prota/Sprite");
                    if(_spriteShader == null) throw new Exception("Shader not found: Prota/Sprite");
                    keywordUseLight = new LocalKeyword(_spriteShader, "USE_LIGHT");
                }
                return _spriteShader;
            }
        }
        
        
        public MeshRenderer meshRenderer { get; private set; }
        public MeshFilter meshFilter { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public Mesh mesh { get; private set; }
        public Material material { get; private set; }
        
        public Rect localRect => rectTransform.rect;
        
        // ====================================================================================================
        // ====================================================================================================
        
        public Vector4 extend;  // (xmin-, ymin-, xmax+, ymax+)
        
        [Header("Sprite")]
        public bool render = true;
        public Sprite sprite;
        public Sprite normal;
        public Vector2 uvOffset = Vector2.zero;
        public bool uvOffsetByTime = false;
        [ShowWhen("uvOffsetByTime")] public bool uvOffsetByRealtime = false;
        
        public bool flipSpriteX;
        public bool flipSpriteY;
        public Vector2 spriteFlipVector => new Vector2(flipSpriteX ? -1 : 1, flipSpriteY ? -1 : 1);
        
        [Header("Mask")]
        public Sprite mask;
        [ShowWhen("mask")] public Vector2 maskUVOffset = Vector2.zero;
        [ShowWhen("mask")] public bool maskUVOffsetByTime = false;
        [ShowWhen("mask", "maskUVOffsetByTime")] public bool maskUVOffsetByRealtime = false;
        [ShowWhen("mask")] public bool flipMaskX = false;
        [ShowWhen("mask")] public bool flipMaskY = false;
        public Vector2 maskFlipVector => new Vector2(flipMaskX ? -1 : 1, flipMaskY ? -1 : 1);
        [ShowWhen("mask"), ColorUsage(true, true)] public Color maskUsage = new Color(1, 1, 1, 1);
        

        [Header("Color")]
        [ColorUsage(true, true)] public Color vertexColor = Color.white;
        [ColorUsage(true, true)] public Color color = Color.white;
        [ColorUsage(true, true)] public Color addColor = new Color(0, 0, 0, 0);
        [ColorUsage(true, true)] public Color overlapColor = new Color(0, 0, 0, 0);
        [Range(-1, 1)] public float hueOffset = 0;
        [Range(-1, 1)] public float brightnessOffset = 0;
        [Range(-1, 1)] public float saturationOffset = 0;
        [Range(-1, 1)] public float contrastOffset = 0;
        
        [Header("Stencil")]
        public bool useStencil = false;
        [ShowWhen("useStencil")] public PowerOfTwoEnumByte stencilRef = 0;
        [ShowWhen("useStencil")] public PowerOfTwoEnumByte stencilReadMask = PowerOfTwoEnumByte.All;
        [ShowWhen("useStencil")] public PowerOfTwoEnumByte stencilWriteMask = PowerOfTwoEnumByte.All;
        [ShowWhen("useStencil")] public CompareFunction stencilCompare = CompareFunction.Always;
        [ShowWhen("useStencil")] public StencilOp stencilPass = StencilOp.Keep;
        
        
        [Header("Material")]
        public bool useLight = true;
        public BlendMode srcBlendMode = BlendMode.SrcAlpha;
        public BlendMode dstBlendMode = BlendMode.OneMinusSrcAlpha;
        public CompareFunction depthTest = CompareFunction.Always;
        public OnOffEnum depthWrite = OnOffEnum.On;
        [Range(0, 1)] public float alphaClip = 0;
        
        
        [Header("Render")]
        public int renderQueueOverride = -1;
        public int sortingLayer = 0;
        public int orderInLayer = 0;
        public int renderLayerMask = -1;
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        void LateUpdate()
        {
            SyncSpriteInfoToRectTransform();
            UpdateMeshVertices();
            UpdateRendererProperties();
            UpdateMaterial();
        }

        [EditorButton] public bool syncSpriteToRect; 
        void SyncSpriteInfoToRectTransform()
        {
            if(!syncSpriteToRect) return;
            syncSpriteToRect = false;
            
            if(sprite == null) return;
            
            var vertices = sprite.vertices;
            var rect = new Rect(
                (vertices[0] + vertices[3]) / 2,
                (vertices[3] - vertices[0]).Abs()
            );
            rectTransform.sizeDelta = rect.size;
            rectTransform.pivot = Vector2.one - rect.center / rect.size;
        }

        // ====================================================================================================
        // ====================================================================================================
        
        Rect submittedRect;
        Vector4 submittedExtend;
        Sprite submittedSprite;
        Sprite submittedNormal;
        Sprite submittedMask;
        Vector2 submittedFlipSpriteVector;
        Vector2 submittedFlipMaskVector;
        Color submittedVertexColor;
        
        public bool NeedUpdateVertices()
        {
            if(submittedRect != localRect) return true;
            if(submittedSprite != sprite) return true;
            if(submittedNormal != normal) return true;
            if(submittedMask != mask) return true;
            if(submittedFlipSpriteVector != spriteFlipVector) return true;
            if(submittedFlipMaskVector != maskFlipVector) return true;
            if(submittedExtend != extend) return true;
            if(submittedVertexColor != vertexColor) return true;
            return false;
        }
        
        public void UpdateMeshVertices()
        {
            if(!NeedUpdateVertices()) return;
            
            var localRect = this.localRect;
            
            // 计算扩展后的矩形.
            var rect = localRect;
            rect.xMin -= extend.x;
            rect.yMin -= extend.y;
            rect.xMax += extend.z;
            rect.yMax += extend.w;
            
            // 计算扩展后的 uv.
            var spriteUV = sprite ? sprite.uv : null;
            var normalUV = normal ? normal.uv : null;
            var maskUV = mask ? mask.uv : null;
            var localSize = localRect.size;
            if(sprite) TransformUVExtend(localSize, extend, spriteUV);
            if(normal) TransformUVExtend(localSize, extend, normalUV);
            if(mask) TransformUVExtend(localSize, extend, maskUV);
            
            var tempVertices = new Vector3[4];
            
            // 顺序: 左上, 右上, 左下, 右下. uv 同.
            tempVertices[0] = new Vector3(rect.xMin, rect.yMax, 0);
            tempVertices[1] = new Vector3(rect.xMax, rect.yMax, 0);
            tempVertices[2] = new Vector3(rect.xMin, rect.yMin, 0);
            tempVertices[3] = new Vector3(rect.xMax, rect.yMin, 0);
            
            var tempColors = new Color[4];
            tempColors[0] = vertexColor;
            tempColors[1] = vertexColor;
            tempColors[2] = vertexColor;
            tempColors[3] = vertexColor;
            
            if(sprite) FlipSpriteUV(spriteUV, flipSpriteX, flipSpriteY);
            if(normal) FlipSpriteUV(normalUV, flipSpriteX, flipSpriteY);
            if(mask) FlipSpriteUV(maskUV, flipMaskX, flipMaskY);
            
            mesh.SetVertices(tempVertices);
            mesh.SetColors(tempColors);
            mesh.SetIndices(defaultTriangles, MeshTopology.Triangles, 0);
            
            mesh.SetUVs(0, sprite ? spriteUV : defaultUVs);
            mesh.SetUVs(1, normal ? normalUV : defaultUVs);
            mesh.SetUVs(2, mask ? maskUV : defaultUVs);
            
            mesh.RecalculateBounds();
            
            submittedRect = rect;
            submittedExtend = extend;
            submittedSprite = sprite;
            submittedNormal = normal;
            submittedMask = mask;
            submittedFlipSpriteVector = spriteFlipVector;
            submittedFlipMaskVector = maskFlipVector;
            submittedVertexColor = vertexColor;
        }
        
        static void FlipSpriteUV(Vector2[] uv, bool flipX, bool flipY)
        {
            if(flipX)
            {
                Swap(ref uv[0], ref uv[1]);
                Swap(ref uv[2], ref uv[3]);
            }
            
            if(flipY)
            {
                Swap(ref uv[0], ref uv[2]);
                Swap(ref uv[1], ref uv[3]);
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        bool NeedUpdateRendererProperties()
        {
            return meshRenderer.sortingOrder != orderInLayer
                   || meshRenderer.sortingLayerID != sortingLayer
                   || meshRenderer.renderingLayerMask != renderLayerMask;
        }
        
        void UpdateRendererProperties()
        {
            if(!NeedUpdateRendererProperties()) return;
            meshRenderer.sortingOrder = orderInLayer;
            meshRenderer.sortingLayerID = sortingLayer;
            meshRenderer.renderingLayerMask = unchecked((uint)renderLayerMask);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        private static class Hashes
        {
            public static int _MainTex;
            public static int _MainTex_ST;
            public static int _MaskTex;
            public static int _MaskTex_ST;
            public static int _NormalTex;
            public static int _NormalTex_ST;
            public static int _Color;
            public static int _AddColor;
            public static int _OverlapColor;
            public static int _MaskUsage;
            public static int _HueOffset;
            public static int _BrightnessOffset;
            public static int _SaturationOffset;
            public static int _ContrastOffset;
            public static int _BlendSrc;
            public static int _BlendDst;
            public static int _ZTest;
            public static int _ZWrite;
            public static int _AlphaClip;
            public static int _StencilRef;
            public static int _StencilReadMask;
            public static int _StencilWriteMask;
            public static int _StencilCompare;
            public static int _StencilOp;
            
            
            [InitializeOnLoadMethod]
            [RuntimeInitializeOnLoadMethod]
            static void Init()
            {
                _MainTex = Shader.PropertyToID("_MainTex");
                _MainTex_ST = Shader.PropertyToID("_MainTex_ST");
                _MaskTex = Shader.PropertyToID("_MaskTex");
                _MaskTex_ST = Shader.PropertyToID("_MaskTex_ST");
                _NormalTex = Shader.PropertyToID("_NormalTex");
                _NormalTex_ST = Shader.PropertyToID("_NormalTex_ST");
                _Color = Shader.PropertyToID("_Color");
                _AddColor = Shader.PropertyToID("_AddColor");
                _OverlapColor = Shader.PropertyToID("_OverlapColor");
                _MaskUsage = Shader.PropertyToID("_MaskUsage");
                _HueOffset = Shader.PropertyToID("_HueOffset");
                _BrightnessOffset = Shader.PropertyToID("_BrightnessOffset");
                _SaturationOffset = Shader.PropertyToID("_SaturationOffset");
                _ContrastOffset = Shader.PropertyToID("_ContrastOffset");
                _BlendSrc = Shader.PropertyToID("_BlendSrc");
                _BlendDst = Shader.PropertyToID("_BlendDst");
                _ZTest = Shader.PropertyToID("_ZTest");
                _ZWrite = Shader.PropertyToID("_ZWrite");
                _AlphaClip = Shader.PropertyToID("_AlphaClip");
                _StencilRef = Shader.PropertyToID("_StencilRef");
                _StencilReadMask = Shader.PropertyToID("_StencilReadMask");
                _StencilWriteMask = Shader.PropertyToID("_StencilWriteMask");
                _StencilCompare = Shader.PropertyToID("_StencilCompare");
                _StencilOp = Shader.PropertyToID("_StencilOp");
            }
        }
        
        void UpdateMaterial()
        {
            static void SetTexture(Material mat, int hash, int sthash, Sprite sprite,
                Vector2 uvOffset, bool uvOffsetByTime, bool uvOffsetByRealtime)
            {
                static float UVOffsetMult(bool uvOffsetByTime, bool uvOffsetByRealtime)
                {
                    if(!uvOffsetByTime) return 1;
                    if(uvOffsetByRealtime) return Time.realtimeSinceStartup;
                    return Time.time;
                }
                
                var texture = sprite == null ? null : sprite.texture;
                if(texture == null)
                {
                    mat.SetTexture(hash, null);
                }
                else
                {
                    mat.SetTexture(hash, texture);
                    uvOffset *= UVOffsetMult(uvOffsetByTime, uvOffsetByRealtime);
                    mat.SetVector(sthash, new Vector4(1, 1, uvOffset.x, uvOffset.y));
                }
            }
            
            material.renderQueue = renderQueueOverride >= 0 ? renderQueueOverride : material.renderQueue;
            
            SetTexture(material, Hashes._MainTex, Hashes._MainTex_ST, sprite, uvOffset, uvOffsetByTime, uvOffsetByRealtime);
            SetTexture(material, Hashes._NormalTex, Hashes._NormalTex_ST, normal, uvOffset, uvOffsetByTime, uvOffsetByRealtime);
            SetTexture(material, Hashes._MaskTex, Hashes._MaskTex_ST, mask, maskUVOffset, maskUVOffsetByTime, maskUVOffsetByRealtime);
            
            material.SetKeyword(keywordUseLight, useLight);
            
            material.SetColor(Hashes._Color, color);
            material.SetColor(Hashes._AddColor, addColor);
            material.SetColor(Hashes._OverlapColor, overlapColor);
            
            material.SetColor(Hashes._MaskUsage, maskUsage);
            
            material.SetFloat(Hashes._HueOffset, hueOffset);
            material.SetFloat(Hashes._BrightnessOffset, brightnessOffset);
            material.SetFloat(Hashes._SaturationOffset, saturationOffset);
            material.SetFloat(Hashes._ContrastOffset, contrastOffset);
            
            material.SetFloat(Hashes._BlendSrc, (int)srcBlendMode);
            material.SetFloat(Hashes._BlendDst, (int)dstBlendMode);
            
            material.SetFloat(Hashes._ZTest, (int)depthTest);
            material.SetFloat(Hashes._ZWrite, (int)depthWrite);
            
            material.SetFloat(Hashes._AlphaClip, alphaClip);
            
            if(useStencil)
            {
                material.SetInteger(Hashes._StencilRef, unchecked((int)stencilRef));
                material.SetInteger(Hashes._StencilReadMask, unchecked((int)stencilReadMask));
                material.SetInteger(Hashes._StencilWriteMask, unchecked((int)stencilWriteMask));
                material.SetFloat(Hashes._StencilCompare, unchecked((int)stencilCompare));
                material.SetFloat(Hashes._StencilOp, unchecked((int)stencilPass));
            }
            else
            {
                material.SetInteger(Hashes._StencilRef, 0);
                material.SetInteger(Hashes._StencilReadMask, 255);
                material.SetInteger(Hashes._StencilWriteMask, 255);
                material.SetFloat(Hashes._StencilCompare, (int)CompareFunction.Always);
                material.SetFloat(Hashes._StencilOp, (int)StencilOp.Keep);
            }
        }
        
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnEnable()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            rectTransform = GetComponent<RectTransform>();
            meshFilter.mesh = mesh = new Mesh() { name = "ProtaSpriteRenderer" };
            meshRenderer.sharedMaterial = material = new Material(cachedMaterial) { name = "ProtaSpriteRenderer" };
            
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.allowOcclusionWhenDynamic = false;
            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            
            submittedRect = Rect.zero;
        }
        
        void OnDisable()
        {
            ClearMesh();
            ClearMaterial();
        }
        
        void ClearMesh()
        {
            DestroyImmediate(mesh);
            meshFilter.mesh = null;
            mesh = null;
        }
        
        void ClearMaterial()
        {
            DestroyImmediate(material);
            meshRenderer.material = null;
            material = null;
        }
        
        
        static readonly Vector2[] defaultUVs = new Vector2[] {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
        };
        
        static readonly int[] defaultTriangles = new int[] {
            0, 1, 2,
            2, 1, 3,
        };
        
        
        static void Swap<T>(ref T a, ref T b)
        {
            var t = a;
            a = b;
            b = t;
        }
        
        
        static void TransformUVExtend(Vector2 size, Vector4 extend, Vector2[] points)
        {
            // 顺序: 左上, 右上, 左下, 右下.
            var xmin = points[0].x;
            var xmax = points[1].x;
            var ymin = points[2].y;
            var ymax = points[0].y;
            
            var uvsize = new Vector2(xmax - xmin, ymax - ymin);
            
            var exmin = xmin - extend.x / size.x * uvsize.x;
            var exmax = xmax + extend.z / size.x * uvsize.x;
            var eymin = ymin - extend.y / size.y * uvsize.y;
            var eymax = ymax + extend.w / size.y * uvsize.y;
            
            points[0] = new Vector2(exmin, eymax);
            points[1] = new Vector2(exmax, eymax);
            points[2] = new Vector2(exmin, eymin);
            points[3] = new Vector2(exmax, eymin);
        }
            
    }
}
