using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using Prota.Unity;
using System.Linq;
using System.Collections.Generic;
using System;

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
        
        const int columnWidth = 120;
        
        public ObjectField dataTableObjectField;
        
        public ObjectField dataTableComponentField;
        
        public VisualElement headerRoot;
        
        public VisualElement viewRoot;
        
        
        [MenuItem("ProtaFramework/DataTable/Inspector %5")]
        public static void OpenWindow()
        {
            var window = EditorWindow.GetWindow<DataTableInspector>();
            window.titleContent = new GUIContent("Data Inspector");
        }
        
        void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
            
            rootVisualElement
                .AddChild(new VisualElement()
                    .SetHorizontalLayout()
                    .AddChild(dataTableComponentField = new ObjectField("Component") { objectType = typeof(DataTableComponent) }
                        .OnValueChange<ObjectField, UnityEngine.Object>(x => {
                            table = (x.newValue as DataTableComponent)?.table;
                        })
                    )
                    .AddChild(new Button(() => (dataTableComponentField.value as DataTableComponent)?.table?.Clear()) { text = "Clear" })
                    .AddChild(new Button(() => SetTestData(dataTableComponentField.value as DataTableComponent)) { text = "Sample" })
                )
                .AddChild(new VisualElement().AsHorizontalSeperator(2))
                .AddChild(new VisualElement().SetHorizontalLayout()
                    .AddChild(new Button(CreateLine) { text = "Add Record" })
                )
                .AddChild(headerRoot = new VisualElement()
                    .SetHorizontalLayout()
                )
                .AddChild(new VisualElement().AsHorizontalSeperator(2))
                .AddChild(new ScrollView(ScrollViewMode.VerticalAndHorizontal)
                    .SetGrow()
                    .AddChild(viewRoot = new VisualElement() { name = "ViewRoot" }
                        .SetHorizontalLayout()
                    )
                );
            ;
        }
        
        
        void OnDisable()
        {
            EditorApplication.update -= EditorUpdate;
            rootVisualElement.Clear();
        }
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        Dictionary<int, Action<int, DataValue, DataValue>> callbacks = new Dictionary<int, Action<int, DataValue, DataValue>>();
        
        void Rebind(DataTable original, DataTable x)
        {
            if(original != null)
            {
                foreach(var column in original.data)
                {
                    column.addCallbacks -= OnAdd;
                    column.removeCallbacks -= OnRemove;
                    column.modifyCallbacks -= callbacks[column.columnId];
                }
                ClearView();
            }
            
            if(x != null)
            {
                CreateView();
                foreach(var column in x.data)
                {
                    column.addCallbacks += OnAdd;
                    column.removeCallbacks += OnRemove;
                    column.modifyCallbacks += callbacks[column.columnId] = (i, from, to) => OnModify(i, column.columnId, from, to);
                }
            }
        }
        
        
        
        void CreateView()
        {
            viewRoot.SetChildList(table.data.Count,
                (col) => {
                    var columnData = table.data[col];
                    
                    headerRoot.AddChild(new VisualElement()
                        .SetVerticalLayout()
                        .SetWidth(columnWidth)
                        .AddChild(new TextField() { name = "input" })
                        .AddChild(new Label(columnData.columnName) { name = "columnName" }
                            .SetTextCentered()
                            .SetColor(new Color(.1f, .1f, .15f, 1))
                        )
                        .AddChild(new Label(columnData.type.ToString()) { name = "columnType" }
                            .SetTextCentered()
                            .SetColor(new Color(.2f, columnData.isIndex ? .3f : .2f, .2f, 1))
                        )
                    );
                        
                    var column = new VisualElement()
                        .SetVerticalLayout()
                        .SetWidth(columnWidth);
                        
                    for(int i = 0; i < table.lineCount; i++)
                    {
                        var grid = new DataTableGridView(i, columnData.columnId, table);
                        grid.ShowValue(columnData[i]);
                        column.Add(grid);
                    }
                    return column;
                },
                (col, g) => {
                    headerRoot[col].SetVisible(false);
                    g.SetVisible(false);
                },
                (col, g) => {
                    headerRoot[col].Q<Label>("columnName").text = table.data[col].columnName;
                    headerRoot[col].Q<Label>("columnType").text = table.data[col].type.ToString();
                    headerRoot[col].SetVisible(true);
                    g.SetVisible(true);
                }
            );
        }
        
        void ClearView()
        {
            foreach(var child in viewRoot.Children())
            {
                child.SetVisible(false);
            }
        }
        
        void OnAdd(int i)
        {
            for(int x = 0; x < table.columnCount; x++)
            {
                var columnData = table.data[x];
                var columnView = viewRoot[x];
                
                var grid = null as DataTableGridView;
                if(i < columnView.childCount)
                {
                    grid = columnView[i] as DataTableGridView;
                    grid.SetVisible(true);
                }
                else
                {
                    columnView.AddChild(grid = new DataTableGridView(i, columnData.columnId, table));
                }
                
                grid.ShowValue(columnData[i]);
            }
        }
        
        void OnRemove(int i, int swap)
        {
            for(int x = 0; x < table.columnCount; x++)
            {
                var columnView = viewRoot[x];
                columnView[columnView.childCount - 1].SetVisible(false);
                if(i < columnView.childCount)
                {
                    (columnView[i] as DataTableGridView).ShowValue(table.data[x][swap]);
                }
            }
        }
        
        void OnModify(int i, int x, DataValue from, DataValue to)
        {
            (viewRoot[x][i] as DataTableGridView).ShowValue(to);
        }
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        
        void EditorUpdate()
        {
            var curSelection = Selection.activeObject;
            if(curSelection == null) return;
            
            // 根据当前点击的对象刷新列表.
            if(curSelection is GameObject g)
            {
                if(g.TryGetComponent<DataTableComponent>(out var c))
                {
                    dataTableComponentField.value = c;
                }
            }
            
            var curTable = (dataTableComponentField.value as DataTableComponent)?.table;
            if(curTable != table) table = curTable;
        }
        
        // ============================================================================================================
        // ============================================================================================================
        // ============================================================================================================
        
        void CreateLine()
        {
            if(table == null) return;
            table.AddRecord(r =>
            {
                for(int i = 0; i < table.columnCount; i++)
                {
                    var v = headerRoot[i].Q<TextField>("input").value;
                    var type = table[i].type;
                    
                    bool success;
                    switch(type)
                    {
                        case DataType.Int32: success = int.TryParse(v, out var intValue); if(success) r.Add(intValue); break;
                        case DataType.Int64: success = long.TryParse(v, out var longValue); if(success) r.Add(longValue); break;
                        case DataType.Float32: success = float.TryParse(v, out var floatValue); if(success) r.Add(floatValue); break;
                        case DataType.Float64: success = double.TryParse(v, out var doubleValue); if(success) r.Add(doubleValue); break;
                        case DataType.String: success = true; r.Add(v); break;
                        default: continue;
                    }
                    
                    if(!success)
                    {
                        UnityEngine.Debug.Log($"第{ i }列[{ table[i].columnName }] 填写的类型不正确, 应为 [{ type }] 值为 [{ v }]");
                        return;
                    }
                }
            });
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
            
            component.table.DataByName("name").modifyCallbacks += (x, from, to) => {
                UnityEngine.Debug.Log($"行[{ x }] 值[{ from } => { to }]");
            };
            
            component.Update();
            
        }
        
    }
}