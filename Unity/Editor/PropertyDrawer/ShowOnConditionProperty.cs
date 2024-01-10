using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(ShowWhenAttribute))]
    public class ShowWhenDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(ShouldDraw(property)) return EditorGUI.GetPropertyHeight(property, label, true);
            return 0;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool shouldDraw = ShouldDraw(property);
            if (shouldDraw) EditorGUI.PropertyField(position, property, label, true);
        }

        bool ShouldDraw(SerializedProperty property)
        {
            var attr = fieldInfo.GetCustomAttribute<ShowWhenAttribute>();
            return CheckShow(attr.names, property.serializedObject);
        }
        
        bool CheckShowSingle(string name, SerializedObject obj)
        {
            var prop = obj.FindProperty(name);
            if(prop != null && prop.propertyType == SerializedPropertyType.Boolean) return prop.boolValue;
            if(prop != null && prop.propertyType == SerializedPropertyType.ObjectReference) return prop.objectReferenceValue != null;
            
            var pref = obj.targetObject.ProtaReflection();
            bool hasMethod = pref.type.HasMethod(name);
            if(hasMethod && pref.Call(name).PassValue(out var st) != null && st is bool k && k) return true;
            
            return false;
        }
        
        bool CheckShow(string[] names, SerializedObject obj)
        {
            foreach (var name in names)
            {
                if(!CheckShowSingle(name, obj)) return false;
            }
            
            return true;
        }
    }
}
