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
        public Vector4 maskUsage = new Vector4(1, 1, 1, 1);
        
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
            UpdateSpriteUV();
            UpdatMaskUV();
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
        
        public bool NeedUpdateVertices()
        {
            if(submittedRect != localRect) return true;
            return false;
        }
        
        [ThreadStatic] static Vector3[] tempVertices;
        [ThreadStatic] static int[] tempTriangles;
        public void UpdateMeshVertices()
        {
            if(!NeedUpdateVertices()) return;
            
            var rect = localRect;
            
            if(tempVertices == null) tempVertices = new Vector3[4];
            if(tempTriangles == null) tempTriangles = new int[6];
            
            // 顺序: 左上, 右上, 左下, 右下
            tempVertices[0] = new Vector3(rect.xMin, rect.yMax, 0);
            tempVertices[1] = new Vector3(rect.xMax, rect.yMax, 0);
            tempVertices[2] = new Vector3(rect.xMin, rect.yMin, 0);
            tempVertices[3] = new Vector3(rect.xMax, rect.yMin, 0);
            mesh.SetVertices(tempVertices);
            
            tempTriangles[0] = 0;
            tempTriangles[1] = 1;
            tempTriangles[2] = 2;
            tempTriangles[3] = 2;
            tempTriangles[4] = 1;
            tempTriangles[5] = 3;
            mesh.SetIndices(tempTriangles, MeshTopology.Triangles, 0);
            
            mesh.RecalculateBounds();
            
            submittedRect = rect;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        Sprite submittedSprite;
        Vector2 submittedSpriteFlip;
        Vector2 submittedSpriteUVOffset;
        bool submittedSpriteUVOffsetByTime;
        bool submittedSpriteUVOffsetByRealtime;
        Vector2[] submittedSpriteUV;
        
        public void UpdateSpriteUV()
        {
            SetUV(mesh,
                sprite, ref submittedSprite, ref submittedSpriteUV,
                0,
                flipVector, ref submittedSpriteFlip,
                uvOffset, ref submittedSpriteUVOffset,
                uvOffsetByTime, ref submittedSpriteUVOffsetByTime,
                uvOffsetByRealtime, ref submittedSpriteUVOffsetByRealtime
            );
        }
        Sprite submittedMask;
        Vector2 submittedMaskFlip;
        Vector2 submittedMaskUVOffset;
        bool submittedMaskUVOffsetByTime;
        bool submittedMaskUVOffsetByRealtime;
        Vector2[] submittedMaskUV;
        
        public void UpdatMaskUV()
        {
            SetUV(mesh,
                mask, ref submittedMask, ref submittedMaskUV,
                3,
                flipVector, ref submittedMaskFlip,
                maskUVOffset, ref submittedMaskUVOffset,
                maskUVOffsetByTime, ref submittedMaskUVOffsetByTime,
                maskUVOffsetByRealtime, ref submittedMaskUVOffsetByRealtime
            );
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
            public static int _MaskTex;
            public static int _NormalTex;
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
            
            
            [InitializeOnLoadMethod]
            [RuntimeInitializeOnLoadMethod]
            static void Init()
            {
                _MainTex = Shader.PropertyToID("_MainTex");
                _MaskTex = Shader.PropertyToID("_MaskTex");
                _NormalTex = Shader.PropertyToID("_NormalTex");
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
            }
        }
        
        void UpdateMaterial()
        {
            static void SetTexture(Material mat, int hash, Sprite sprite)
            {
                var texture = sprite == null ? null : sprite.texture;
                if(texture == null) mat.SetTexture(hash, null);
                else mat.SetTexture(hash, texture);
            }
            
            material.renderQueue = renderQueueOverride >= 0 ? material.renderQueue : renderQueueOverride;
            
            
            SetTexture(material, Hashes._MainTex, sprite);
            SetTexture(material, Hashes._NormalTex, normal);
            SetTexture(material, Hashes._MaskTex, mask);
            
            material.SetColor(Hashes._Color, color);
            material.SetColor(Hashes._AddColor, addColor);
            material.SetColor(Hashes._OverlapColor, overlapColor);
            
            material.SetVector(Hashes._MaskUsage, maskUsage);
            
            material.SetFloat(Hashes._HueOffset, hueOffset);
            material.SetFloat(Hashes._BrightnessOffset, brightnessOffset);
            material.SetFloat(Hashes._SaturationOffset, saturationOffset);
            material.SetFloat(Hashes._ContrastOffset, contrastOffset);
            
            material.SetFloat(Hashes._BlendSrc, (int)srcBlendMode);
            material.SetFloat(Hashes._BlendDst, (int)dstBlendMode);
            
            material.SetFloat(Hashes._ZTest, (int)depthTest);
            material.SetFloat(Hashes._ZWrite, (int)depthWrite);
            
            material.SetFloat(Hashes._AlphaClip, alphaClip);
            
            meshRenderer.sharedMaterial = material;
        }
        
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        [ThreadStatic] static Vector2[] tempUVs;
        static void SetUV(Mesh mesh, Sprite sprite, ref Sprite recordSprite,
            ref Vector2[] uvCache,
            int uvlayer,
            Vector2 flipVector, ref Vector2 recordFlipVector,
            Vector2 uvOffset, ref Vector2 recordUVOffset,
            bool uvOffsetByTime, ref bool recordUVOffsetByTime,
            bool uvOffsetByRealtime, ref bool recordUVOffsetByRealtime)
        {
            if(mesh == null || sprite == null) return;
            
            bool shouldUpdateUV = false;
            if(!shouldUpdateUV && sprite != recordSprite) shouldUpdateUV = true;
            if(!shouldUpdateUV && flipVector != recordFlipVector) shouldUpdateUV = true;
            if(!shouldUpdateUV && uvOffset != recordUVOffset) shouldUpdateUV = true;
            if(!shouldUpdateUV && uvOffsetByTime) shouldUpdateUV = true;
            if(!shouldUpdateUV) return;
            
            if(tempUVs == null) tempUVs = new Vector2[4];
            if(recordSprite != sprite) uvCache = sprite.uv;
            recordSprite = sprite;
            recordFlipVector = flipVector;
            recordUVOffset = uvOffset;
            recordUVOffsetByTime = uvOffsetByTime;
            recordUVOffsetByRealtime = uvOffsetByRealtime;
            
            var topLeftIndex = 0;
            var topRightIndex = 1;
            var bottomLeftIndex = 2;
            var bottomRightIndex = 3;
            if(flipVector.x < 0)
            {
                (topLeftIndex, topRightIndex) = (topRightIndex, topLeftIndex);
                (bottomLeftIndex, bottomRightIndex) = (bottomRightIndex, bottomLeftIndex);
            }
            if(flipVector.y < 0)
            {
                (topLeftIndex, bottomLeftIndex) = (bottomLeftIndex, topLeftIndex);
                (topRightIndex, bottomRightIndex) = (bottomRightIndex, topRightIndex);
            }
            
            tempUVs[0] = uvCache[topLeftIndex];
            tempUVs[1] = uvCache[topRightIndex];
            tempUVs[2] = uvCache[bottomLeftIndex];
            tempUVs[3] = uvCache[bottomRightIndex];
            
            if(uvOffsetByTime)
            {
                var t = uvOffsetByRealtime ? Time.realtimeSinceStartup : Time.time;
                for(var i = 0; i < tempUVs.Length; i++) tempUVs[i] += uvOffset * t;
            }
            else
            {
                for(var i = 0; i < tempUVs.Length; i++) tempUVs[i] += uvOffset;
            }
            
            mesh.SetUVs(uvlayer, tempUVs);
        }
        
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
            submittedSprite = null;
            submittedMask = null;
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
        
        
        static Vector2[] defaultUVs = new Vector2[] {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
        };
    }
}
