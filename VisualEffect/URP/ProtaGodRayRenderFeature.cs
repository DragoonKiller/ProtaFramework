using UnityEngine;
using Prota;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Prota.Unity;

namespace Prota.VisualEffect
{

    public class ProtaGodRayRenderFeature : ScriptableRendererFeature
    {
        public float resolutionMult = 0.5f;
        
        public LayerMask maskForCullObjects;
        
        public LayerMask mask;
        
        public int iteration = 1;
        
        public ProtaGodRayRenderPass pass;
        
        [Readonly] public bool godLightSourceFound;
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(renderingData.cameraData.cameraType != CameraType.Game) return;
            renderer.EnqueuePass(pass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if(renderingData.cameraData.cameraType != CameraType.Game) return;
            pass.SetTarget(renderer.cameraColorTargetHandle);
        }

        public override void Create()
        {
            pass = new ProtaGodRayRenderPass(this);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }




    [ExecuteAlways]
    public class ProtaGodRayRenderPass : ScriptableRenderPass
    {
        ProtaGodRayRenderFeature feature = null;
        
        Material materialX;
        Material materialY;
        Material finalBlit;
        Material drawCullArea;
        Material drawCulled;
        Material drawRadialBlur;
        
        
        RenderTexture cull;
        RenderTexture swapA;
        RenderTexture swapB;
        
        RTHandle targetHandle;
        
        Vector2Int size => new Vector2Int(
            (Screen.width * feature.resolutionMult).CeilToInt(),
            (Screen.height * feature.resolutionMult).CeilToInt()
        );
            
        
        int iteration => feature.iteration * 2;
        
        public ProtaGodRayRenderPass(ProtaGodRayRenderFeature x)
        {
            feature = x;
            this.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }
        
        public void SetTarget(RTHandle handle)
        {
            targetHandle = handle;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            PrepareMaterial();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (iteration < 0) return;
            
            var godLight = GodLightSource.instance;
            feature.godLightSourceFound = godLight != null;
            if(godLight == null) return;
            
            var camera = renderingData.cameraData.camera;
            
            CreateSwapBuffer();
            
            // 画所有物件的遮挡贴图.
            using var cmd = new CommandBuffer() { name = "ProtaGaussianBlurRenderFeature" };
            var listDesc = camera.GetRenderListDesc(context, "Universal2D", feature.maskForCullObjects);
            listDesc.overrideMaterial = drawCullArea;
            cmd.SetRenderTarget(cull);
            cmd.ClearRenderTarget(true, true, Color.black.WithA(0));
            cmd.DrawRendererList(context, listDesc);
            
            // 画太阳.
            listDesc = camera.GetRenderListDesc(context, "Universal2D", feature.mask);
            listDesc.overrideMaterial = drawCulled;
            drawCulled.SetTexture("_Cull", cull);
            cmd.SetRenderTarget(swapB);
            cmd.ClearRenderTarget(true, true, Color.black.WithA(0));
            cmd.DrawRendererList(context, listDesc);
            
            // 画径向模糊.
            var godLightScreenPos = camera.WorldToScreenPoint(godLight.worldPos);
            var rangedRefScreenPos = camera.WorldToScreenPoint(godLight.radiusRefPos);
            var radiusInScreen = (godLightScreenPos - rangedRefScreenPos).magnitude;
            var sampleRadiusInScreen = godLight.sampleRadius / godLight.radius * radiusInScreen;
            drawRadialBlur.SetVector("_Center", godLightScreenPos * feature.resolutionMult);
            drawRadialBlur.SetFloat("_Radius", radiusInScreen * feature.resolutionMult);
            drawRadialBlur.SetFloat("_SampleRadius", sampleRadiusInScreen * feature.resolutionMult);
            drawRadialBlur.SetInt("_SampleCount", 20);
            cmd.SetRenderTarget(swapA);
            cmd.ClearRenderTarget(true, true, Color.black.WithA(0));
            cmd.Blit(swapB, swapA, drawRadialBlur);
            
            // 画模糊.
            using var _ = TempList.Get<RenderTexture>(out var list);
            for (int i = 0; i <= iteration; i++) list.Add(i % 2 == 0 ? swapA : swapB);
            for (int i = 0; i < feature.iteration; i++)
            {
                cmd.Blit(list[i * 2], list[i * 2 + 1], materialX);
                cmd.Blit(list[i * 2 + 1], list[i * 2 + 2], materialY);
            }
            
            // 把太阳画到原图上.
            cmd.SetRenderTarget(renderingData.cameraData.targetTexture);
            finalBlit.SetFloat("_Intensity", godLight.intensity);
            cmd.Blit(list[list.Count - 1],
                targetHandle,
                finalBlit
            );
            
            // cmd.Blit(swapA, targetHandle, finalBlit);
            
            context.ExecuteCommandBuffer(cmd);
        }
        
        void PrepareMaterial()
        {
            materialX = "Hidden/Prota/GaussianBlurSinglePassHorizontal".CreateMaterialFromShaderName();
            materialX.name = "Prota God Light Render Pass Horizontal";
            // materialX.SetFloat("_Mult", feature.intensity);
        
            materialY = "Hidden/Prota/GaussianBlurSinglePassVertical".CreateMaterialFromShaderName();
            materialY.name = "Prota God Light Render Pass Vertical";
            // materialX.SetFloat("_Mult", feature.intensity);
        
        
            finalBlit = "Hidden/Prota/GaussianBlurResult".CreateMaterialFromShaderName();
            finalBlit.name = "Prota Gaussian Blur Final Blit";
        
            drawCullArea = "Hidden/Prota/DrawDepth".CreateMaterialFromShaderName();
            drawCullArea.name = "Prota Gaussian Blur Draw Cull Area";
            drawCullArea.SetFloat("_AlphaClip", 0.5f);
        
            drawCulled = "Hidden/Prota/DrawCulled".CreateMaterialFromShaderName();
            drawCulled.name = "Prota Gaussian Blur Draw Culled";
            
            drawRadialBlur = "Hidden/Prota/RadialBlur".CreateMaterialFromShaderName();
            drawRadialBlur.name = "Prota Gaussian Blur Radial Blur";
        }

        void CreateSwapBuffer()
        {
            if(swapA != null) return;
            
            var size = this.size;
            
            if(size.x == 0 || size.y == 0)
            {
                Debug.LogError("ProtaGaussianBlurRenderPass: size is zero");
                return;
            }
            
            swapA = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.RGB111110Float);
            swapB = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.RGB111110Float);
            cull = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.RGB111110Float);
            
            swapA.name = "Prota Gaussian Blur Swap A";
            swapB.name = "Prota Gaussian Blur Swap B";
            cull.name = "Prota Gaussian Blur Cull";
        }
        
        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            if(swapA != null) RenderTexture.ReleaseTemporary(swapA);
            if(swapB != null) RenderTexture.ReleaseTemporary(swapB);
            if(cull != null) RenderTexture.ReleaseTemporary(cull);
            swapA = null;
            swapB = null;
            cull = null;
        }
    }
}

