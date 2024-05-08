using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Profiling;

namespace Prota.Unity
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(MeshFilter))]
    public class ProtaRectmeshGenerator : MonoBehaviour
    {
        public static HashSet<ProtaRectmeshGenerator> all = new HashSet<ProtaRectmeshGenerator>();
        
        const int executionPriority = 1600;
        
        // ====================================================================================================
        // 控制属性
        // ====================================================================================================
        
        
        // 用以确定 uv.
        public Sprite sprite;
        
        // xyzw:左右上下的扩展.
        public Vector4 extend = Vector4.zero;
        
        // uv 偏移.
        public bool uvOffsetByTime = false;
        public Vector2 uvOffset = Vector2.zero;
        public bool uvOffsetByRealtime = false;
        
        // 剪切形变, 角度.
        [Range(-90, 90)] public float shear;
        // 剪切形变是否改变高度?
        public bool useRadialShear;
        
        public bool flipX;
        public bool flipY;
        [ColorUsage(true, true)] public Color vertexColor = Color.white;
        
        [NonSerialized] public bool needUpdateMesh = false;
        
        // ====================================================================================================
        // 记录属性
        // ====================================================================================================
        
        [NonSerialized] RectTransform rectTransform;
        [NonSerialized] MeshFilter meshFilter;
        [NonSerialized] Mesh mesh;
        
        [NonSerialized] int updatedToFrame;
        static int thisFrame;
        [NonSerialized] Rect trRect;
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        void OnEnable()
        {
            rectTransform = GetComponent<RectTransform>();
            meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh = new Mesh() { name = "GeneratedMesh" };
            all.Add(this);
            needUpdateMesh = true;
        }
        
        void OnDisable()
        {
            all.Remove(this);
            if(meshFilter.sharedMesh == mesh)
            {
                DestroyImmediate(meshFilter.sharedMesh);
                mesh = null;
            }
        }
        
        
        
        // ====================================================================================================
        // 更新控制
        // ====================================================================================================
        // 还有多线程支持
        
        void OnWillRenderObject()
        {
            if(!Application.isPlaying)
            {
                Step();
                return;
            }
            
            // 只有运行时才会并行计算.
            // 并行计算会更新所有的 updatedToFrame, 这样被更新后的组件就不会再次更新.
            thisFrame = Time.frameCount;
            if(updatedToFrame != thisFrame)
            {
                StepAll();
            }
        }
        
        static void StepAll()
        {
            Profiler.BeginSample("::ProtaRectmeshGenerator.StepAll");
            foreach(var x in all) x.trRect = x.rectTransform.rect;
            foreach(var x in all) SpriteUVCache.Prepare(x.sprite);
            Parallel.ForEach(all, x => x.StepSingleMultithread());
            foreach(var x in all) x.SetMesh();
            Profiler.EndSample();
        }
        
        void StepSingleMultithread()
        {
            updatedToFrame = thisFrame;
            RemoveTempObjectsWhenTimesUp();
            if(!needUpdateMesh) return;
            UpdateMeshValues();
        }
        
        void Step()
        {
            thisFrame = Time.frameCount;
            trRect = rectTransform.rect;
            SpriteUVCache.Prepare(sprite);
            if(needUpdateMesh)
            {
                UpdateMeshValues();
                SetMesh();
            }
            RemoveTempObjectsWhenTimesUp();
        }
        
        
        // ====================================================================================================
        // 网格更新逻辑
        // ====================================================================================================

        public Rect localRect => rectTransform.rect;
        
        
        [NonSerialized] Vector3[] tempVertices;
        [NonSerialized] Vector2[] tempUV;
        [NonSerialized] Color[] tempColors;
        [NonSerialized] Bounds tempBounds;
        [NonSerialized] float lastTempCreatedTime;
        [NonSerialized] bool meshNeedsToBeSet;
        
        void UpdateMeshValues()
        {
            // 记录临时对象被创建的时间.
            if(tempVertices == null || tempUV == null || tempColors == null) lastTempCreatedTime = thisFrame;
            if(tempVertices == null) tempVertices = new Vector3[4];
            if(tempUV == null) tempUV = new Vector2[4];
            if(tempColors == null) tempColors = new Color[4];
            
            var rect = trRect;
        
            // 计算扩展后的矩形.
            rect = rect.Expend(extend.x, extend.z, extend.y, extend.w);
            
            // 顺序: 左上, 右上, 左下, 右下. uv 同.
            rect.FillWithStandardMeshOrder(tempVertices);
            
            var radShear = shear.ToRadian();
            
            // 应用剪切形变.
            if(useRadialShear)
            {
                var xOffsetTop = rect.yMax * Mathf.Sin(radShear);
                var xOffsetBottom = rect.yMin * Mathf.Sin(radShear);
                var yOffsetTop = rect.yMax * (1 - Mathf.Cos(radShear));
                var yOffsetBottom = rect.yMin * (1 - Mathf.Cos(radShear));
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
                var xOffsetTop = rect.yMax * Mathf.Tan(radShear);
                var xOffsetBottom = rect.yMin * Mathf.Tan(radShear);
                tempVertices[0].x += xOffsetTop;
                tempVertices[1].x += xOffsetTop;
                tempVertices[2].x += xOffsetBottom;
                tempVertices[3].x += xOffsetBottom;
            }
            
            
            tempColors.Fill(vertexColor);
            
            if(sprite)
            {
                var uv = SpriteUVCache.Get(sprite);
                uv.CopyTo(tempUV);
            }
            else
            {
                ProtaUnityConstant.rectMeshUVs.CopyTo(tempUV);
            }
            
            TransformUVExtend(trRect.size, extend, tempUV);
            
            if(flipX)
            {
                tempUV[0].Swap(ref tempUV[1]);
                tempUV[2].Swap(ref tempUV[3]);
            }
            if(flipY)
            {
                tempUV[0].Swap(ref tempUV[2]);
                tempUV[1].Swap(ref tempUV[3]);
            }
            
            tempBounds.center = rect.center;
            tempBounds.size = rect.size;      // 扩展后的矩形的大小.
            
            needUpdateMesh = false;
            meshNeedsToBeSet = true;
        }
        
        void SetMesh()
        {
            if(!meshNeedsToBeSet) return;
            mesh.SetVertices(tempVertices);
            mesh.SetUVs(0, tempUV);
            mesh.SetColors(tempColors);
            mesh.SetIndices(ProtaUnityConstant.rectMeshTriangles, MeshTopology.Triangles, 0);
            mesh.bounds = tempBounds;
            meshNeedsToBeSet = false;
        }
        
        void RemoveTempObjectsWhenTimesUp()
        {
            // 100 帧以后, 临时对象会被清除.
            if(thisFrame - lastTempCreatedTime > 100f)
            {
                tempVertices = null;
                tempUV = null;
                tempColors = null;
            }
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
        
        
        // ====================================================================================================
        // ====================================================================================================
        
    }
}
