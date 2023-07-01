using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Linq;

using Prota.Unity;
using System.Collections.Generic;
using NUnit.Framework;

namespace Prota.Editor
{
    public class ProtaInspector : EditorWindow
    {
        static bool useLogicalOrder
        {
            get => EditorPrefs.GetBool("prota::ProtaInspector_UseLogicalOrder", true);
            set => EditorPrefs.SetBool("prota::ProtaInspector_UseLogicalOrder", value);
        }
        
        static class SelectCache
        {
            readonly static List<(GameObject g, int i)> recordSelect = new List<(GameObject, int)>();
            public static bool TryFindSelect(GameObject g, out int index)
            {
                index = 0;
                if(g == null) return false;
                for(int i = 0; i < recordSelect.Count; i++)
                {
                    if (recordSelect[i].g != g) continue;
                    index = recordSelect[i].i;
                    return true;
                }
                return false;
            }
            public static bool UpdateOrAdd(GameObject g, int select)
            {
                if(g == null) return false;
                for(int i = 0; i < recordSelect.Count; i++)
                {
                    if (recordSelect[i].g != g) continue;
                    recordSelect[i] = (g, select);
                    return true;
                }
                
                if(recordSelect.Count > 10) recordSelect.RemoveAt(0);
                recordSelect.Add((g, select));
                return false;
            }
        }
        
        
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
        VisualElement invalidPart;
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
                    .AddChild(invalidPart = new VisualElement() { name = "invalidPart" }
                        .AddChild(new Label("GameObject contains invalid components")
                            .SetTextColor(Color.red)
                        )
                    )
                    .AddChild(componentListPart = new VisualElement() { name = "componentListPart" })
                    .AddChild(new VisualElement().AsHorizontalSeperator(2))
                    .AddChild(componentContentPart = new VisualElement() { name = "componentContentPart" }
                        .SetGrow()
                    )
                    .AddChild(new VisualElement().AsHorizontalSeperator(2))
                    .AddChild(copyPastePart = new VisualElement() { name = "settingsPart" }
                        .AddChild(new VisualElement().SetHorizontalLayout()
                            .AddChild(new Toggle("use logical order") { name = "useLogicalOrder" })
                        )
                    )
                    .AddChild(copyPastePart = new VisualElement() { name = "copyPastePart" }
                        .AddChild(new VisualElement().SetHorizontalLayout()
                            .AddChild(new Button(CopyComponent) { text = "copy" })
                            .AddChild(new Button(PasteComponentValues) { text = "paste" })
                            .AddChild(new Button(ClearInvalidComponents) { text = "clear" })
                        )
                        .AddChild(new VisualElement().SetHorizontalLayout()
                            .AddChild(new Button(DeleteComponent) { text = "delete" })
                        )
                    )
                )
                .AddChild(normalPart = new VisualElement() { name = "normalPart" })
                .AddChild(noSelectedPart = new VisualElement() { name = "noSelectedPart" }
                    .AddChild(new Label("No Selected Object"))
                );
            
            gameObjectInspectorPart.SetVisible(false);
            normalPart.SetVisible(false);
            noSelectedPart.SetVisible(true);
            
            root.Q<Toggle>("useLogicalOrder").value = useLogicalOrder;
            root.Q<Toggle>("useLogicalOrder").RegisterValueChangedCallback(x => {
                useLogicalOrder = x.newValue;
                RebuildInspector();
            });
            
