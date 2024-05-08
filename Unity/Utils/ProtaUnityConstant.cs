
using UnityEngine;
using Prota.Unity;
using UnityEditor;

namespace Prota.Unity
{
    [InitializeOnLoad]
    public static partial class ProtaUnityConstant
    {
        static Mesh _rectMesh;
        public static Mesh rectMesh
        {
            get
            {
                if(_rectMesh) return _rectMesh;
                _rectMesh = new Mesh() { name = "ProtaUnityConstant.rectMesh" };
                rectMesh.vertices = rectMeshVertices;
                rectMesh.uv = rectMeshUVs;
                rectMesh.triangles = rectMeshTriangles;
                rectMesh.colors = rectMeshColors;
                rectMesh.RecalculateBounds();
                return _rectMesh;
            }
        }
        
        static Mesh _rectMeshOne;
        public static Mesh rectMeshOne
        {
            get
            {
                if(_rectMeshOne) return _rectMeshOne;
                _rectMeshOne = new Mesh() { name = "ProtaUnityConstant.rectMeshOne" };
                rectMeshOne.vertices = rectMeshVerticesOne;
                rectMeshOne.uv = rectMeshUVs;
                rectMeshOne.triangles = rectMeshTriangles;
                rectMesh.colors = rectMeshColors;
                rectMeshOne.RecalculateBounds();
                return _rectMeshOne;
            }
        }
        
        public static readonly Vector3[] rectMeshVertices = new Vector3[] {
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
        };
        
        public static readonly Vector3[] rectMeshVerticesOne = new Vector3[] {
            new Vector3(-1, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(-1, -1, 0),
            new Vector3(1, -1, 0),
        };
        
        public static readonly Vector2[] rectMeshUVs = new Vector2[] {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
        };
        
        public static readonly Color[] rectMeshColors = new Color[] {
            Color.white,
            Color.white,
            Color.white,
            Color.white,
        };
        
        public static readonly int[] rectMeshTriangles = new int[] { 0, 1, 2, 2, 1, 3 };
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        static Material _urpSpriteUnlitMat;
        public static Material urpSpriteUnlitMat
        {
            get
            {
                if(!_urpSpriteUnlitMat)
                {
                    var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
                    _urpSpriteUnlitMat = new Material(shader);
                }
                return _urpSpriteUnlitMat;
            }
        }
        
        static Material _urpSpriteLitMat;
        public static Material urpSpriteLitMat
        {
            get
            {
                if(!_urpSpriteLitMat)
                {
                    var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default");
                    _urpSpriteLitMat = new Material(shader);
                }
                return _urpSpriteLitMat;
            }
        }
        
        static Material _uiDefaultMat;
        public static Material uiDefaultMat
        {
            get
            {
                if(!_uiDefaultMat)
                {
                    var shader = Shader.Find("UI/Default");
                    _uiDefaultMat = new Material(shader);
                }
                return _uiDefaultMat;
            }
        }
        
    }
    
}
