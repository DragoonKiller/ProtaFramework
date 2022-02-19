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
        
        public ObjectField dataTableObjectField;
        
        public ObjectField dataTableComponentField;
        
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
                    .AddChild(new Button(() => SetTestData(dataTableComponentField.value as DataTableComponent)) { text = "Sample" })
                )
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
            foreach(var columnData in table.data)
            {
                var column = new VisualElement()
                    .SetVerticalLayout()
                    .SetWidth(120)
                    .AddChild(new Label(columnData.columnName)
                        .SetTextCentered()
                    )
                    .AddChild(new Label(columnData.type.ToString())
                        .SetTextCentered()
                        .SetColor(new Color(.4f, .4f, .4f, 1))
                    );
                viewRoot.Add(column);
                for(int i = 0; i < table.lineCount; i++)
                {
                    var grid = new DataTableGridView(i, columnData.columnId, table);
                    grid.ShowValue(columnData[i]);
                    column.Add(grid);
                }
            }
        }
        
        void ClearView()
        {
            viewRoot.Clear();
        }
        
        void OnAdd(int i)
        {
            for(int x = 0; x < table.columnCount; x++)
            {
                var columnData = table.data[x];
                var columnView = viewRoot[x];
                var grid = new DataTableGridView(i, columnData.columnId, table);
                grid.ShowValue(columnData[i]);
                columnView.Add(grid);
            }
        }
        
        void OnRemove(int i, int swap)
        {
            int viewLine = i + 2;
            for(int x = 0; x < table.columnCount; x++)
            {
                var columnView = viewRoot[x];
                columnView.RemoveAt(columnView.childCount - 1);
                if(viewLine < columnView.childCount)
                {
                    (columnView[viewLine] as DataTableGridView).ShowValue(table.data[x][swap]);
                }
            }
        }
        
        void OnModify(int i, int x, DataValue from, DataValue to)
        {
            var viewline = i + 2;
            (viewRoot[x][viewline] as DataTableGridView).ShowValue(to);
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
                Log.Info($"行[{ x }] 值[{ from } => { to }]");
            };
            
            component.Update();
            
        }
        
    }
}