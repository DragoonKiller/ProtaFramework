using UnityEngine;
using Prota;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Prota.Unity;

namespace Prota.VisualEffect
{

    public class ProtDebugRenderFeature : ScriptableRendererFeature
    {
        public ProtaDebugRenderPass pass;
        
        public PowerOfTwoEnumByte stencilMask = PowerOfTwoEnumByte.All;
        
        public bool showDepth;
        
        public bool showStencil;
        
        public Material depthMaterial;
        
        public Material stencilMaterial;
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(renderingData.cameraData.cameraType != CameraType.Game) return;
            renderer.EnqueuePass(pass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if(renderingData.cameraData.cameraType != CameraType.Game) return;
            pass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }

        public override void Create()
        {
            pass = new ProtaDebugRenderPass(this);
            depthMaterial = new Material(Shader.Find("Hidden/Prota/DepthDebug"));
            stencilMaterial = new Material(Shader.Find("Hidden/Prota/StencilDebug"));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }




    [ExecuteAlways]
    public class ProtaDebugRenderPass : ScriptableRenderPass
    {
        ProtDebugRenderFeature feature = null;
        
        RTHandle targetHandle;
        RTHandle depthHandle;
        
        RenderTexture rt;
        
        public ProtaDebugRenderPass(ProtDebugRenderFeature x)
        {
            feature = x;
            this.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }
        
        public void SetTarget(RTHandle handle, RTHandle depthHandle)
        {
            targetHandle = handle;
            this.depthHandle = depthHandle;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            base.OnCameraSetup(cmd, ref renderingData);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            rt = RenderTexture.GetTemporary(cameraTextureDescriptor);
            rt.name = "ProtaDebugRenderPassRT";
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("ProtaDebugRenderPass");
            cmd.name = "ProtaDebugRenderPass";
            
            if(feature.showDepth)
            {
                cmd.Blit(depthHandle, rt, feature.depthMaterial);
                cmd.Blit(rt, targetHandle);
            }
            else if(feature.showStencil)
            {
                feature.stencilMaterial.SetFloat("_StencilReadMask", (int)feature.stencilMask);
                
                // 不能用 blit...?
                // https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.Blit.html
                // https://forum.unity.com/threads/blitting-with-stencil-values-from-previous-render-feature-pass.1452610/
                cmd.Blit(null, targetHandle, feature.stencilMaterial);
                
                Debug.LogError("Not yet implemented");
            }
            else
            {
                
            }
            
            
            context.ExecuteCommandBuffer(cmd);
            context.Submit();
            
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            RenderTexture.ReleaseTemporary(rt);
            rt = null;
        }

    }
}

