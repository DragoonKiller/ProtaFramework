using UnityEngine;
using Prota;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Prota.Unity;

namespace Prota.VisualEffect
{

    public class ProtaSkyboxRenderFeature : ScriptableRendererFeature
    {
        public ProtaSkyboxRenderPass pass;
        
        [GradientUsage(true)] public Gradient colorGradient;
        
        public Material material;
        
        public Texture2D generatedTexture;
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if(renderingData.cameraData.cameraType != CameraType.Game
            && renderingData.cameraData.cameraType != CameraType.SceneView
            && renderingData.cameraData.cameraType != CameraType.Preview) return;
            renderer.EnqueuePass(pass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if(renderingData.cameraData.cameraType != CameraType.Game
            && renderingData.cameraData.cameraType != CameraType.SceneView
            && renderingData.cameraData.cameraType != CameraType.Preview) return;
            pass.SetTarget(renderer.cameraColorTargetHandle);
        }

        public override void Create()
        {
            pass = new ProtaSkyboxRenderPass(this);
            var shader = Shader.Find("Hidden/Prota/Skybox");
            material = new Material(shader) { name = "ProtaSkybox" };
            generatedTexture = new Texture2D(1, 256, TextureFormat.RGBA32, false, true) {
                name = "ProtaSkyboxGradientTexture"
            };
            generatedTexture.wrapMode = TextureWrapMode.Clamp;
            generatedTexture.filterMode = FilterMode.Bilinear;
            generatedTexture.anisoLevel = 0;
            generatedTexture.hideFlags = HideFlags.HideAndDontSave;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }




    [ExecuteAlways]
    public class ProtaSkyboxRenderPass : ScriptableRenderPass
    {
        ProtaSkyboxRenderFeature feature = null;
        
        RTHandle targetHandle;
        
        Gradient submittedGradient;
        
        public ProtaSkyboxRenderPass(ProtaSkyboxRenderFeature x)
        {
            feature = x;
            this.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
        }
        
        public void SetTarget(RTHandle handle)
        {
            targetHandle = handle;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            // Debug.LogError("Configure");
            GenerateTextureFromGradient();
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("ProtaSkyboxRenderPass");
            
            cmd.SetRenderTarget(targetHandle);
            cmd.Blit(feature.generatedTexture, targetHandle, feature.material);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        void GenerateTextureFromGradient()
        {
            if(feature.colorGradient == null) return;
            if(submittedGradient != null && submittedGradient.Equals(feature.colorGradient)) return;
            
            // Debug.LogError("GenerateTextureFromGradient");
            
            var gradient = feature.colorGradient;
            
            for (int i = 0; i < 256; i++)
            {
                var color = gradient.Evaluate(1.0f - i / 255f);
                feature.generatedTexture.SetPixel(0, i, color);
            }
            feature.generatedTexture.Apply();
            
            submittedGradient = new Gradient();
            submittedGradient.SetKeys(gradient.colorKeys, gradient.alphaKeys);
        }
        
    }
}

