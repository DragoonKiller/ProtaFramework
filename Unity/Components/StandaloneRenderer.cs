using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Prota.Unity;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Prota.Unity
{
    [Serializable]
    public struct StandaloneRenderEntry
    {
        public Rect position;       // blit range.
        public bool useCenterPivot;
        public Material material;
    }
    
    // 集成渲染贴图, 仅需一个组件把 RenderTexture 需要的所有东西画上去.
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class StandaloneRenderer : MonoBehaviour
    {
        public RenderTexture renderTexture;
        
        public Rect range => this.RectTransform().rect;
        
        public StandaloneRenderEntry[] entries;
        
        #if UNITY_EDITOR
        public bool preview;
        #endif
        
        CommandBuffer _cmd;
        CommandBuffer cmd => _cmd ?? (_cmd = new CommandBuffer() { name = "StandaloneRenderer" });
        
        void OnEnable()
        {
            RenderPipelineManager.beginContextRendering += this.OnRendering;
        }
        
        void OnDisable()
        {
            RenderPipelineManager.beginContextRendering -= this.OnRendering;
            if(_cmd != null) _cmd.Dispose();
        }


        void Update()
        {
            #if UNITY_EDITOR
            if(preview) Preview();
            #endif
        }
        
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
            if(!renderTexture) return;
            if(entries.IsNullOrEmpty()) return;
            
            cmd.Clear();
            cmd.SetRenderTarget(renderTexture);
            cmd.ClearRenderTarget(true, true, Color.red);
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            foreach(var entry in entries)
            {
                if(!entry.material)
                {
                    Debug.LogWarning($"Material in StandaloneRenderer [{ this.GetNamePath() }] is null.", this);
                    continue;
                }
                var trs = Matrix4x4.TRS(
                    entry.position.min.ToVec3(),
                    Quaternion.identity,
                    entry.position.size.ToVec3()
                );
                var mesh = entry.useCenterPivot ? ProtaUnityConstant.rectMeshOne : ProtaUnityConstant.rectMesh;
                cmd.DrawMesh(mesh, trs, entry.material, 0, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        

    }
}
