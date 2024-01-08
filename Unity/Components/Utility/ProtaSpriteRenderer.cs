using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

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
        public static Material _cachedMaterial;
        public static Material cachedMaterial
            => _cachedMaterial == null ? _cachedMaterial = new Material(Shader.Find("Prota/Sprite")) : _cachedMaterial;
        
        public MeshRenderer meshRenderer { get; private set; }
        public MeshFilter meshFilter { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public Mesh mesh { get; private set; }
        public Material material { get; private set; }
        
        public Rect localRect => rectTransform.rect;
        
        // ====================================================================================================
        // ====================================================================================================
        
        [Header("Sprite")]
        public Sprite sprite;
        public Sprite normal;
        public Vector2 uvOffset = Vector2.zero;
        public bool uvOffsetByTime = false;
        [ShowWhen("usOffsetByTime")] public bool uvOffsetByRealtime = false;
        
        [Header("Mask")]
        public Sprite mask;
        public Vector2 maskUVOffset = Vector2.zero;
        public bool maskUVOffsetByTime = false;
        [ShowWhen("maskUVOffsetByTime")] public bool maskUVOffsetByRealtime = false;
        [ColorUsage(true, true)] public Color maskUsage = new Color(1, 1, 1, 1);
        
        [Header("Image")]
        public bool flipX = false;
        public bool flipY = false;
        public Vector2 flipVector => new Vector2(flipX ? -1 : 1, flipY ? -1 : 1);
        

        [Header("Color")]
        [ColorUsage(true, true)] public Color color = Color.white;
        [ColorUsage(true, true)] public Color addColor = new Color(0, 0, 0, 0);
        [ColorUsage(true, true)] public Color overlapColor = new Color(0, 0, 0, 0);
        [Range(-1, 1)] public float hueOffset = 0;
        [Range(-1, 1)] public float brightnessOffset = 0;
        [Range(-1, 1)] public float saturationOffset = 0;
        [Range(-1, 1)] public float contrastOffset = 0;
        
        [Header("Stencil")]
        public PowerOfTwoEnumByte stencilRef = 0;
        public PowerOfTwoEnumByte stencilReadMask = PowerOfTwoEnumByte.All;
        public PowerOfTwoEnumByte stencilWriteMask = PowerOfTwoEnumByte.All;
        public UnityEngine.Rendering.CompareFunction stencilCompare = UnityEngine.Rendering.CompareFunction.Always;
        public UnityEngine.Rendering.StencilOp stencilPass = UnityEngine.Rendering.StencilOp.Keep;
        
        [Header("Material")]
        
        public UnityEngine.Rendering.BlendMode srcBlendMode = UnityEngine.Rendering.BlendMode.SrcAlpha;
        public UnityEngine.Rendering.BlendMode dstBlendMode = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
        public UnityEngine.Rendering.CompareFunction depthTest = UnityEngine.Rendering.CompareFunction.Always;
        public OnOffEnum depthWrite = OnOffEnum.On;
        [Range(0, 1)] public float alphaClip = 0;
        
        
        [Header("Render")]
        public int renderQueueOverride = -1;
        public int sortingLayer = 0;
        public int orderInLayer = 0;
        public int renderLayerMask = -1;
        
        // ====================================================================================================
        // ====================================================================================================
        
        void Update()
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
        Sprite submittedSprite;
        Sprite submittedNormal;
        Sprite submittedMask;
        Vector2 submittedFlipVector;
        
        public bool NeedUpdateVertices()
        {
            if(submittedRect != localRect) return true;
            if(submittedSprite != sprite) return true;
            if(submittedNormal != normal) return true;
            if(submittedMask != mask) return true;
            if(submittedFlipVector != flipVector) return true;
            return false;
        }
        
        [ThreadStatic] static Vector3[] tempVertices;
        public void UpdateMeshVertices()
        {
            if(!NeedUpdateVertices()) return;
            
            var rect = localRect;
            
            if(tempVertices == null) tempVertices = new Vector3[4];
            
            // 顺序: 左上, 右上, 左下, 右下
            tempVertices[0] = new Vector3(rect.xMin, rect.yMax, 0);
            tempVertices[1] = new Vector3(rect.xMax, rect.yMax, 0);
            tempVertices[2] = new Vector3(rect.xMin, rect.yMin, 0);
            tempVertices[3] = new Vector3(rect.xMax, rect.yMin, 0);
            
            if(flipX) // 交换左右.
            {
                tempVertices[0].x = rect.xMax;
                tempVertices[1].x = rect.xMin;
                tempVertices[2].x = rect.xMax;
                tempVertices[3].x = rect.xMin;
            }
            
            if(flipY) // 交换上下.
            {
                tempVertices[0].y = rect.yMin;
                tempVertices[1].y = rect.yMin;
                tempVertices[2].y = rect.yMax;
                tempVertices[3].y = rect.yMax;
            }
            
            mesh.SetVertices(tempVertices);
            
            mesh.SetIndices(defaultTriangles, MeshTopology.Triangles, 0);
            
            static void SetUV(Mesh mesh, int layer, Sprite sprite)
            {
                if(sprite == null) mesh.SetUVs(layer, defaultUVs);
                else mesh.SetUVs(layer, sprite.uv);
            }
            
            SetUV(mesh, 0, sprite);
            // 1 = lighting uv.
            SetUV(mesh, 2, normal);
            SetUV(mesh, 3, mask);
            
            mesh.RecalculateBounds();
            
            submittedRect = rect;
            submittedSprite = sprite;
            submittedNormal = normal;
            submittedMask = mask;
            submittedFlipVector = flipVector;
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
            
            material.SetColor(Hashes._Color, color);
            material.SetColor(Hashes._AddColor, addColor);
            material.SetColor(Hashes._OverlapColor, overlapColor);
            
            material.SetColor(Hashes._MaskUsage, maskUsage);
            
            material.SetFloat(Hashes._HueOffset, hueOffset);
            material.SetFloat(Hashes._BrightnessOffset, brightnessOffset);
            material.SetFloat(Hashes._SaturationOffset, saturationOffset);
            material.SetFloat(Hashes._ContrastOffset, contrastOffset);
            
            material.SetInteger(Hashes._BlendSrc, (int)srcBlendMode);
            material.SetInteger(Hashes._BlendDst, (int)dstBlendMode);
            
            material.SetFloat(Hashes._ZTest, (int)depthTest);
            material.SetFloat(Hashes._ZWrite, (int)depthWrite);
            
            material.SetFloat(Hashes._AlphaClip, alphaClip);
            
            material.SetInteger(Hashes._StencilRef, unchecked((int)stencilRef));
            material.SetInteger(Hashes._StencilReadMask, unchecked((int)stencilReadMask));
            material.SetInteger(Hashes._StencilWriteMask, unchecked((int)stencilWriteMask));
            material.SetInteger(Hashes._StencilCompare, unchecked((int)stencilCompare));
            material.SetInteger(Hashes._StencilOp, unchecked((int)stencilPass));
            
            meshRenderer.sharedMaterial = material;
        }
        
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnEnable()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            rectTransform = GetComponent<RectTransform>();
            meshFilter.mesh = mesh = new Mesh() { name = "ProtaSpriteRenderer" };
            meshRenderer.material = material = new Material(cachedMaterial) { name = "ProtaSpriteRenderer" };
            
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.allowOcclusionWhenDynamic = false;
            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            
            submittedRect = Rect.zero;
        }
        
        void OnDisable()
        {
            if(mesh != null)
            {
                DestroyImmediate(mesh);
                meshFilter.mesh = null;
            }
            
            if(material != null)
            {
                DestroyImmediate(material);
                meshRenderer.material = null;
            }
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
    }
}
