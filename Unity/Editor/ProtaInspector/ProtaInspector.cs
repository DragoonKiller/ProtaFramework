using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Linq;

using Prota.Unity;
using System.Collections.Generic;

namespace Prota.Editor
{
    public class ProtaInspector : EditorWindow
    {
        static string serializedCopy;
        
        [MenuItem("ProtaFramework/Window/Prota Inspector")]
        static void ShowWindow()
        {
            ProtaInspector wnd = GetWindow<ProtaInspector>();
            wnd.titleContent = new GUIContent("Prota Inspector");
        }
        
        struct ComponentInfo
        {
            public Component component;
            public int index;
            public Type type => component.GetType();
        }
        
        VisualElement root;
        VisualElement normalPart;
        VisualElement gameObjectInspectorPart;
        VisualElement componentListPart;
        VisualElement componentContentPart;
        VisualElement copyPastePart;
        VisualElement noSelectedPart;
        readonly List<(Type type, int index)> groups = new List<(Type type, int index)>();
        readonly Dictionary<int, List<Component>> targetObjects = new Dictionary<int, List<Component>>();
        readonly List<ComponentInfo> components = new List<ComponentInfo>();
        SerializedObject inspectTarget;
        int curSelect = 0;
        int hover = 0;
        
        
        void OnEnable()
        {
            rootVisualElement.AddChild(CreateInspectorGUI());
            Selection.selectionChanged += OnSelectionChanged;
        }
        
        void OnDisable()
        {
            if(rootVisualElement.Contains(root)) rootVisualElement.Remove(root);
            Selection.selectionChanged -= OnSelectionChanged;
        }
        
        void Update()
        {
        }

        VisualElement CreateInspectorGUI()
        {
            root = new VisualElement() { name = "root" }
                .SetGrow()
                .AddChild(gameObjectInspectorPart = new VisualElement() { name = "inspectorPart" }
                    .SetGrow()
                    .AddChild(componentListPart = new VisualElement() { name = "componentListPart" })
                    .AddChild(new VisualElement().AsHorizontalSeperator(2))
                    .AddChild(componentContentPart = new VisualElement() { name = "componentContentPart" }
                        .SetGrow()
                    )
                    .AddChild(new VisualElement().AsHorizontalSeperator(2))
                    .AddChild(copyPastePart = new VisualElement() { name = "copyPastePart" }
                        .SetHorizontalLayout()
                        .AddChild(new Button(CopyComponent) { text = "copy" })
                        .AddChild(new Button(PasteComponentValues) { text = "paste" })
                    )
                )
                .AddChild(normalPart = new VisualElement() { name = "normalPart" })
                .AddChild(noSelectedPart = new VisualElement() { name = "noSelectedPart" }
                    .AddChild(new Label("No Selected Object"))
                );
            
            gameObjectInspectorPart.SetVisible(false);
            normalPart.SetVisible(false);
            noSelectedPart.SetVisible(true);
            
            return root;
        }
        
        void OnSelectionChanged()
        {
            curSelect = 0;
            hover = -1;
            RebuildInspector();
        }

        void RebuildInspector()
        {
            var objects = Selection.objects;
            if(objects.Length == 0) return;
            
            inspectTarget = new SerializedObject(objects);
            
            componentContentPart.Clear();
            
            var allAreGameObjects = objects.Where(x => x is GameObject).Count() == objects.Length;
            if(allAreGameObjects)
            {
                componentListPart.Clear();
                componentContentPart.Clear();
                SetupComponentData();
                CreateGameObjectInspectorElement(inspectTarget);
            }
            else
            {
                var element = CreateNormalInspectorElement(inspectTarget);
                normalPart.Clear();
                normalPart.AddChild(element);
            }
            
            copyPastePart.SetVisible(curSelect.In(0, (groups?.Count ?? 0) - 1));
            gameObjectInspectorPart.SetVisible(allAreGameObjects);
            normalPart.SetVisible(!allAreGameObjects);
            noSelectedPart.SetVisible(false);
        }
        
