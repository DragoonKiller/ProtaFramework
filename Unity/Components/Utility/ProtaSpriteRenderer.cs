using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(RectTransform))]
    public class ProtaSpriteRenderer : MonoBehaviour
    {
        public static Material _cachedMaterial;
        public static Material cachedMaterial => _cachedMaterial ??= new Material(Shader.Find("Prota/Sprite"));
        
        MeshRenderer _meshRenderer;
        public MeshRenderer meshRenderer => _meshRenderer ??= GetComponent<MeshRenderer>();
        
        MeshFilter _meshFilter;
        public MeshFilter meshFilter => _meshFilter ??= GetComponent<MeshFilter>();
        
        RectTransform _rectTransform;
        public RectTransform rectTransform => _rectTransform ??= GetComponent<RectTransform>();
        
        public Rect localRect => rectTransform.rect;
        
        // ====================================================================================================
        // ====================================================================================================
        
        [Header("Universal")]
        public Sprite sprite;
        public Sprite sprite2;
        public Sprite sprite3;
        public Sprite sprite4;
        
        
        [Header("Color")]
        [ColorUsage(true, true)] public Color color = Color.white;
        [Range(-1, 1)] public float hueOffset = 0;
        [Range(-1, 1)] public float brightnessOffset = 0;
        [Range(-1, 1)] public float saturationOffset = 0;
        [Range(-1, 1)] public float contrastOffset = 0;
        
        
        [Header("Render")]
        public int renderQueueOverride = -1;
        public int sortingLayer = 0;
        public int orderInLayer = 0;
        public uint renderLayerMask = 0;
        public bool flipX = false;
        public bool flipY = false;
        
        
        [Header("UVOffset")]
        public Vector2 uvOffsetByTime = Vector2.zero;
        public bool uvOffsetByRealtime = false;
        
        // ====================================================================================================
        // ====================================================================================================
        
        bool UseSprite2() => sprite != null;
        bool UseSprite3() => sprite != null && sprite2 != null;
        bool UseSprite4() => sprite != null && sprite2 != null && sprite3 != null;
        
        bool needUpdateMesh = false;
        
        bool needUpdateMaterial = false;
        
        // ====================================================================================================
        // ====================================================================================================
        
        [field: SerializeField, Readonly] public Mesh mesh { get; private set; }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnValidate()
        {
            needUpdateMesh = true;
            needUpdateMaterial = true;
        }
        
        void Update()
        {
            UpdateMesh();
            UpdateMaterial();
        }
        
        void UpdateMesh()
        {
            if(!needUpdateMesh) return;
            needUpdateMesh = false;
            
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
            mesh.SetTriangles(triangles, 0);
            
            using var ____ = TempList.Get<Sprite>(out var spriteList);
            GetSpritesAsList(spriteList);
            for(int i = 0; i < spriteList.Count; i++)
            {
                var uv = spriteList[i].uv;
                
                if(uv.Length != 4)
                {
                    Debug.LogError($"Sprite [{sprite.name}] needs to be [FullRect]. Packing mode[{sprite.packingMode}]");
                    continue;
                }
                mesh.SetUVs(i, uv);
            }
            
            mesh.RecalculateBounds();
            
            meshFilter.sharedMesh = mesh;
            meshRenderer.sortingOrder = orderInLayer;
            meshRenderer.sortingLayerID = sortingLayer;
            meshRenderer.renderingLayerMask = renderLayerMask;
        }
        
        
        
        static string[] textureNames = {
            "_MainTex",
            "_MainTex2",
            "_MainTex3",
            "_MainTex4",
        };
        
        void UpdateMaterial()
        {
            if(!needUpdateMaterial) return;
            needUpdateMaterial = false;
            
            if(meshRenderer.sharedMaterial == null)
            {
                meshRenderer.sharedMaterial = new Material(cachedMaterial);
            }
            
            var mat = meshRenderer.sharedMaterial;
            
            mat.renderQueue = renderQueueOverride >= 0 ? mat.renderQueue : renderQueueOverride;
            
            mat.SetTexture("_MainTex", sprite.texture);
            
            mat.SetColor("_Color", color);
            
            mat.SetFloat("_HueOffset", hueOffset);
            mat.SetFloat("_BrightnessOffset", brightnessOffset);
            mat.SetFloat("_SaturationOffset", saturationOffset);
            mat.SetFloat("_ContrastOffset", contrastOffset);
            
            mat.SetVector("_Flip", new Vector2(flipX ? 1 : 0, flipX ? 1 : 0));
            
            var t = uvOffsetByRealtime ? Time.realtimeSinceStartup : Time.time;
            mat.SetVector("_UVOffset", uvOffsetByTime * t);
            
            using var _ = TempList.Get<Sprite>(out var spriteList);
            GetSpritesAsList(spriteList);
            
            for(int i = 0; i < spriteList.Count; i++)
            {
                mat.SetTexture(textureNames[i], spriteList[i].texture);
            }
        }
        
        void GetSpritesAsList(List<Sprite> spriteList)
        {
            spriteList.Clear();
            if(sprite != null) spriteList.Add(sprite);
            if(UseSprite2() && sprite2 != null) spriteList.Add(sprite2);
            if(UseSprite3() && sprite3 != null) spriteList.Add(sprite3);
            if(UseSprite4() && sprite4 != null) spriteList.Add(sprite4);
            
        }
    }
}
