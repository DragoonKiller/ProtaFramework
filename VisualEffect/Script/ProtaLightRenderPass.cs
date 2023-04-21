using UnityEngine;
using Prota;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Experimental.Rendering;

[ExecuteAlways]
public class ProtaLightRenderPass : ScriptableRenderPass
{
    int sizeMult = 2;
    
    LightRenderFeature feature = null;
    
    RenderTexture addRt;
    RenderTexture multRt;
    
    public ProtaLightRenderPass(LightRenderFeature x)
    {
        feature = x;
        sizeMult = x.sizeMult;
        this.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if(feature.material == null)
        {
            Debug.LogWarning("ProtaFramework: LightRenderPass: material is null");
            return;
        }
        
        var size = new Vector2Int(Screen.width / sizeMult, Screen.width / sizeMult);
        if(size.x == 0 || size.y == 0) return;
        
        
        
        addRt = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.ARGB32);
        multRt = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.ARGB32);
        
        ShaderTagId shaderTagId = new ShaderTagId("ProtaLight2D");
        
        feature.material.SetTexture("_LightAddTexture", addRt);
        feature.material.SetTexture("_LightMultTexture", multRt);
        
        var cmd = new CommandBuffer();
        cmd.name = "Light Render";
        
        var lightLayer = LayerMask.NameToLayer("Light");
        renderingData.cameraData.camera.TryGetCullingParameters(out var cullingParams);
        cullingParams.cullingMask = 1u << lightLayer;
        var cullingResult = context.Cull(ref cullingParams);
        
        // 2600: 2D 乘光照材质.
        var renderListDesc = new RendererListDesc(shaderTagId, cullingResult, renderingData.cameraData.camera);
        renderListDesc.renderQueueRange = new RenderQueueRange(2600, 2600);
        var rdList = context.CreateRendererList(renderListDesc);
        cmd.SetRenderTarget(multRt);
        cmd.ClearRenderTarget(true, true, Color.black);
        cmd.DrawRendererList(rdList);
        
        // 2600: 2D 乘光照材质.
        renderListDesc = new RendererListDesc(shaderTagId, cullingResult, renderingData.cameraData.camera);
        renderListDesc.renderQueueRange = new RenderQueueRange(2601, 2601);
        rdList = context.CreateRendererList(renderListDesc);
        cmd.SetRenderTarget(addRt);
        cmd.ClearRenderTarget(true, true, Color.black);
        cmd.DrawRendererList(rdList);
        
        feature.material.SetColor("_SkyColorAdd", feature.skyColorAdd);
        feature.material.SetColor("_SkyColorMult", feature.skyColorMult);
        feature.material.SetColor("_MaxLightAdd", feature.maxColorAdd);
        feature.material.SetColor("_MaxLightMult", feature.maxColorMult);
        cmd.Blit(null as Texture, BuiltinRenderTextureType.CameraTarget, feature.material);
        
        context.ExecuteCommandBuffer(cmd);
        cmd.Release();
        
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        base.FrameCleanup(cmd);
        RenderTexture.ReleaseTemporary(addRt);
        RenderTexture.ReleaseTemporary(multRt);
        addRt = null;
        multRt = null;
    }
}
