using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    [ExecuteAlways]
    public class ProtaMaterialProvider : MonoBehaviour
    {
        [NonSerialized] MaterialPropertyBlock data;
        public bool useInstantiatedMaterial;
        public Material referenceMaterial;
        public Material instanceMaterial;
        
        public Renderer[] targets = Array.Empty<Renderer>();
        
        [SerializeField] VectorEntry[] vectorEntries = Array.Empty<VectorEntry>();
        [SerializeField] FloatEntry[] floatEntries = Array.Empty<FloatEntry>();
        [SerializeField] IntEntry[] intEntries = Array.Empty<IntEntry>();
        [SerializeField] TextureEntry[] textureEntries = Array.Empty<TextureEntry>();
        [SerializeField] MatrixEntry[] matrixEntries = Array.Empty<MatrixEntry>();
        
        // ====================================================================================================
        // ====================================================================================================
        
        public void OnEnable()
        {
            submittedMaterial = null;
            Update();
        }
        
        public void OnDisable()
        {
            if(instanceMaterial != null) DestroyImmediate(instanceMaterial);
            instanceMaterial = null;
            submittedMaterial = null;
        }
        
        void Update()
        {
            SyncMaterialState();
            SyncDataToPropertyBlock();
            SyncDataToMaterial();
            AssignMaterialToAllTargets();
        }
        
        Material submittedMaterial;
        
        void SyncMaterialState()
        {
            if(useInstantiatedMaterial)
            {
                data = null;
                
                if(referenceMaterial == null)
                {
                    if(instanceMaterial != null) DestroyImmediate(instanceMaterial);
                    submittedMaterial = instanceMaterial = null;
                }
                else if(submittedMaterial != referenceMaterial)
                {
                    if(instanceMaterial != null) DestroyImmediate(instanceMaterial);
                    submittedMaterial = referenceMaterial;
                    instanceMaterial = new Material(referenceMaterial) { name = referenceMaterial.name + "Instance" };
                }
            }
            else
            {
                data = new MaterialPropertyBlock();
                if(instanceMaterial != null) DestroyImmediate(instanceMaterial);
                submittedMaterial = instanceMaterial = null;
            }
        }
        
        public void SyncDataToPropertyBlock()
        {
            if(useInstantiatedMaterial) return;
            data.Clear();
            foreach(var entry in vectorEntries) data.SetVector(entry.id, entry.value);
            foreach(var entry in floatEntries) data.SetFloat(entry.id, entry.value);
            foreach(var entry in intEntries) data.SetInteger(entry.id, entry.value);
            foreach(var entry in textureEntries) if(entry.value != null) data.SetTexture(entry.id, entry.value);
            foreach(var entry in matrixEntries) data.SetMatrix(entry.id, entry.value);
        }
        
        public void SyncDataToMaterial()
        {
            if(!useInstantiatedMaterial) return;
            foreach(var entry in vectorEntries) instanceMaterial.SetVector(entry.id, entry.value);
            foreach(var entry in floatEntries) instanceMaterial.SetFloat(entry.id, entry.value);
            foreach(var entry in intEntries) instanceMaterial.SetInteger(entry.id, entry.value);
            foreach(var entry in textureEntries) if(entry.value != null) instanceMaterial.SetTexture(entry.id, entry.value);
            foreach(var entry in matrixEntries) instanceMaterial.SetMatrix(entry.id, entry.value);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        [Serializable]
        struct VectorEntry
        {
            public int id;
            public Vector4 value;
        }
        
        [Serializable]
        struct FloatEntry
        {
            public int id;
            public float value;
        }
        
        [Serializable]
        struct IntEntry
        {
            public int id;
            public int value;
        }
        
        [Serializable]
        struct TextureEntry
        {
            public string name;
            public int id;
            public Texture value;
        }
        
        [Serializable]
        struct MatrixEntry
        {
            public string name;
            public int id;
            public Matrix4x4 value;
        }
        
        Renderer _renderer;
        public new Renderer renderer
        {
            get
            {
                if(_renderer == null) _renderer = GetComponent<Renderer>();
                return _renderer;
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        static Shader _spriteShader;
        static Material _cachedMaterial;
        static Material cachedMaterial
        {
            get
            {
                if(_cachedMaterial == null)
                {
                    _cachedMaterial = new Material(shader);
                }
                return _cachedMaterial;
            }
        }
        
        static Shader shader
        {
            get
            {
                if(_spriteShader == null)
                {
                    _spriteShader = Shader.Find("Prota/Sprite");
                    if(_spriteShader == null) throw new Exception("Shader not found: Prota/Sprite");
                }
                return _spriteShader;
            }
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        void AssignMaterialToAllTargets()
        {
            foreach(var t in targets) AssignMaterial(t);
        }
        
        void AssignMaterial(Renderer rd)
        {
            if(rd == null) return;
            
            if(useInstantiatedMaterial)
            {
                if(instanceMaterial == null) return;
                rd.sharedMaterial = instanceMaterial;
            }
            else
            {
                if(data == null) return;
                rd.sharedMaterial = referenceMaterial;
                rd.SetPropertyBlock(data);
            }
        }
        
    }
    
}
