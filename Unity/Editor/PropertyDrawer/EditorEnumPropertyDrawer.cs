using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;
using System;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(EnumEditor))]
    public class EditorEnumDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Type enumType = this.GetActualFieldPropertyType(property);
            
            TextField textField;
            PropertyField propField;
            
            var root = new VisualElement()
                .SetHorizontalLayout()
                .AddChild(textField = new TextField(property.displayName) { value = Enum.GetName(enumType, property.enumValueFlag) }
                    .SetGrow()
                    .SetMaxWidth(300)
                )
                .AddChild(propField = new PropertyField(property) { label = "" }
                    .SetGrow()
                );
                
            textField.OnValueChange((ChangeEvent<string> e) => {
                if(e.newValue == e.previousValue) return;
                var validValue = Enum.TryParse(enumType, e.newValue, true, out var result);
                if(!validValue) property.enumValueFlag = 0;
                else property.enumValueFlag = (int)result;
                // $"{ validValue } :: { result } :: { (int)result }".Log();
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript();
            });
            
            // propField.RegisterValueChangeCallback((SerializedPropertyChangeEvent e) => {
            //     var value = e.changedProperty.enumValueFlag;
            //     var name = Enum.GetName(enumType, value);
            //     textField.value = name;
            // });
            
            propField.SetEnabled(false);
            
            return root;
        }
    }
}
