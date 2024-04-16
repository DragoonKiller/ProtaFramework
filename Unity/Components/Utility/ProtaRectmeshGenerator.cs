using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prota.Unity
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(MeshFilter))]
    public class ProtaRectmeshGenerator : MonoBehaviour
    {
        const int executionPriority = 1600;
        
        RectTransform rectTransform;
        MeshFilter meshFilter;
        Mesh mesh;
        
        // 用以确定 uv.
        public Sprite sprite;
        
        // xyzw:左右上下的扩展.
        public Vector4 extend = Vector4.zero;
        
        // uv 偏移.
        public bool uvOffsetByTime = false;
        [ShowWhen("uvOffsetByTime")] public Vector2 uvOffset = Vector2.zero;
        [ShowWhen("uvOffsetByTime")] public bool uvOffsetByRealtime = false;
        
        
        // 剪切形变, 角度.
        [Range(-90, 90)] public float shear;
        // 剪切形变是否改变高度?
        public bool useRadialShear;
        
        public bool flipX;
        public bool flipY;
        [ColorUsage(true, true)] public Color vertexColor = Color.white;
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
            meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh = new Mesh() { name = "GeneratedMesh" };
            forceUpdateMesh = true;
        }
        
        void OnDisable()
        {
            if(meshFilter.sharedMesh == mesh)
            {
                DestroyImmediate(meshFilter.sharedMesh);
                mesh = null;
            }
        }
        
        void OnWillRenderObject()
        {
            Step();
        }
        
        
        void Step()
        {
            if(NeedUpdateMesh()) UpdateMesh();
        }
        
        
        // ====================================================================================================
        // ====================================================================================================

        public bool forceUpdateMesh = false;
        
        
        public Rect localRect => rectTransform.rect;
        
        
        [NonSerialized] Rect submittedRect;
        [NonSerialized] Vector4 submittedExtend;
        [NonSerialized] bool submittedUseRadialShear;
        [NonSerialized] float submittedShear;
        [NonSerialized] Vector2 submittedUvOffset;
        [NonSerialized] bool submittedUvOffsetByTime;
        [NonSerialized] bool submittedUvOffsetByRealtime;
        [NonSerialized] bool submittedFlipX;
        [NonSerialized] bool submittedFlipY;
        [NonSerialized] Color submittedVertexColor;
        [NonSerialized] Sprite submittedSprite;
        
        bool NeedUpdateMesh()
        {
            if(forceUpdateMesh) return true;
            if(submittedRect != rectTransform.rect) return true;
            if(submittedExtend != extend) return true;
            if(submittedUseRadialShear != useRadialShear) return true;
            if(submittedShear != shear) return true;
            if(submittedUvOffset != uvOffset) return true;
            if(submittedUvOffsetByTime != uvOffsetByTime) return true;
            if(submittedUvOffsetByRealtime != uvOffsetByRealtime) return true;
            if(submittedFlipX != flipX) return true;
            if(submittedFlipY != flipY) return true;
            if(submittedVertexColor != vertexColor) return true;
            if(submittedSprite != sprite) return true;
            return false;
        }
        
        [ThreadStatic] static Vector3[] tempVertices;
        [ThreadStatic] static Vector2[] tempUV;
        [ThreadStatic] static Color[] tempColors;
        
        static readonly int[] defaultTriangles = new int[] {
            0, 1, 2,
            2, 1, 3,
        };
        
        void UpdateMesh()
        {
            if(tempVertices == null) tempVertices = new Vector3[4];
            if(tempUV == null) tempUV = new Vector2[4];
            if(tempColors == null) tempColors = new Color[4];
            
            var rect = rectTransform.rect;
        
            // 计算扩展后的矩形.
            rect.xMin -= extend.x;
            rect.yMin -= extend.y;
            rect.xMax += extend.z;
            rect.yMax += extend.w;
            
            // 顺序: 左上, 右上, 左下, 右下. uv 同.
            tempVertices[0] = new Vector3(rect.xMin, rect.yMax, 0);
            tempVertices[1] = new Vector3(rect.xMax, rect.yMax, 0);
            tempVertices[2] = new Vector3(rect.xMin, rect.yMin, 0);
            tempVertices[3] = new Vector3(rect.xMax, rect.yMin, 0);
            
            // 应用剪切形变.
            if(useRadialShear)
            {
                var xOffsetTop = rect.yMax * Mathf.Sin(shear.ToRadian());
                var xOffsetBottom = rect.yMin * Mathf.Sin(shear.ToRadian());
                var yOffsetTop = rect.yMax * (1 - Mathf.Cos(shear.ToRadian()));
                var yOffsetBottom = rect.yMin * (1 - Mathf.Cos(shear.ToRadian()));
                tempVertices[0].x += xOffsetTop;
                tempVertices[1].x += xOffsetTop;
                tempVertices[2].x += xOffsetBottom;
                tempVertices[3].x += xOffsetBottom;
                tempVertices[0].y -= yOffsetTop;
                tempVertices[1].y -= yOffsetTop;
                tempVertices[2].y -= yOffsetBottom;
                tempVertices[3].y -= yOffsetBottom;
            }
            else
            {
                var xOffsetTop = rect.yMax * Mathf.Tan(shear.ToRadian());
                var xOffsetBottom = rect.yMin * Mathf.Tan(shear.ToRadian());
                tempVertices[0].x += xOffsetTop;
                tempVertices[1].x += xOffsetTop;
                tempVertices[2].x += xOffsetBottom;
                tempVertices[3].x += xOffsetBottom;
            }
            
            tempColors[0] = vertexColor;
            tempColors[1] = vertexColor;
            tempColors[2] = vertexColor;
            tempColors[3] = vertexColor;
            
            if(sprite)
            {
                var uv = sprite.uv;
                tempUV[0] = uv[0];
                tempUV[1] = uv[1];
                tempUV[2] = uv[2];
                tempUV[3] = uv[3];
            }
            else
            {
                tempUV[0] = new Vector2(0, 1);
                tempUV[1] = new Vector2(1, 1);
                tempUV[2] = new Vector2(0, 0);
                tempUV[3] = new Vector2(1, 0);
            }
            
            TransformUVExtend(rectTransform.rect.size, extend, tempUV);
            
            
            if(flipX)
            {
                Swap(ref tempUV[0], ref tempUV[1]);
                Swap(ref tempUV[2], ref tempUV[3]);
            }
            if(flipY)
            {
                Swap(ref tempUV[0], ref tempUV[2]);
                Swap(ref tempUV[1], ref tempUV[3]);
            }
            
            mesh.SetVertices(tempVertices);
            mesh.SetUVs(0, tempUV);
            mesh.SetColors(tempColors);
            mesh.SetIndices(defaultTriangles, MeshTopology.Triangles, 0);
            
            mesh.RecalculateBounds();
            
            forceUpdateMesh = false;
            submittedRect = rect;
            submittedExtend = extend;
            submittedUseRadialShear = useRadialShear;
            submittedShear = shear;
            submittedUvOffset = uvOffset;
            submittedUvOffsetByTime = uvOffsetByTime;
            submittedUvOffsetByRealtime = uvOffsetByRealtime;
            submittedFlipX = flipX;
            submittedFlipY = flipY;
            submittedVertexColor = vertexColor;
            submittedSprite = sprite;
        }

        static void Swap<T>(ref T a, ref T b)
        {
            var t = a;
            a = b;
            b = t;
        }
        
        
        static void TransformUVExtend(Vector2 size, Vector4 extend, Vector2[] points)
        {
            // 顺序: 左上, 右上, 左下, 右下.
            var xmin = points[0].x;
            var xmax = points[1].x;
            var ymin = points[2].y;
            var ymax = points[0].y;
            
            var uvsize = new Vector2(xmax - xmin, ymax - ymin);
            
            var exmin = xmin - extend.x / size.x * uvsize.x;
            var exmax = xmax + extend.z / size.x * uvsize.x;
            var eymin = ymin - extend.y / size.y * uvsize.y;
            var eymax = ymax + extend.w / size.y * uvsize.y;
            
            points[0] = new Vector2(exmin, eymax);
            points[1] = new Vector2(exmax, eymax);
            points[2] = new Vector2(exmin, eymin);
            points[3] = new Vector2(exmax, eymin);
        }
        
        
    }
}
