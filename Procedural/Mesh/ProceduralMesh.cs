using System.Collections.Generic;
using UnityEngine;

using Prota.Unity;
using System;

namespace Prota.Procedural
{
    public interface IProceduralMeshGenerator : IDisposable
    {
        public void UpdateMesh(ProceduralMesh mesh);
    }
    
    // 将生成的 mesh 放置到 MeshFilter 中.
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter))]
    public class ProceduralMesh : MonoBehaviour
    {
        public enum MeshType
        {
            None,
            Quad,
            Polygon,
        }
        
        public MeshType type;
        
        [SerializeField, Readonly] MeshFilter _meshFilter;
        public MeshFilter meshFilter
            => _meshFilter == null ? _meshFilter = this.GetComponent<MeshFilter>() : _meshFilter;
        
        [SerializeReference] public IProceduralMeshGenerator meshGenerator;
        
        public Mesh mesh
        {
            get => meshFilter.mesh;
            set => meshFilter.mesh = value;
        }
        
        void OnEnable()
        {
            RegenMesh();
        }
        
        public void RegenMesh()
        {
            if(meshGenerator != null)
            {
                meshGenerator.Dispose();
                meshGenerator = null;
            }
            
            if(type == MeshType.None)
            {
                meshGenerator = null;
            }
            else if(type == MeshType.Quad)
            {
                meshGenerator = new QuadGenerator();
            }
            else if(type == MeshType.Polygon)
            {
                this.gameObject.GetOrCreate<PolygonCollider2D>();
                meshGenerator = new PolygonGenerator();
            }
            
            meshGenerator?.UpdateMesh(this);
        }
    }
}