            return root;
        }
        
        void OnSelectionChanged()
        {
            if(Selection.activeGameObject != null)
            {
                if(SelectCache.TryFindSelect(Selection.activeGameObject, out var index))
                {
                    curSelect = index;
                }
                else
                {
                    SelectCache.UpdateOrAdd(Selection.activeGameObject, curSelect);
                }
            }
            
            
            hover = -1;
            
            RebuildInspector();
        }

        void RebuildInspector()
        {
            var objects = Selection.objects;
            if(objects.Length == 0) return;
            
            inspectTarget = new SerializedObject(objects);
            
            componentContentPart.Clear();
            
            bool hasInvalidData = false;
            var allAreGameObjects = objects.Where(x => x is GameObject).Count() == objects.Length;
            if(allAreGameObjects)
            {
                componentListPart.Clear();
                componentContentPart.Clear();
                SetupComponentData(out hasInvalidData);
                CreateGameObjectInspectorElement(inspectTarget);
            }
            else
            {
                var element = CreateNormalInspectorElement(inspectTarget);
                normalPart.Clear();
                normalPart.AddChild(element);
            }
            
            invalidPart.SetVisible(hasInvalidData);
            copyPastePart.SetVisible(curSelect.In(0, (groups?.Count ?? 0) - 1));
            gameObjectInspectorPart.SetVisible(allAreGameObjects);
            normalPart.SetVisible(!allAreGameObjects);
            noSelectedPart.SetVisible(false);
            
        }
        
        static VisualElement CreateNormalInspectorElement(SerializedObject inspectTarget)
        {
            var rt = new ScrollView();
            
            UnityEditor.Editor editor = null;
            if(inspectTarget.targetObjects.IsAllOfType<Texture>())
            {
                var importers = inspectTarget.targetObjects
                    .Select(x => AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(x)))
                    .ToArray();
                editor = UnityEditor.Editor.CreateEditor(importers);
            }
            else
            {
                editor = UnityEditor.Editor.CreateEditor(inspectTarget.targetObjects);
            }
            
            
            if(editor.GetType().Name != "GenericInspector")
            {
                editor.ProtaReflection().Set("m_SerializedObject", inspectTarget);
                var element = editor.CreateInspectorGUI();
                if(element != null)
                {
                    rt.AddChild(element);
                }
                else
                {
                    rt.AddChild(new IMGUIContainer(() => editor.OnInspectorGUI()));
                }
            }
            else
            {
                InspectorElement.FillDefaultInspector(rt, inspectTarget, editor);
            }
            
            rt.Bind(inspectTarget);
            return rt.SetGrow().SetMargin(6, 4, 4, 4);
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
        
        static Dictionary<Type, int> priority = new Dictionary<Type, int> {
            [typeof(Transform)] = -1000,
            [typeof(Rigidbody)] = -100,
            [typeof(Rigidbody2D)] = -100,
            [typeof(Collider)] = -100,
            [typeof(Collider2D)] = -100,
            [typeof(Renderer)] = -20,
            [typeof(UnityEngine.UI.Graphic)] = -10,
            [typeof(CanvasGroup)] = -10,
            [typeof(CanvasRenderer)] = -10,
            [typeof(ECS)] = 1000,
            [typeof(ERoot)] = 1000,
            [typeof(EComponent)] = 1001,
            [typeof(DataBinding)] = 10000,
            [typeof(GamePropertyList)] = 10000,
            [typeof(ResourceBinding)] = 10000,
            [typeof(DontDestroyOnLoad)] = 10000,
        };
        static int GetPriority(Type x)
        {
            if(priority.TryGetValue(x, out var p)) return p;
            return 0;
        }
            
        void SetupComponentData(out bool hasInvalidData)
        {
            hasInvalidData = false;
            var gameObjects = inspectTarget.targetObjects.Cast<GameObject>();
            var components = new List<ComponentInfo>();
            foreach(var g in gameObjects)
            {
                var count = new Dictionary<Type, int>();
                foreach(var c in g.GetComponents<Component>())
                {
                    if(c == null)
                    {
                        hasInvalidData = true;
                        continue;
                    }
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
            
            // sort the group.
            if(useLogicalOrder)
            {
                var _ = TempDict.Get<Type, int>(out var indexOfType);
                for(int i = 0; i < groups.Count; i++) indexOfType[groups[i].type] = i;
                
                groups.Sort((x, y) => {
                    var p1 = GetPriority(x.type);
                    var p2 = GetPriority(y.type);
                    if(p1 != p2) return p1.CompareTo(p2);
                    return indexOfType[x.type].CompareTo(indexOfType[y.type]);
                });
            }
            
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
            SelectCache.UpdateOrAdd(inspectTarget.targetObject as GameObject, i);
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
        
        void ClearInvalidComponents()
        {
            Undo.RecordObjects(Selection.objects, "Clear Invalid Components");
            foreach(var g in Selection.objects)
            {
                if(!(g is GameObject gx)) continue;
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gx);
            }
            OnSelectionChanged();
            Debug.Log("done.");
        }
        
        void PasteComponentValues()
        {
            if(components == null || components.Count == 0) return;
            Undo.RecordObjects(targetObjects[curSelect].ToArray(), "Paste Component Values");
            foreach(var c in components)
            {
                UnityEditorInternal.ComponentUtility.PasteComponentValues(c.component);
            }
        }
        
        void DeleteComponent()
        {
            if(targetObjects == null || targetObjects.Count == 0) return;
            Undo.RecordObjects(targetObjects[curSelect].ToArray(), "Delete Component");
            foreach(var c in targetObjects[curSelect])
            {
                if(c is Transform) continue;
                UnityEngine.Object.DestroyImmediate(c);
            }
            OnSelectionChanged();
        }
    }
}
