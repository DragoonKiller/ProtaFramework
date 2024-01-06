using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Prota.Unity
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(RectTransform))]
    public class ProtaSpriteRenderer : MonoBehaviour
    {
        public static Material _cachedMaterial;
        public static Material cachedMaterial
            => _cachedMaterial == null ? _cachedMaterial = new Material(Shader.Find("Prota/Sprite")) : _cachedMaterial;
        
        MeshRenderer _meshRenderer;
        public MeshRenderer meshRenderer
            => _meshRenderer == null ? _meshRenderer = GetComponent<MeshRenderer>() : _meshRenderer;
        
        MeshFilter _meshFilter;
        public MeshFilter meshFilter
            => _meshFilter == null ? _meshFilter = GetComponent<MeshFilter>() : _meshFilter;
        
        RectTransform _rectTransform;
        public RectTransform rectTransform
            => _rectTransform == null ? _rectTransform = GetComponent<RectTransform>() : _rectTransform;
        
        public Rect localRect => rectTransform.rect;
        
        // ====================================================================================================
        // ====================================================================================================
        
        [Header("Sprite")]
        public Sprite sprite;
        
        
        [Header("Color")]
        [ColorUsage(true, true)] public Color color = Color.white;
        [ColorUsage(true, true)] public Color addColor = new Color(0, 0, 0, 0);
        [Range(-1, 1)] public float hueOffset = 0;
        [Range(-1, 1)] public float brightnessOffset = 0;
        [Range(-1, 1)] public float saturationOffset = 0;
        [Range(-1, 1)] public float contrastOffset = 0;
        
        
        [Header("UV")]
        public bool flipX = false;
        public bool flipY = false;
        public Vector2 uvOffset = Vector2.zero;
        public bool uvOffsetByTime = false;
        public bool uvOffsetByRealtime = false;
        
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
        
        [field: Header("Record"), SerializeField, Readonly]
        public Rect recordTransformRect { get; private set; }
        [field: SerializeField, Readonly] public Vector2[] spriteUVCache { get; private set; }
        [field: SerializeField, Readonly] public Sprite recordSprite { get; private set; }
        [field: SerializeField, Readonly] public Vector2 recordFlip { get; private set; }
        [field: SerializeField, Readonly] public Vector2 recordUVOffset { get; private set; }
        [field: SerializeField, Readonly] public bool recordUVOffsetByTime { get; private set; }
        [field: SerializeField, Readonly] public bool recordUVOffsetByRealtime { get; private set; }
        
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        [field: SerializeField, Readonly] public Mesh mesh { get; private set; }
        [field: SerializeField, Readonly] public Material material { get; private set; }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnValidate()
        {
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.allowOcclusionWhenDynamic = false;
            meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        }
        
        void Update()
        {
            UpdateMesh();
            UpdateRendererProperties();
            UpdateMaterial();
            UpdateUV();
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
        
        
        bool NeedUpdateMesh()
        {
            if(recordTransformRect != localRect) return true;
            if(mesh == null) return true;
            if(recordSprite != sprite) return true;
            return false;
        }
        
        void UpdateMesh()
        {
            if(!NeedUpdateMesh()) return;
            
            if(mesh != null)
            {
                DestroyImmediate(mesh);
                mesh = null;
            }
            
            meshRenderer.sharedMaterial = null;
            (mesh == null).Assert();
            
            mesh = new Mesh();
            mesh.name = $"GeneratedMesh";
            
            using var _ = TempList.Get<Vector3>(out var vertices);
            using var __ = TempList.Get<int>(out var triangles);

            var rect = localRect;
            
            // 顺序: 左上, 右上, 左下, 右下
            vertices.Add(new Vector3(rect.xMin, rect.yMax, 0));
            vertices.Add(new Vector3(rect.xMax, rect.yMax, 0));
            vertices.Add(new Vector3(rect.xMin, rect.yMin, 0));
            vertices.Add(new Vector3(rect.xMax, rect.yMin, 0));
            
            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);
            triangles.Add(2);
            triangles.Add(1);
            triangles.Add(3);
            
            mesh.SetVertices(vertices);
            mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
            
            if(sprite == null)
            {
                mesh.SetUVs(0, defaultUVs);
            }
            else
            {
                if(sprite.uv.Length != 4)
                {
                    Debug.LogError($"Sprite [{sprite.name}] needs to be [FullRect]. Packing mode[{sprite.packingMode}]");
                }
                mesh.SetUVs(0, sprite.uv);
            }
            
            mesh.RecalculateBounds();
            
            meshFilter.sharedMesh = mesh;
            
            recordTransformRect = localRect;
            recordSprite = sprite;
            spriteUVCache = sprite == null ? defaultUVs : sprite.uv;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        bool NeedUpdateUV()
        {
            if(uvOffsetByTime) return true;
            if(recordFlip.x != (flipX ? -1 : 1)) return true;
            if(recordFlip.y != (flipY ? -1 : 1)) return true;
            if(recordUVOffset != uvOffset) return true;
            return false;
        }
        
        [ThreadStatic] static Vector2[] tempUVs = new Vector2[4];
        void UpdateUV()
        {
            if(!NeedUpdateUV()) return;
            
            SetWithSprite();
            
            recordFlip = new Vector2(flipX ? -1 : 1, flipY ? -1 : 1);
            recordUVOffset = uvOffset;
            recordUVOffsetByTime = uvOffsetByTime;
            recordUVOffsetByRealtime = uvOffsetByRealtime;
        }
    
        void SetWithSprite()
        {
            if(spriteUVCache.Length != 4)
            {
                Debug.LogError($"Sprite [{sprite.name}] needs to be [FullRect]. Packing mode[{sprite.packingMode}]");
                return;
            }
            
            var topLeftIndex = 0;
            var topRightIndex = 1;
            var bottomLeftIndex = 2;
            var bottomRightIndex = 3;
            if(flipX)
            {
                (topLeftIndex, topRightIndex) = (topRightIndex, topLeftIndex);
                (bottomLeftIndex, bottomRightIndex) = (bottomRightIndex, bottomLeftIndex);
            }
            if(flipY)
            {
                (topLeftIndex, bottomLeftIndex) = (bottomLeftIndex, topLeftIndex);
                (topRightIndex, bottomRightIndex) = (bottomRightIndex, topRightIndex);
            }
            
            tempUVs[0] = spriteUVCache[topLeftIndex];
            tempUVs[1] = spriteUVCache[topRightIndex];
            tempUVs[2] = spriteUVCache[bottomLeftIndex];
            tempUVs[3] = spriteUVCache[bottomRightIndex];
            
            if(uvOffsetByTime)
            {
                var t = Time.time;
                for(var i = 0; i < tempUVs.Length; i++) tempUVs[i] += uvOffset * t;
            }
            else
            {
                for(var i = 0; i < tempUVs.Length; i++) tempUVs[i] += uvOffset;
            }
            
            mesh.SetUVs(0, tempUVs);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        private static class Hashes
        {
            public static int _MainTex;
            public static int _Color;
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
                _Color = Shader.PropertyToID("_Color");
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
            if(material == null)
            {
                material = new Material(cachedMaterial) {
                    name = $"GeneratedMaterial",
                };
            }
            
            material.renderQueue = renderQueueOverride >= 0 ? material.renderQueue : renderQueueOverride;
            
            if(sprite != null) material.SetTexture(Hashes._MainTex, sprite.texture);
            
            material.SetColor(Hashes._Color, color);
            
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
        
        void OnDestroy()
        {
            if(mesh != null)
            {
                DestroyImmediate(mesh);
                mesh = null;
            }
            
            if(material != null)
            {
                DestroyImmediate(material);
                material = null;
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
