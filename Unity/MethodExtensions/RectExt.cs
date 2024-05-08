using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        public static bool ContainsInclusive(this Rect rect, Vector2 point)
        {
            return rect.xMin <= point.x
                && point.x <= rect.xMax
                && rect.yMin <= point.y
                && point.y <= rect.yMax;
        }
        
        public static Vector2 BottomLeft(this Rect rect) => rect.min;
        
        public static Vector2 BottomRight(this Rect rect) => new Vector2(rect.xMax, rect.yMin);
        
        public static Vector2 TopLeft(this Rect rect) => new Vector2(rect.xMin, rect.yMax);
        
        public static Vector2 TopRight(this Rect rect) => rect.max;
        
        public static Vector2 TopCenter(this Rect rect) => new Vector2(rect.center.x, rect.yMax);
        
        public static Vector2 BottomCenter(this Rect rect) => new Vector2(rect.center.x, rect.yMin);
        
        public static Vector2 LeftCenter(this Rect rect) => new Vector2(rect.xMin, rect.center.y);
        
        public static Vector2 RightCenter(this Rect rect) => new Vector2(rect.xMax, rect.center.y);
        
        public static float EdgeDistanceToPoint(this Rect rect, Vector2 point)
        {
            if(rect.xMin <= point.x && point.x <= rect.xMax) return (0f).Max(
                (rect.yMin - point.y).Max(point.y - rect.yMax)
            );
            if(point.x < rect.xMin) return (point, rect.BottomLeft()).Length().Min((point, rect.TopLeft()).Length());
            else return (point, rect.BottomRight()).Length().Min((point, rect.TopRight()).Length());
        }
        
        public static float CornerDistanceToPoint(this Rect rect, Vector2 point)
        {
            return (point, rect.BottomLeft()).Length()
                .Min((point, rect.BottomRight()).Length())
                .Min((point, rect.TopLeft()).Length())
                .Min((point, rect.TopRight()).Length())
            ;
        }
        
        public static Rect MoveDown(this Rect x, float a) => new Rect(x.x, x.y - a, x.width, x.height);
        
        public static Rect MoveLeft(this Rect x, float a) => new Rect(x.x - a, x.y, x.width, x.height);
        
        public static Rect MoveRight(this Rect x, float a) => new Rect(x.x + a, x.y, x.width, x.height);
        
        public static Rect MoveUp(this Rect x, float a) => new Rect(x.x, x.y + a, x.width, x.height);
        
        public static Rect Shrink(this Rect x, float l, float r, float b, float t) => new Rect(x.x + l, x.y + b, x.width - l - r, x.height - b - t);
        
        public static Rect Expend(this Rect x, float l, float r, float b, float t) => new Rect(x.x - l, x.y - b, x.width + l + r, x.height + b + t);
        
        public static Rect WithHeight(this Rect x, float a) => new Rect(x.x, x.y, x.width, a);
        
        public static Rect WithWidth(this Rect x, float a) => new Rect(x.x, x.y, a, x.height);
        
        public static Rect Move(this Rect x, Vector2 d) => new Rect(x.position + d, x.size);
        
        public static Vector2 ToLocalPosition(this Rect r, Vector2 d) => new Vector2((d.x - r.x) / r.size.x, (d.y - r.y) / r.size.y);
        
        // return: dx, dy (for top points);
        // shear: angle in degree.
        public static Vector2 ShearOffset(this Rect r, float shear)
        {
            var angle = shear * Mathf.Deg2Rad;
            var dx = r.height * Mathf.Sin(angle);
            var dy = r.width * (1 - Mathf.Cos(angle));
            return new Vector2(dx, dy);
        }
        
        // 最小的覆盖两个矩形的矩形.
        public static Rect BoundingBox(this Rect r, Rect g)
        {
            var xMin = r.xMin.Min(g.xMin);
            var xMax = r.xMax.Max(g.xMax);
            var yMin = r.yMin.Min(g.yMin);
            var yMax = r.yMax.Max(g.yMax);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }
        
        // 把负数的长宽变为正数, 保持矩形的位置不变.
        public static Rect Normalize(this Rect r)
        {
            var min = r.min;
            var max = r.max;
            if(min.x > max.x) (min.x, max.x) = (max.x, min.x);
            if(min.y > max.y) (min.y, max.y) = (max.y, min.y);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        
        
        // anchor: 缩放位置, 是矩形所处坐标系上,和矩形同级的点.
        public static Rect ResizeWithAnchor(this Rect r, Vector2 anchor, Vector2 size)
        {
            var min = r.min - anchor;
            var max = r.max - anchor;
            min = Vector2.Scale(min, size);
            max = Vector2.Scale(max, size);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        
        // 点 p 在 r 坐标系中的相对位置. Rect r 定义坐标系, 左下角 r.min 是原点 (0, 0), 右上角 r.max 是 (1, 1).
        public static Vector2 NormalizedToPoint(this Rect r, Vector2 p)
        {
            return Rect.NormalizedToPoint(r, p);
        }
        
        public static Vector2 PointToNormalized(this Rect r, Vector2 p)
        {
            return Rect.PointToNormalized(r, p);
        }
        
        // 将矩形 r 从 t 定义的坐标系转换到 v 定义的坐标系.
        // 参考坐标系左下角是 (0, 0), 右上角是 (1, 1).
        public static Rect XMap(this Rect r, Rect from, Rect to)
        {
            var min = from.NormalizedToPoint(r.min);
            var max = from.NormalizedToPoint(r.max);
            min = to.min + to.size * min;
            max = to.min + to.size * max;
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        
        // 将矩形 r 从自身所处的坐标系转换到 reference 定义的坐标系.
        public static Rect NormalizedIn(this Rect r, Rect reference)
        {
            return r.XMap(reference, Rect.MinMaxRect(0, 0, 1, 1));
        }
        
        // 把矩形 r 变换为矩形 p, 需要做的缩放和平移操作.
        // 缩放和平移都以 (xmin, ymin) 为原点. 平移使用世界坐标(即两个矩形所在的坐标系).
        public static (Vector2 offset, Vector2 scale) OffsetScaleFor(this Rect r, Rect p)
        {
            var scale = p.size / r.size;
            var offset = p.min - r.min;
            return (offset, scale);
        }
        
        public static void FillWithStandardMeshOrder(this Rect x, Vector2[] arr)
        {
            arr[0] = x.TopLeft();
            arr[1] = x.TopRight();
            arr[2] = x.BottomRight();
            arr[3] = x.BottomLeft();
        }
        
        public static void FillWithStandardMeshOrder(this Rect x, Vector3[] arr)
        {
            arr[0] = x.TopLeft();
            arr[1] = x.TopRight();
            arr[2] = x.BottomRight();
            arr[3] = x.BottomLeft();
        }
    }
}
