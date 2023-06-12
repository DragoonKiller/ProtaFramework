using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Text;
using System.IO;

namespace Prota.Editor
{
    
    
    public static partial class ProtaEditorCommands
    {
        const string classTemplate = 
@"
    public class {0} : Singleton<{0}>
    {{
        protected override void Awake()
        {{
            base.Awake();
        }}
    }}
";

        [MenuItem("Assets/ProtaFramework/Csv/复制代码到剪贴板", priority = 600)]
        static void CopyCsvCode()
        {
            var csvFiles = Selection.objects
                .Where(o => o is TextAsset && AssetDatabase.GetAssetPath(o).EndsWith(".csv"))
                .Select(o => o as TextAsset)
                .ToArray();
            
            var sb = new StringBuilder();
            sb.Append("namespace Data");
            sb.AppendLine();
            
            foreach(var csvFile in csvFiles)
            {
                sb.AppendLine("    public class " + csvFile.name);
                sb.AppendLine("    {");
                
                var csvContent = csvFile.text;
                var csv = new CsvParser(csvContent, 2);
                
                for(int i = 0; i < csv.headerInfo.properties.Count; i++)
                {
                    var name = csv.GetString(-2, i);
                    var desc = csv.GetString(-1, i);
                    var type = csv.gridType[i];
                    var typeName = type == CsvGridType.Float || type == CsvGridType.Int ? "float" : "string";
                    sb.AppendLine($"        public {typeName} {name}; // {desc}");
                }
                
                sb.AppendLine("    }");
                sb.AppendLine();
                
                Debug.Log($"CSV file [{csvFile.name}] at [{ AssetDatabase.GetAssetPath(csvFile) }] copied");
            }
            
            sb.AppendLine("}");
            
            GUIUtility.systemCopyBuffer = sb.ToString();
        }
        
        
    }
}
