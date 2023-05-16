
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
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = fieldInfo.GetCustomAttribute<Readonly>();
            if(!ShouldDraw(attr)) return null;
            var res = new PropertyField(property);
            if(ShouldBeReadonly(attr)) res.SetEnabled(false);
            return res;
        }
        
        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;
        
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
