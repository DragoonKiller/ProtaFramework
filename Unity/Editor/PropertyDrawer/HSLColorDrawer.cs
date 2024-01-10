using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(HSLColor))]
    public class HSLColorDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hslValue = new HSLColor();
            hslValue.hue = property.FindPropertyRelative("hue").floatValue;
            hslValue.saturation = property.FindPropertyRelative("saturation").floatValue;
            hslValue.lightness = property.FindPropertyRelative("lightness").floatValue;
            
            var color = hslValue.ToColor();
            var newColor = EditorGUI.ColorField(position, label, color, true, true, false);
            
            if(newColor != color)
            {
                var newHslValue = new HSLColor(newColor);
                property.FindPropertyRelative("hue").floatValue = newHslValue.hue;
                property.FindPropertyRelative("saturation").floatValue = newHslValue.saturation;
                property.FindPropertyRelative("lightness").floatValue = newHslValue.lightness;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
