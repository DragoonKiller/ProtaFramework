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
using Unity.Properties;

[CustomPropertyDrawer(typeof(ReferenceEditor))]
public class ReferenceEditorDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property) 
    {
        var elementType = this.GetActualFieldPropertyType(property);
        var choices = TypeCache.GetTypesDerivedFrom(elementType);
        
        var names = choices.Select(x => x.Name).Where(x => !x.Contains("`")).ToList();
        names.Insert(0, "null");
        
        var value = property.managedReferenceValue;
        
        var root = new VisualElement();
        
        if(!(attribute as ReferenceEditor).flat)
        {
            var oriProp = new PropertyField(property, property.displayName);
            root.AddChild(new VisualElement().SetHorizontalLayout()
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
            );
            root.AddChild(oriProp);
        }
        else
        {
            root.AddChild(new Box().PassValue(out var box));
            
            box.AddChild(new VisualElement()
                .SetHorizontalLayout()
                .AddChild(new Toggle().PassValue(out var toggle)
                    .SetMargin(0, 3, 0, 0)
                )
                // .AddChild(new Label("[" + property.propertyType + "]"))
                .AddChild(new Label(property.displayName).SetTextBold())
            );
            
            box.AddChild(new VisualElement().PassValue(out var container));
            
            void Redraw()
            {
                var it = property.Copy();
                var end = it.GetEndProperty();
                while(it.NextVisible(true) && !SerializedProperty.EqualContents(it, end))
                {
                    container.AddChild(new PropertyField(it, it.displayName));
                }
            }
            
            toggle.value = property.managedReferenceValue != null;
            toggle.OnValueChange((ChangeEvent<bool> e) =>
            {
                if (e.newValue)
                {
                    property.managedReferenceValue = Activator.CreateInstance(elementType);
                }
                else
                {
                    property.managedReferenceValue = null;
                }
                
                property.serializedObject.ApplyModifiedProperties();
                container.Clear();
                Redraw();
                root.Bind(property.serializedObject);
            });
            
            Redraw();
        }
        
        
        return root;
    }
}
