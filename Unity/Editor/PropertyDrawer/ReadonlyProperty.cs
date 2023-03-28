
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(Readonly))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attr = fieldInfo.GetCustomAttribute<Readonly>();
            if(ShouldDraw(attr)) return EditorGUI.GetPropertyHeight(property, label, true);
            return 0;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = fieldInfo.GetCustomAttribute<Readonly>();
            
            void DrawOrNot()
            {
                if(ShouldDraw(attr)) EditorGUI.PropertyField(position, property, label, true);
            }
            
            if(ShouldBeReadonly(attr))
            {
                GUI.enabled = false;
                DrawOrNot();
                GUI.enabled = true;
            }
            else
            {
                DrawOrNot();
            }
        }
        
        bool ShouldBeReadonly(Readonly attr)
        {
            var isReadonly = false;
            isReadonly |= attr.whenPlaying && Application.isPlaying;
            isReadonly |= attr.whenEditing && !Application.isPlaying;
            return isReadonly;
        }
        
        bool ShouldDraw(Readonly attr)
        {
            bool dontDraw = false;
            dontDraw |= attr.hideWhenEditing && !Application.isPlaying;
            dontDraw |= attr.hideWhenPlaying && Application.isPlaying;
            return !dontDraw;
        }
        
        
    }
}
