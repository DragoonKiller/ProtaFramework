using UnityEngine;
using Prota;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Prota.Unity;

public class LightRenderFeature : ScriptableRendererFeature
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