        static VisualElement CreateNormalInspectorElement(SerializedObject inspectTarget)
        {
            return new ScrollView()
                .SetGrow()
                .AddChild(new InspectorElement(inspectTarget));
        }
        
        
        void CreateGameObjectInspectorElement(SerializedObject inspectTarget)
        {
            // for each group, create a button for it.
            for(int _i = 0; _i < groups.Count; _i++)
            {
                var i = _i;
                var gr = groups[i];
                var button = new VisualElement();
                button.OnClick(e => UpdateSelect(i));
                
                var hint = gr.type.GetCustomAttributes(typeof(ProtaHint), true).FirstOrDefault() as ProtaHint;
                
                button.SetHorizontalLayout()
                    .SetGrow()
                    .SetHeight(24)
                    .AddChild(new VisualElement()
                        .SetHorizontalLayout()
                        .SetCentered()
                        .SetGrow()
                        .AddChild(new Image() { image = targetObjects[i][0].FindEditorIcon() }
                            .SetSize(16, 16)
                            .SetMargin(2, 4, 0, 0)
                        )
                        .AddChild(new Label(gr.type.Name)
                            .SetFontSize(13)
                            .SetGrow()
                        )
                        .AddChild(new Label(hint?.content ?? "")
                            .SetFontSize(13)
                        )
                    )
                    .OnHoverLeave((x, enter) => {
                        if(enter) hover = i;
                        else if(hover == i) hover = -1;
                        UpdateColor();
                    });
                
                componentListPart.AddChild(new VisualElement()
                    .AddChild(button)
                    .AddChild(new VisualElement().AsHorizontalSeperator(1))
                );
                
                if(i == curSelect) UpdateSelect(i);
            }
            
            
        }
        
        void SetupComponentData()
        {
            var gameObjects = inspectTarget.targetObjects.Cast<GameObject>();
            var components = new List<ComponentInfo>();
            foreach(var g in gameObjects)
            {
                var count = new Dictionary<Type, int>();
                foreach(var c in g.GetComponents<Component>())
                {
                    var type = c.GetType();
                    if(!count.ContainsKey(type)) count[type] = 0;
                    count[type]++;
                    var info = new ComponentInfo() { component = c, index = count[type] - 1 };
                    components.Add(info);
                }
            }
            
            // group by type and index.
            groups.Clear();
            foreach(var c in components) groups.AddNoDuplicate((c.type, c.index));
            
            // filter all components that are not in the groups.
            targetObjects.Clear();
            for(int i = 0; i < groups.Count; i++)
            {
                var curGroup = groups[i];
                targetObjects.Add(i, new List<Component>().PassValue(out var list));
                list.AddRange(components.Where(x => x.type == curGroup.type && x.index == curGroup.index).Select(x => x.component));
            }
        }
        
        void UpdateComponentContent()
        {
            var serializedObject = new SerializedObject(targetObjects[curSelect].ToArray());
            componentContentPart.Clear();
            componentContentPart.AddChild(CreateNormalInspectorElement(serializedObject));
        }
        
        void UpdateSelect(int i)
        {
            curSelect = i;
            UpdateComponentContent();
            UpdateColor();
        }
                
        void UpdateColor()
        {
            for(int i = 0; i < componentListPart.childCount; i++)
            {
                var button = componentListPart[i];
                if(i == curSelect) button.style.backgroundColor = new StyleColor("#382040FF".ToColor());
                else if(i == hover) button.style.backgroundColor = new StyleColor("#202020FF".ToColor());
                else button.style.backgroundColor = new StyleColor(Color.clear);
            }
        }
        
        void CopyComponent()
        {
            if(targetObjects == null || targetObjects.Count == 0) return;
            UnityEditorInternal.ComponentUtility.CopyComponent(targetObjects[curSelect][0]);
        }
        
        void PasteComponentValues()
        {
            if(components == null || components.Count == 0) return;
            foreach(var c in components) UnityEditorInternal.ComponentUtility.PasteComponentValues(c.component);
        }
                
    }
}
