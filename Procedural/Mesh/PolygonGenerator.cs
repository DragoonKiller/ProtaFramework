using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace Prota.Procedural
{
    [Serializable]
    public class PolygonGenerator : IProceduralMeshGenerator
    {
        [field:SerializeField] public Mesh mesh { get; private set; }

        public void Dispose()
        {
            if(mesh != null) UnityEngine.Object.DestroyImmediate(mesh);
            mesh = null;
        }


        public void UpdateMesh(ProceduralMesh p)
        {
            Dispose();
            mesh = new Mesh();
            
            using var _ = TempList.Get<Vector2>(out var vertices);
            var polygonCollider = p.GetComponent<PolygonCollider2D>();
            polygonCollider.GetPath(0, vertices);
            using var __ = TempList.Get<int>(out var indices);
            Prota.Unity.Algorithm.Triangulate(vertices, indices);
            
            using var ___ = TempList.Get<Vector3>(out var vertices3d);
            foreach(var v in vertices) vertices3d.Add(v);
            
            mesh.SetVertices(vertices3d);
            mesh.SetIndices(indices, MeshTopology.Triangles, 0, true, 0);
            
            mesh.RecalculateBounds();
            
            mesh.name = "Generated:Polygon";
            
            p.mesh = mesh;
        }
        
        
        
    }
}
