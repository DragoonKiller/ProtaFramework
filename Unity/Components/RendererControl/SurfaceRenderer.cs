using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Prota.Unity;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using UnityEditor;
using System.Runtime.InteropServices;

namespace Prota.Unity
{
    
    // 集成渲染贴图, 代替 Camera 渲染挂在这个节点里面的物体, 并且可以显示出来.
    // 支持迭代渲染, 可以用来做特殊效果.
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class SurfaceRenderer : MonoBehaviour
    {
        public RenderTexture renderTexture;
        
        public Color clearColor = Color.clear;
        
        public float zNear = -1000;
        public float zFar = 1000;
        
        public float pixelPerUnit = 16;
        
        public bool passRenderTextureToRenderer = false;
        
        public bool renderInScene = true;
        
        public Renderer[] renderers = Array.Empty<Renderer>();
        
        // ====================================================================================================
        // ====================================================================================================
        
        [NonSerialized] RenderTexture handledRenderTexture;
        [NonSerialized] MaterialPropertyBlock _props;
        MaterialPropertyBlock props => _props ?? (_props = new MaterialPropertyBlock());
        [NonSerialized] RenderTexture tempRenderTexture;
        [EditorButton] public bool fetchRenderers = false;
        
        CommandBuffer _cmd;
        CommandBuffer cmd => _cmd ?? (_cmd = new CommandBuffer() { name = "StandaloneRenderer" });
        
        static int _screenTexId;
        
        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            _screenTexId = Shader.PropertyToID("_ScreenTex");
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        void OnValidate()
        {
            if(zNear >= zFar) Debug.LogError("zNear should be less than zFar.", this);
        }
        
        void OnEnable()
        {
            RenderPipelineManager.beginContextRendering += this.OnRendering;
        }
        
        void OnDisable()
        {
            RenderPipelineManager.beginContextRendering -= this.OnRendering;
            
            ClearCmd();
            ClearTempRenderTexture();
            ClearHandledRenderTexture();
        }
        
        void ClearCmd()
        {
            if(_cmd != null)
            {
                _cmd.Dispose();
                _cmd = null;
            }
        }
        
        void ClearTempRenderTexture()
        {
            if(tempRenderTexture != null)
            {
                DestroyImmediate(tempRenderTexture);
                tempRenderTexture = null;
            }
        }
        
        void ClearHandledRenderTexture()
        {
            if(handledRenderTexture != null)
            {
                DestroyImmediate(handledRenderTexture);
                handledRenderTexture = null;
            }
        }
        
        void Update()
        {
            if(fetchRenderers.SwapSet(false))
            {
                renderers = this.GetComponentsInChildren<Renderer>().ToArray();
                foreach(var rd in renderers) rd.enabled = false;
            }
            if(renderInScene) Preview();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        void Preview()
        {
            var p = new RenderParams(ProtaUnityConstant.urpSpriteUnlitMat);
            p.matProps = new MaterialPropertyBlock();
            p.matProps.SetColor("_Color", Color.white);
            p.matProps.SetTexture("_MainTex", renderTexture);
            var localToWorld = this.RectTransform().ToWorldMatrix();
            Graphics.RenderMesh(p, ProtaUnityConstant.rectMesh, 0, localToWorld);
        }

        void OnRendering(ScriptableRenderContext context, List<Camera> cameras)
        {
            if(renderTexture != handledRenderTexture && handledRenderTexture != null)
            {
                ClearHandledRenderTexture();
            }
            
            if(!renderTexture)
            {
                handledRenderTexture = renderTexture = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);
                handledRenderTexture.name = "Generated Texture";
            }
            
            ResizeRenderTexture();
            PrpareTemporaryTexture();
            
            cmd.Clear();
            cmd.SetRenderTarget(renderTexture);
            cmd.ClearRenderTarget(true, true, clearColor);
            
            SetViewProj(cmd);
            
            if(passRenderTextureToRenderer)
            {
                foreach(var rd in renderers)
                {
                    // 只有需要 _ScreenTex 的材质才会接收到 renderTexture.
                    if(rd.sharedMaterial.HasProperty(_screenTexId))
                    {
                        rd.GetPropertyBlock(props);
                        props.SetTexture(_screenTexId, tempRenderTexture);
                        rd.SetPropertyBlock(props);
                        cmd.Blit(renderTexture, tempRenderTexture);
                    }
                    cmd.DrawRenderer(rd, rd.sharedMaterial, 0, 0);
                }
            }
            else
            {
                foreach(var rd in renderers)
                {
                    cmd.DrawRenderer(rd, rd.sharedMaterial, 0, 0);
                }
            }
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
        
        void SetViewProj(CommandBuffer cmd)
        {
            var rect = this.RectTransform().rect;
            var orthoViewMatrix = Matrix4x4.Ortho(rect.x, rect.x + rect.width, rect.y, rect.y + rect.height, zNear, zFar);
            cmd.SetViewProjectionMatrices(this.transform.worldToLocalMatrix, orthoViewMatrix);
        }
        
        void PrpareTemporaryTexture()
        {
            if(passRenderTextureToRenderer && tempRenderTexture == null)
            {
                tempRenderTexture = new RenderTexture(renderTexture.width, renderTexture.height, 0, renderTexture.format);
                tempRenderTexture.name = "StandaloneRenderer Temp";
            }
            
            if(!passRenderTextureToRenderer)
            {
                DestroyImmediate(tempRenderTexture);
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        void ResizeRenderTexture()
        {
            var expectedSize = (this.RectTransform().rect.size * pixelPerUnit).RoundToInt();
            if(renderTexture.texelSize == expectedSize) return;
            
            renderTexture.Release();
            renderTexture.width = expectedSize.x;
            renderTexture.height = expectedSize.y;
            renderTexture.Create();
            
            if(tempRenderTexture != null)
            {
                tempRenderTexture.Release();
                tempRenderTexture.width = expectedSize.x;
                tempRenderTexture.height = expectedSize.y;
                tempRenderTexture.Create();
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        

    }
}
