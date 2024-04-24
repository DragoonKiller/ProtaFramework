using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEditor;
using UnityEngine.Profiling;

namespace Prota.Unity
{
    [ExecuteAlways]
    public class ProtaMaterialProvider : MonoBehaviour
    {
        static HashSet<ProtaMaterialProvider> all = new HashSet<ProtaMaterialProvider>();
        
        const int executionPriority = 1601;
        
        [NonSerialized] MaterialPropertyBlock data;
        public bool useInstantiatedMaterial;
        public Material referenceMaterial;
        public Material instanceMaterial;
        
        public Renderer[] targets = Array.Empty<Renderer>();
        
        [SerializeField] VectorEntry[] vectorEntries = Array.Empty<VectorEntry>();
        [SerializeField] bool[] vectorValid = Array.Empty<bool>();
        [SerializeField] FloatEntry[] floatEntries = Array.Empty<FloatEntry>();
        [SerializeField] bool[] floatValid = Array.Empty<bool>();
        [SerializeField] IntEntry[] intEntries = Array.Empty<IntEntry>();
        [SerializeField] bool[] intValid = Array.Empty<bool>();
        [SerializeField] TextureEntry[] textureEntries = Array.Empty<TextureEntry>();
        [SerializeField] bool[] textureValid = Array.Empty<bool>();
        [SerializeField] MatrixEntry[] matrixEntries = Array.Empty<MatrixEntry>();
        [SerializeField] bool[] matrixValid = Array.Empty<bool>();
        
        // ====================================================================================================
        // ====================================================================================================
        
        static Texture2D whiteTexture;
        
        static int thisFrame;
        
        int updatedFrame;
        
        
        [InitializeOnLoadMethod]
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            whiteTexture = Texture2D.whiteTexture;
        }
        
        public void OnEnable()
        {
            all.Add(this);
            submittedMaterial = null;
            Step();
        }
        
        public void OnDisable()
        {
            all.Remove(this);
            if(instanceMaterial != null) DestroyImmediate(instanceMaterial);
            instanceMaterial = null;
            submittedMaterial = null;
        }
        
        void OnWillRenderObject()
        {
            if(!Application.isPlaying) Step();
            else
            {
                thisFrame = Time.frameCount;
                if(updatedFrame == thisFrame) return;
                StepAll();
            }
        }
        
        void StepAll()
        {
            Profiler.BeginSample("::ProtaMaterialProvider.StepAll");
            foreach(var x in all) x.SyncMaterialState();
            foreach(var x in all) x.ClearNullTextures();
            Parallel.ForEach(all, x => x.SyncDataToPropertyBlock());
            foreach(var x in all) x.SyncDataToMaterial();
            foreach(var x in all) x.AssignMaterialToAllTargets();
            foreach(var x in all) x.updatedFrame = thisFrame;
            Profiler.EndSample();
        }
        
        void Step()
        {
            ClearNullTextures();
            SyncMaterialState();
            SyncDataToPropertyBlock();
            SyncDataToMaterial();
            AssignMaterialToAllTargets();
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        
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
                if(data == null) data = new MaterialPropertyBlock();
                if(instanceMaterial != null) DestroyImmediate(instanceMaterial);
                submittedMaterial = instanceMaterial = null;
            }
        }
        
        public void SyncDataToPropertyBlock()
        {
            if(useInstantiatedMaterial) return;
            
            data.Clear();
            
            for(int i = 0; i < vectorEntries.Length; i++)
                if(vectorValid[i]) data.SetVector(vectorEntries[i].id, vectorEntries[i].value);
                
            for(int i = 0; i < floatEntries.Length; i++)
                if(floatValid[i]) data.SetFloat(floatEntries[i].id, floatEntries[i].value);
                
            for(int i = 0; i < intEntries.Length; i++)
                if(intValid[i]) data.SetInt(intEntries[i].id, intEntries[i].value);
                
            for(int i = 0; i < textureEntries.Length; i++)
                if(textureValid[i]) data.SetTexture(textureEntries[i].id, textureEntries[i].value);
                
            for(int i = 0; i < matrixEntries.Length; i++)
                if(matrixValid[i]) data.SetMatrix(matrixEntries[i].id, matrixEntries[i].value);
        }
        
        public void SyncDataToMaterial()
        {
            if(!useInstantiatedMaterial) return;
            for(int i = 0; i < vectorEntries.Length; i++)
                if(vectorValid[i]) instanceMaterial.SetVector(vectorEntries[i].id, vectorEntries[i].value);
                else instanceMaterial.SetVector(vectorEntries[i].id, referenceMaterial.GetVector(vectorEntries[i].id));
            
            for(int i = 0; i < floatEntries.Length; i++)
                if(floatValid[i]) instanceMaterial.SetFloat(floatEntries[i].id, floatEntries[i].value);
                else instanceMaterial.SetFloat(floatEntries[i].id, referenceMaterial.GetFloat(floatEntries[i].id));
                
            for(int i = 0; i < intEntries.Length; i++)
                if(intValid[i]) instanceMaterial.SetInteger(intEntries[i].id, intEntries[i].value);
                else instanceMaterial.SetInteger(intEntries[i].id, referenceMaterial.GetInt(intEntries[i].id));
                
            for(int i = 0; i < textureEntries.Length; i++)
                if(textureValid[i]) instanceMaterial.SetTexture(textureEntries[i].id, textureEntries[i].value);
                else instanceMaterial.SetTexture(textureEntries[i].id, referenceMaterial.GetTexture(textureEntries[i].id));
                
            for(int i = 0; i < matrixEntries.Length; i++)
                if(matrixValid[i]) instanceMaterial.SetMatrix(matrixEntries[i].id, matrixEntries[i].value);
                else instanceMaterial.SetMatrix(matrixEntries[i].id, referenceMaterial.GetMatrix(matrixEntries[i].id));
        }
        
        
        void ClearNullTextures()
        {
            for(int i = 0; i < textureEntries.Length; i++)
            {
                if(textureEntries[i].value == null)
                {
                    textureEntries[i].value = Texture2D.whiteTexture;
                }
            }
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
                rd.SetPropertyBlock(null);
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
