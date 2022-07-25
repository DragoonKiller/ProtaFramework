using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota
{
    [RequireComponent(typeof(Renderer))]
    [ExecuteAlways]
    public class MaterialHandler : MonoBehaviour
    {
        Renderer rd => this.GetComponent<Renderer>();
        
        [SerializeField] Material[] materialTemplates;
        
        [SerializeReference] Material[] materialCloneSource;
        
        [SerializeReference] Material[] materialInstances;
        
        [SerializeField] public bool isShared = false;
        
        public Material mat => mats?.FirstOrDefault();
        
        public Material[] mats => isShared ? materialTemplates : materialInstances;
        
        static Material[] tempArr = new Material[1];
        
        void Awake()
        {
            if(materialTemplates == null) materialTemplates = tempArr;
            materialCloneSource = materialTemplates.ToArray();
            materialInstances = materialTemplates.ToArray();
        }
        
        void OnDestroy()
        {
            Clear();
        }
        
        void Update()
        {
            var targetMats = isShared ? materialTemplates : materialInstances;
            if(rd.sharedMaterials != targetMats) rd.sharedMaterials = targetMats;
            SyncMaterials();
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        public MaterialHandler SetMaterialTemplate(int index, Material material)
        {
            if(index < 0) throw new ArgumentOutOfRangeException("index");
            if(materialTemplates.Length <= index)
            {
                var s = new Material[index + 1];
                materialTemplates.CopyTo(s, 0);
                s[index] = material;
            }
            return this;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        public MaterialHandler MakeUnique()
        {
            if(!isShared) return this;
            isShared = false;
            NewMaterialClone();
            Update();
            return this;
        }
        
        public MaterialHandler MakeShared()
        {
            if(isShared) return this;
            isShared = true;
            Clear();
            return this;
        }
        
        void NewMaterialClone()
        {
            if(isShared) return;
            if(materialInstances != null) Clear();
            materialInstances = materialTemplates.Select(x => x == null ? null : new Material(x) { name = "[!]" + x.name }).ToArray();
            materialCloneSource = materialTemplates.ToArray();
        }
        
        void SyncMaterials()
        {
            if(isShared) return;
            var n = materialTemplates.Length;
            var m = materialCloneSource.Length;
            if(n != m || !materialCloneSource.SequenceEqual(materialTemplates))
            {
                NewMaterialClone();
            }
        }
        
        void Clear()
        {
            materialCloneSource = null;
            materialInstances.DestroyAllImmediate();
        }
        
    }
}
