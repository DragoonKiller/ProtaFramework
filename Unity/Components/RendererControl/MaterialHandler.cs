using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota
{
    [RequireComponent(typeof(Renderer))]
    [DisallowMultipleComponent]
    public class MaterialHandler : MonoBehaviour
    {
        public Renderer rd => this.GetComponent<Renderer>();
        
        void OnDestroy()
        {
            // 编辑模式下不做处理.
            if(!Application.isPlaying) return;
            
            // 删除当前的所有材质实例.
            // 如果材质实例没有被创建, 那么获取 rd.materials 的时候也会创建新的实例.
            // Unity 似乎没有检查一个 Renderer 的材质是否为 shared 的方式.
            // 但是既然已经创建了这个组件, 我们就默认材质被实例化了.
            rd.materials.DestroyAll();
        }
    }
    
    
    public static partial class UnityMethodExtensions
    {
        public static Material SetMainTex(this Material mat, Texture texture)
        {
            mat.SetTexture("_MainTex", texture);
            return mat;
        }
        
        public static MaterialHandler MaterialHandler(this Component s)
            => s.GetOrCreate<MaterialHandler>();
        
        public static Component MakeMaterialUnique(this Component s)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            return s;
        }
        
        public static Material GetMaterialInstance(this Component s)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            return handler.rd.material;
        }
        
        public static Material[] GetMaterialInstances(this Component s)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            return handler.rd.materials;
        }
        
        public static void SetMaterial(this Component s, Material material, int index = 0)
        {
            var handler = s.MaterialHandler();
            Debug.Assert(handler);
            handler.rd.SetMaterial(material, index);
        }
    }
}
