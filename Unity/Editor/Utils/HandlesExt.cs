using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Prota.Editor
{
    public static partial class UnityMethodExtensions
    {
        public static void DrawHandles(this Rect rect, Vector3? _offset = null, Vector3? _scale = null)
        {
            var offset = _offset ?? Vector3.zero;
            var scale = _scale ?? Vector3.one;
            var scaledRect = new Rect(rect.center - rect.size * scale * 0.5f, rect.size * scale); 
            Handles.DrawLine((Vector3)scaledRect.BottomLeft() + offset, (Vector3)scaledRect.BottomRight() + offset);
            Handles.DrawLine((Vector3)scaledRect.BottomRight() + offset, (Vector3)scaledRect.TopRight() + offset);
            Handles.DrawLine((Vector3)scaledRect.TopRight() + offset, (Vector3)scaledRect.TopLeft() + offset);
            Handles.DrawLine((Vector3)scaledRect.TopLeft() + offset, (Vector3)scaledRect.BottomLeft() + offset);
        }
        
        public static void DrawHandlesCross(this Rect rect, Vector3? _offset = null)
        {
            var offset = _offset ?? Vector3.zero;
            Handles.DrawLine((Vector3)rect.TopRight() + offset, (Vector3)rect.BottomLeft() + offset);
            Handles.DrawLine((Vector3)rect.TopLeft() + offset, (Vector3)rect.BottomRight() + offset);
        }
    }
}