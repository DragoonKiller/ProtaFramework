using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;
using System;

using Behaviour = Prota.Unity.GameProperty.Behaviour;
using Display = Prota.Unity.GameProperty.Display;

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
                    .SetNoInteraction()
                    .SetGrow()
                    .SetMinWidth(100)
                )
                .AddChild(new VisualElement().PassValue(out var instInfo)
                    .SetHorizontalLayout()
                    .AddChild(new FloatField().PassValue(out var baseValueField)
                        .WithBinding(property.SubField("_baseValue"))
                        .SetWidth(60)
                    )
                    .AddChild(new FloatField().PassValue(out var actualValueField)
                        .WithBinding(property.SubBackingField("value"))
                        .SetWidth(60)
                        .SetNoInteraction()
                    )
                    .AddChild(new TextField().PassValue(out var actualValueDisplayField)
                        .SetWidth(60)
                        .SetNoInteraction()
                    )
                    .AddChild(new EnumField().PassValue(out var behaviourField)
                        .WithBinding(property.SubBackingField("behaviour"))
                        .SetWidth(50)
                    )
                )
            );
            
            actualValueDisplayField.ReactOnChange(s => {
                
                if(!Application.isPlaying)
                {
                    actualValueField.value = baseValueField.value;
                }
                
                var b = property.SubBackingField("behaviour");
                var display = (Behaviour)b.enumValueIndex switch {
                    Behaviour.Float => Display.Float,
                    Behaviour.Int => Display.Int,
                    Behaviour.Bool => Display.TrueOrFalse,
                    _ => throw new ArgumentOutOfRangeException()
                };
                s.value = GameProperty.ToString(display, actualValueField.value);
            }, baseValueField);
            
            root.Bind(property.serializedObject);
            
            // void TagDirty() => EditorUtility.SetDirty(property.serializedObject.targetObject);
            // 
            // nameField.OnValueChange((ChangeEvent<string> e) =>{
            //     if(e.newValue != e.previousValue) TagDirty();
            // });
            // baseValueField.OnValueChange((ChangeEvent<float> e) =>{
            //     if(e.newValue != e.previousValue) TagDirty();
            // });
            // behaviourField.OnValueChange((ChangeEvent<Enum> e) => {
            //     if(e.newValue != e.previousValue) TagDirty();
            // });
            // actualValueField.OnValueChange((ChangeEvent<float> e) => {
            //     if(e.newValue != e.previousValue) TagDirty();
            // });
            // 
            
            return root;
        }
        
    }
    
}
