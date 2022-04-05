using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Prota.Unity;

namespace Prota.Editor
{
    public static partial class MethodExtensions
    {
        public static List<Color> gizmosColors = new List<Color>();
        
        public static Color PushToGizmos(this Color color)
        {
            gizmosColors.Add(Gizmos.color);
            Gizmos.color = color;
            return color;
        }
        
        public static void PopFromGizmos(this ref Color color)
        {
            color = Gizmos.color;
            Gizmos.color = gizmosColors.Last();
            gizmosColors.RemoveLast();
        }
        
        public static List<Color> handleColors = new List<Color>();
        
        public static void PushToHandle(this Color color)
        {
            handleColors.Add(Gizmos.color);
            Gizmos.color = color;
        }
        
        public static void PopFromHandle(this ref Color color)
        {
            color = Gizmos.color;
            Gizmos.color = handleColors.Last();
            handleColors.RemoveLast();
        }
    }
}