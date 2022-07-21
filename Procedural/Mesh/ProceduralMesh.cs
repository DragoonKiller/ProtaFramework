using System.Collections.Generic;
using UnityEngine;


namespace Prota.Procedural
{
    public interface IProceduralMeshGenerator
    {
        public Mesh mesh { get; }
        
        public void UpdateMesh();
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
        }
        
        [SerializeField] MeshType _type;
        
        [SerializeField] MeshType recordType;
        
        public MeshType type
        {
            get => _type;
            set
            {
                type = value;
                Update();
            }
        }
        
        
        MeshFilter m => this.GetComponent<MeshFilter>();
        
        [SerializeReference] public IProceduralMeshGenerator meshGenerator;
        
        public void Update()
        {
            if(recordType != type)
            {
                recordType = type;
                
                if(type == MeshType.None)
                {
                    meshGenerator = null;
                }
                else if(type == MeshType.Quad)
                {
                    meshGenerator = new QuadGenerator();
                }
                
                meshGenerator?.UpdateMesh();
                m.mesh = meshGenerator?.mesh;
            }
        }
    }
}
