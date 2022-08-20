
using UnityEngine;

namespace Prota.Editor
{
    public static partial class Log
    {
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