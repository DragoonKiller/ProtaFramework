using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;
using System;
using System.Linq;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(TypeEnum))]
    public class TypeEnumDrawer : PropertyDrawer
    {
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property) 
        {
            var attr = this.attribute as TypeEnum;
            var types = TypeCache.GetTypesDerivedFrom(attr.type).Select(x => x.Name).ToList();
            
            if(attr.allowNull) types.Add("null");
            
            TextField textField;
            PropertyField propField;
            
            var root = new VisualElement()
                .SetHorizontalLayout()
                .AddChild(textField = new TextField(property.displayName) { value = property.stringValue }
                    .SetGrow()
                    .SetMaxWidth(300)
                )
                .AddChild(propField = new PropertyField(property) { label = "" }
                    .SetGrow()
                );
                
            textField.OnValueChange((ChangeEvent<string> e) => {
                if(e.newValue == e.previousValue) return;
                if(types.Contains(e.newValue)) property.stringValue = e.newValue;
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript();
            });
            
            propField.SetEnabled(false);
            
            return root;
        }
    }
}
