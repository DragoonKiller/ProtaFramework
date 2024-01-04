using System.Collections.Generic;
using UnityEngine;
using System;

using Prota.Unity;

namespace Prota.Procedural
{
    [Serializable]
    public class QuadGenerator : IProceduralMeshGenerator
    {
        static Vector3[] quad = new Vector3[4];
        
        public Vector3 bottomLeft = new Vector3(-0.5f, 0, 0);
        public Vector3 bottonRight = new Vector3(0.5f, 0, 0);
        public Vector3 topLeft = new Vector3(-0.5f, 1, 0);
        public Vector3 topRight = new Vector3(0.5f, 1, 0);
        
        static List<int> list = new List<int>{
            0, 1, 2, 3,
        };
        
        static Vector2[] uv = new Vector2[]{
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
        };
        
        [field:SerializeField, Readonly] public Mesh mesh { get; private set; }
        
        public void UpdateMesh(ProceduralMesh p)
        {
            Dispose();
            mesh = new Mesh();
            
            quad[0] = bottomLeft;
            quad[1] = bottonRight;
            quad[2] = topRight;
            quad[3] = topLeft;
            
            mesh.SetVertices(quad);
            mesh.SetUVs(0, uv);
            mesh.SetIndices(list, MeshTopology.Quads, 0, true, 0);
            
            mesh.RecalculateBounds();
            
            mesh.name = "Generated:Quad";
            
            p.mesh = mesh;
        }

        public void Dispose()
        {
            if(mesh != null) UnityEngine.Object.DestroyImmediate(mesh);
            mesh = null;
        }

    }
}
