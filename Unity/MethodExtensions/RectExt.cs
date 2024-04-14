using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        public static Vector2 BottomLeft(this Rect rect) => rect.min;
        
        public static Vector2 BottomRight(this Rect rect) => new Vector2(rect.xMax, rect.yMin);
        
        public static Vector2 TopLeft(this Rect rect) => new Vector2(rect.xMin, rect.yMax);
        
        public static Vector2 TopRight(this Rect rect) => rect.max;
        
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
        
        public static Rect MarginIn(this Rect x, float l, float r, float b, float t) => new Rect(x.x + l, x.y + b, x.width - l - r, x.height - b - t);
        
        public static Rect MarginOut(this Rect x, float l, float r, float b, float t) => new Rect(x.x - l, x.y - b, x.width + l + r, x.height + b + t);
        
        public static Rect WithHeight(this Rect x, float a) => new Rect(x.x, x.y, x.width, a);
        
        public static Rect WithWidth(this Rect x, float a) => new Rect(x.x, x.y, a, x.height);
    }
}
