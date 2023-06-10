using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Prota.Unity
{
    public static partial class UnityMethodExtensions
    {
        public static Rect WorldRect(this RectTransform tr)
        {
            var corners = new Vector3[4];
            tr.GetWorldCorners(corners);
            var min = corners[0];
            var max = corners[2];
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
