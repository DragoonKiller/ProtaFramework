using UnityEditor;
using UnityEngine;


namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        static Color recordColor;
        public static void SetColor(this EditorWindow w, Color c)
        {
            recordColor = GUI.color;
            GUI.color = c;
        }
        public static void ResetColor(this EditorWindow w)
        {
            GUI.color = recordColor;
        }
        
        public static void SeperateLine(this EditorWindow w, float height, Color color)
        {
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, height), color);
        }
        
        public static void SeperateLine(this UnityEditor.Editor w, float height, Color color)
        {
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, height), color);
        }
    }
    
}