using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;

using Prota.Unity;
using Prota.Editor;
using Prota;

[CustomPropertyDrawer(typeof(ReferenceEditor))]
public class ReferenceEditorDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property) 
    {
        var choices = TypeCache.GetTypesDerivedFrom(this.GetActualFieldPropertyType(property));
        
        var names = choices.Select(x => x.Name).Where(x => !x.Contains("`")).ToList();
        names.Insert(0, "null");
        
        var value = property.managedReferenceValue;
        
        var oriProp = new PropertyField(property, property.name);
        var root = new VisualElement()
            .AddChild(new VisualElement().SetHorizontalLayout()
                .AddChild(new Label("[" + property.propertyType + "] " + property.name)
                    .SetGrow()
                )
                .AddChild(new DropdownField(names, value?.GetType().Name ?? "null")
                    .SetWidth(200)
                    .OnValueChange((ChangeEvent<string> e) => {
                        var i = names.FindIndex(x => x == e.newValue);
                        if(i < 0) throw new ArgumentException(e.newValue);
                        var newContent = i == 0 ? null : Activator.CreateInstance(choices[i - 1]);
                        property.managedReferenceValue = newContent;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.UpdateIfRequiredOrScript();
                        oriProp.BindProperty(property);
                        oriProp.label = names[i];
                    })
                )
            )
            .AddChild(oriProp);
        
        return root;
    }
}
