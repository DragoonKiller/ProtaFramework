using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;
using Prota;
using Prota.Unity;
using System.Collections.Generic;

namespace Prota.Editor
{
    public class SerializeReferenceListView<T> : VisualElement
    {
        public SerializeReferenceListView(SerializedProperty arrayProperty, Action<T> onChange = null)
        {
            arrayProperty.AssertNotNull();
            (arrayProperty.isArray).Assert();
            
            var choices = TypeCache.GetTypesDerivedFrom(typeof(T))
                .Where(x => !x.Name.Contains("`"))
                .ToList();
            var choiceNames = choices.Select(x => x.Name).ToList();
            var curSelect = choiceNames.FirstOrDefault();
            
            
            this.hierarchy.Add(new VisualElement()
                .AddChild(new VisualElement().SetHorizontalLayout()
                    .AddChild(new DropdownField(choiceNames, curSelect, null, null)
                        .OnValueChange<DropdownField, string>(x => curSelect = x.newValue)
                        .SetGrow()
                    )
                    .AddChild(
                        new Button(() => {
                            var t = choices.FirstOrDefault(x => x.Name == curSelect);
                            if(t == null) $"Cannot find type { curSelect }".LogError();
                            var newContent = (T)Activator.CreateInstance(t);
                            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
                            var newElement = arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1);
                            newElement.managedReferenceValue = newContent;
                            onChange?.Invoke(newContent);
                            newElement.serializedObject.ApplyModifiedProperties();
                        }) { text = "+" }
                        .SetWidth(60)
                    )
                )
                .AddChild(new PropertyField(arrayProperty))
            );
        }
    }
}
