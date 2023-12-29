using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static RenderTexture Resize(this RenderTexture rt, int width, int height)
        {
            if(rt.width == width && rt.height == height) return rt;
            rt.Release();
            rt.width = width;
            rt.height = height;
            rt.Create();
            return rt;
        }
        
        public static RenderTexture Resize(this RenderTexture rt, Vector2Int size)
        {
            return rt.Resize(size.x, size.y);
        }
        
        public static Material CreateMaterialFromShader(this Shader shader)
        {
            var mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            return mat;
        }
        
        public static Material CreateMaterialFromShaderName(this string shaderName)
        {
            var shader = Shader.Find(shaderName);
            if(shader == null) throw new Exception($"Shader [{shaderName}] not found");
            return shader.CreateMaterialFromShader();
        }
        
        public static CullingResults Cull(this Camera camera, ScriptableRenderContext context, LayerMask? mask = null)
        {
            var cullingParams = new ScriptableCullingParameters();
            if (!camera.TryGetCullingParameters(out cullingParams)) return default;
            if(mask != null) cullingParams.cullingMask = mask.Value.value.ToUInt();
            return context.Cull(ref cullingParams);
        }
        
        public static RendererListDesc GetRenderListDesc(this Camera camera, ScriptableRenderContext context, string shaderTagId, LayerMask? mask = null)
        {
            var tagId = new ShaderTagId(shaderTagId);
            var cullResults = camera.Cull(context, mask);
            return new RendererListDesc(tagId, cullResults, camera) {
                renderQueueRange = RenderQueueRange.all,
                layerMask = mask ?? -1
            };
        }
        
        public static RendererListDesc GetRenderListDesc(this Camera camera, ScriptableRenderContext context, string shaderTagId, CullingResults cullResults, LayerMask? mask = null)
        {
            var tagId = new ShaderTagId(shaderTagId);
            return new RendererListDesc(tagId, cullResults, camera) {
                renderQueueRange = RenderQueueRange.all,
                layerMask = mask ?? -1
            };
        }
        
        public static void DrawRendererList(this CommandBuffer cmd, ScriptableRenderContext context, RendererListDesc listDesc)
        {
            var renderList = context.CreateRendererList(listDesc);
            cmd.DrawRendererList(renderList);
        }
        
    }
}
