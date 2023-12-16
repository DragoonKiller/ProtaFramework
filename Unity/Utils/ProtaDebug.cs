
using UnityEngine;
using Prota.Unity;

namespace Prota.Unity
{
    public static partial class ProtaDebug
    {
        public static void DrawBox2D(Vector3 pos, Vector2 size, float rotation, Color? color = null, float? duration = null)
        {
            if(color == null) color = Color.green;
            if(duration == null) duration = 0;
            
            var halfSize = size / 2;
            var bottomLeft = -halfSize;
            var topRight = halfSize;
            var bottomRight = new Vector2(halfSize.x, -halfSize.y);
            var topLeft = new Vector2(-halfSize.x, halfSize.y);
            var bottomLeftRotated = pos + bottomLeft.Rotate(rotation).ToVec3();
            var topRightRotated = pos + topRight.Rotate(rotation).ToVec3();
            var bottomRightRotated = pos + bottomRight.Rotate(rotation).ToVec3();
            var topLeftRotated = pos + topLeft.Rotate(rotation).ToVec3();
            Debug.DrawLine(bottomLeftRotated, bottomRightRotated, color.Value, duration.Value);
            Debug.DrawLine(bottomRightRotated, topRightRotated, color.Value, duration.Value);
            Debug.DrawLine(topRightRotated, topLeftRotated, color.Value, duration.Value);
            Debug.DrawLine(topLeftRotated, bottomLeftRotated, color.Value, duration.Value);
        }
        
        public static void DrawArrow(Vector3 from, Vector3 to, Color? _color = null, float? _duration = null, float? _size = null)
        {
            var color = _color ?? Color.green;
            var duration = _duration ?? 0;
            var size = _size ?? 0.1f;
            Debug.DrawLine(from, to, color, duration);
            var arrowSide = to.To(from).normalized.ToVec2() * size;
            Debug.DrawLine(to, to + arrowSide.Rotate(30).ToVec3(), color, duration);
            Debug.DrawLine(to, to + arrowSide.Rotate(-30).ToVec3(), color, duration);
        }
        
        
        public static void DrawDirLine(Vector3 from, Vector3 to, Color? color = null, float? duration = null)
        {
            if(color == null) color = Color.green;
            if(duration == null) duration = 0;
            
            const int segment = 10;
            float a = 0.5f;
            Debug.DrawLine(from, (from, to).Lerp(a));
            for(int i = 0; i < segment; i++)
            {
                var f = a;
                a = 1 - (1 - a) * 0.5f;
                Debug.DrawLine((from, to).Lerp((f + a) / 2), (from, to).Lerp(a), color.Value, duration.Value);
            }
        }
        
    }
}
