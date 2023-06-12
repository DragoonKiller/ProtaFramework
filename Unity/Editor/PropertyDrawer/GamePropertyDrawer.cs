using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;
using System;
using System.Runtime.Serialization;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(GamePropertyList))]
    public class GamePropertyListDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            root.AddChild(new PropertyField(property.SubField("properties")));
            
            return root;
        }
    }
    
    [CustomPropertyDrawer(typeof(GameProperty))]
    public class GamePropertyPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            if(property.IsManagedRef()) throw new Exception("[ManagedReference] for [GameProperty] is not supported.");
            
            root.SetHorizontalLayout();
            root.AddChild(new VisualElement().PassValue(out var info)
                .SetHorizontalLayout()
                .SetGrow()
                .AddChild(new TextField().PassValue(out var nameField)
                    .WithBinding(property.SubBackingField("name"))
                    .SetGrow()
                    .SetMinWidth(100)
                )
                .AddChild(new VisualElement().PassValue(out var instInfo)
                    .SetHorizontalLayout()
                    .AddChild(new FloatField().PassValue(out var baseValueField)
                        .WithBinding(property.SubField("_baseValue"))
                        .SetWidth(60)
                    )
                    .AddChild(new TextField().PassValue(out var actualValueField)
                        .SetWidth(60)
                        .SetNoInteraction()
                    )
                    .AddChild(new EnumField().PassValue(out var behaviourField)
                        .WithBinding(property.SubBackingField("behaviour"))
                        .SetWidth(50)
                    )
                )
            );
            
            actualValueField.ReactOnChange(s => {
                var b = property.SubBackingField("behaviour");
                var display = (PropertyBehaviour)b.enumValueIndex switch {
                    PropertyBehaviour.Float => ProeprtyDisplay.Float,
                    PropertyBehaviour.Int => ProeprtyDisplay.Int,
                    PropertyBehaviour.Bool => ProeprtyDisplay.TrueOrFalse,
                    _ => throw new ArgumentOutOfRangeException()
                };
                s.value = GameProperty.ToString(display, property.SubBackingField("value").floatValue);
            }, behaviourField, baseValueField, nameField);
            
            root.Bind(property.serializedObject);
            
            return root;
        }
        
    }
    
}
