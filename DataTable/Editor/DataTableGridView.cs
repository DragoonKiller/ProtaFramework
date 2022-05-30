using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

using System.Collections.Generic;


using Prota.Data;

namespace Prota.Editor
{
    public class DataTableGridView : VisualElement
    {
        VisualElement typeIndicator;
        
        TextField valueIndicator;
        
        int line;
        
        int column;
        
        DataTable table;
        
        
        static Dictionary<DataType, Color> typeToColor = new Dictionary<DataType, Color>{
            { DataType.Int32, new Color(1f, .6f, .6f, 1) },
            { DataType.Int64, new Color(1f, .8f, .8f, 1) },
            { DataType.Float32, new Color(1f, 1f, .5f, 1) },
            { DataType.Float64, new Color(1f, 1f, .8f, 1) },
            { DataType.String, new Color(.6f, .9f, .6f, 1) },
            { DataType.None, new Color(0f, 0f, 0f, 1) },
        };
        
        
        public void ShowValue(DataValue value)
        {
            var color = typeToColor[value.type];
            this.typeIndicator.SetColor(color);
            
            valueIndicator.value = value.stringPresentation;
        }
        
        public void WriteValue(string value)
        {
            
        }
        
        public DataTableGridView(int line, int column, DataTable table)
        {
            this.line = line;
            this.column = column;
            this.table = table;
         
            this.SetHorizontalLayout()
                .SetGrow();
            
            
            hierarchy.Add(typeIndicator = new VisualElement()
                .SetWidth(2)
                .SetHeight(20)
            );
            
            hierarchy.Add(valueIndicator = new TextField() { }
                .SetWidth(120)
                .SetHeight(20)
                .OnValueChange<TextField, string>(x => {
                    if(string.IsNullOrEmpty(x.newValue))
                    {
                        ShowValue(table.data[column][line]);
                        return;
                    }
                    var type = table.data[column].type;
                    switch(type)
                    {
                        case DataType.Int32:
                            if(int.TryParse(x.newValue, out int i32Value)) table.data[column][line] = i32Value;
                            return;
                        case DataType.Int64:
                            if(long.TryParse(x.newValue, out long i64Value)) table.data[column][line] = i64Value;
                            return;
                        case DataType.Float32:
                            if(float.TryParse(x.newValue, out float f32Value)) table.data[column][line] = f32Value;
                            return;
                        case DataType.Float64:
                            if(double.TryParse(x.newValue, out double f64Value)) table.data[column][line] = f64Value;
                            return;
                        case DataType.String:
                            table.data[column][line] = x.newValue;
                            return;
                        default: return;
                    }
                })
            );
            
            ShowValue(new DataValue(DataType.None, new RawDataValue(0)));
        }

    }
}