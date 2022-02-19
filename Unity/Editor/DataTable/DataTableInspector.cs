using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Prota.Unity;
using System.Linq;
using System.Collections.Generic;

namespace Prota.Editor
{
    public class DataTableInspector : UnityEditor.EditorWindow
    {
        DataTable _table;
        
        DataTable table
        {
            get => _table;
            set {
                var original = _table;
                _table = value;
                Rebind(original, value);
            }
        }
        
        public ObjectField dataTableObjectField;
        
        public ObjectField dataTableComponentField;
        
        [MenuItem("ProtaFramework/DataTable/Inspector %5")]
        public static void OpenWindow()
        {
            var window = EditorWindow.GetWindow<DataTableInspector>();
            window.titleContent = new GUIContent("Data Inspector");
        }
        
        void OnEnable()
        {
            rootVisualElement
                .AddChild(new VisualElement()
                    .SetHorizontalLayout()
                    .AddChild(dataTableComponentField = new ObjectField("Component") { objectType = typeof(DataTableComponent) }
                        .OnValueChange<ObjectField, UnityEngine.Object>(x => table = (x.newValue as DataTableComponent).table)
                    )
                    .AddChild(new Button(() => SetTestData(dataTableComponentField.value as DataTableComponent)) { text = "Sample" })
                )
                .AddChild(new ScrollView(ScrollViewMode.VerticalAndHorizontal)
                    .SetGrow()
                );
            ;
        }
        
        
        void OnDisable()
        {
            rootVisualElement.Clear();
        }
        
        void Rebind(DataTable original, DataTable x)
        {
            
        }
        
        void Update()
        {
            var curSelection = Selection.activeObject;
            if(curSelection == null) return;
            
            // 根据当前点击的对象刷新列表.
            else if(curSelection is GameObject g)
            {
                if(g.TryGetComponent<DataTableComponent>(out var c))
                {
                    dataTableComponentField.value = c;
                }
            }
        }
        
        
        
        void SetTestData(DataTableComponent component)
        {
            var schema = new DataSchema(new List<DataEntry>{
                new DataEntry("id", DataType.Int64, true),
                new DataEntry("name", DataType.String, true),
                new DataEntry("owner", DataType.Int64, false),
                new DataEntry("x", DataType.Float32, false),
                new DataEntry("y", DataType.Float32, false),
                new DataEntry("z", DataType.Float32, false),
            });
            
            component.table = new DataTable("TestData", schema);
            
            component.table.AddRecord(a => a
                .Add(0L)
                .Add("PlayerA")
                .Add(0L)
                .Add(0f)
                .Add(1f)
                .Add(2f)
            );
            
            component.table.AddRecord(a => a
                .Add(1L)
                .Add("PlayerB")
                .Add(1L)
                .Add(5f)
                .Add(6f)
                .Add(7f)
            );
            
            component.Update();
            Repaint();
        }
        
    }
}