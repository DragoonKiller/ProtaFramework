using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;

using Prota.Unity;
using System;

namespace Prota.Editor
{
    [CustomPropertyDrawer(typeof(GameProperty))]
    public class GamePropertyPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            if(fieldInfo.GetCustomAttribute<SerializeReference>() == null)
                Debug.LogError($"GameProperty can only be used on fields with the SerializeReference attribute. Field: {fieldInfo.Name}");
            
            var obj = property.managedReferenceValue as GameProperty;
            
            root.SetHorizontalLayout();
            root.AddChild(new Toggle() { value = obj != null }.PassValue(out var toggle)
                .SetWidth(18)
            ).AddChild(new VisualElement().PassValue(out var info)
                .SetHorizontalLayout()
                .SetGrow()
                .AddChild(
                    new TextField() {
                        value = obj == null || obj.name.NullOrEmpty()
                            ? property.displayName
                            : obj.name
                        }.PassValue(out var nameField)
                    .SetGrow()
                )
                .AddChild(new Label("*").PassValue(out var tag)
                    .SetMargin(2, -4, 0, 0)
                )
                .AddChild(new VisualElement().PassValue(out var instInfo)
                    .SetHorizontalLayout()
                    .AddChild(new FloatField(){ value = obj?.baseValue ?? 0 }.PassValue(out var baseValueField)
                        .SetWidth(80)
                    )
                    .AddChild(new TextField().PassValue(out var actualValueField)
                        .SetWidth(80)
                        .SetNoInteraction()
                    )
                    .AddChild(new EnumField(obj == null ? PropertyBehaviour.Float : obj.behaviour).PassValue(out var behaviourField)
                        .SetWidth(80)
                    )
                )
            );
            
            
            toggle.OnValueChange<Toggle, bool>(e => {
                Undo.RecordObject(property.serializedObject.targetObject, "Change GameProperty");
                if(e.newValue)
                {
                    property.managedReferenceValue = obj = new GameProperty();
                    behaviourField.value = PropertyBehaviour.Float;
                    baseValueField.value = obj.baseValue;
                    SyncValueString();
                }
                else
                {
                    property.managedReferenceValue = null;
                }
                instInfo.SetVisible(e.newValue);
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            });
            
            nameField.OnValueChange<TextField, string>(e => {
                Undo.RecordObject(property.serializedObject.targetObject, "Change GameProperty");
                obj.ProtaReflection().Set("name", e.newValue);
                SyncNameProp();
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                if(e.newValue.NullOrEmpty()) nameField.SetValueWithoutNotify(property.displayName);
            });
            
            baseValueField.OnValueChange<FloatField, float>(e => {
                Undo.RecordObject(property.serializedObject.targetObject, "Change GameProperty");
                obj.baseValue = e.newValue;
                SyncValueString();
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            });
            
            behaviourField.OnValueChange<EnumField, Enum>(e => {
                Undo.RecordObject(property.serializedObject.targetObject, "Change GameProperty");
                obj.ProtaReflection().Set("behaviour", e.newValue);
                SyncValueString();
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.UpdateIfRequiredOrScript();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            });
            
            SyncNameProp();
            SyncValueString();
            instInfo.SetVisible(obj != null);
        
            return root;
            
            void SyncValueString()
            {
                if(obj == null) return;
                var display = obj.behaviour switch {
                    PropertyBehaviour.Float => ProeprtyDisplay.Float,
                    PropertyBehaviour.Int => ProeprtyDisplay.Int,
                    PropertyBehaviour.Bool => ProeprtyDisplay.TrueOrFalse,
                    _ => throw new ArgumentOutOfRangeException()
                };
                actualValueField.value = obj.ToString(display);
            }
            
            void SyncNameProp()
            {
                if(obj == null)
                {
                    nameField.SetNoInteraction();
                    return;
                }
                
                nameField.SetInteractable();
                tag.text = obj.name.NullOrEmpty() ? "*" : "";
            }
            
        }
        
    }
    
}
