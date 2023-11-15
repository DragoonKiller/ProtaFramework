using UnityEngine;
using Prota;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Prota.Unity;
using UnityEngine.Rendering.RendererUtils;

public class ProtaLightRenderFeature : ScriptableRendererFeature
{
    public Material material;
    
    public Color skyColorAdd = Color.clear;
    public Color skyColorMult = Color.clear;
    
    public Vector4 maxColorAdd = new Vector4(3, 3, 3, 1);
    public Vector4 maxColorMult = new Vector4(3, 3, 3, 1);
    
    public int sizeMult = 2;
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(new ProtaLightRenderPass(this));
    }
    
    public override void Create()
    {
        
    }
}




[ExecuteAlways]
public class ProtaLightRenderPass : ScriptableRenderPass
{
    int sizeMult = 2;
    
    ProtaLightRenderFeature feature = null;
    
    RenderTexture addRt;
    RenderTexture multRt;
    
    public ProtaLightRenderPass(ProtaLightRenderFeature x)
    {
        feature = x;
        sizeMult = x.sizeMult;
        this.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
    }
    
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var mat = feature.material;
        
        if(mat == null)
        {
            Debug.LogWarning("ProtaFramework: LightRenderPass: material is null");
            return;
        }
        
        var size = new Vector2Int(Screen.width / sizeMult, Screen.width / sizeMult);
        if(size.x == 0 || size.y == 0) return;
        
        
        
        addRt = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.ARGB32);
        addRt.name = "Prota LightRenderPass Add RT";
        multRt = RenderTexture.GetTemporary(size.x, size.y, 0, RenderTextureFormat.ARGB32);
        multRt.name = "Prota LightRenderPass Mult RT";
        
        ShaderTagId shaderTagId = new ShaderTagId("ProtaLight2D");
        
        mat.SetTexture("_LightAddTexture", addRt);
        mat.SetTexture("_LightMultTexture", multRt);
        
        var cmd = new CommandBuffer();
        cmd.name = "Prota Light Render Pass";
        
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
        
        // 2601: 2D 加光照材质.
        renderListDesc = new RendererListDesc(shaderTagId, cullingResult, renderingData.cameraData.camera);
        renderListDesc.renderQueueRange = new RenderQueueRange(2601, 2601);
        rdList = context.CreateRendererList(renderListDesc);
        cmd.SetRenderTarget(addRt);
        cmd.ClearRenderTarget(true, true, Color.black);
        cmd.DrawRendererList(rdList);
        
        mat.SetColor("_SkyColorAdd", feature.skyColorAdd);
        mat.SetColor("_SkyColorMult", feature.skyColorMult);
        mat.SetColor("_MaxLightAdd", feature.maxColorAdd);
        mat.SetColor("_MaxLightMult", feature.maxColorMult);
        cmd.Blit(null as Texture, BuiltinRenderTextureType.CameraTarget, mat);
        
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
