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
using System.Text.RegularExpressions;

namespace Prota.Editor
{
    
    public static partial class ProtaEditorCommands
    {
        [MenuItem("CONTEXT/Object/Change Script")]
        public static void ChangeScript(MenuCommand command)
        {
            void ExecuteWithComponent(Component c)
            {
                Debug.LogError("Not Implemented");
            }
            
            void ExecuteWithScriptableObject(ScriptableObject so)
            {
                var matchPattern = new Regex(@"m_Script: [^\n\r]* guid: ([0-9a-f]{32}),");
                
                var path = AssetDatabase.GetAssetPath(so);
                var text = File.ReadAllText(path);
                var result = matchPattern.Match(text);
                if(!result.Success)
                {
                    Debug.LogError($"Not found m_Script pattner in [{ path }]");
                    return;
                }
                
                var matchedGuidSection = result.Groups[1];
                
                var newFile = EditorUtility.OpenFilePanel("Select Script", "Assets", "cs");
                newFile = newFile.FullPathToAssetPath();
                var newGuid = AssetDatabase.AssetPathToGUID(newFile);
                
                Debug.Log($"SelectedFile: [{ newFile }] guid: [{ matchedGuidSection.Value }] => [{ newGuid }]");
                
                var newText = matchPattern.Replace(text, $"m_Script: { newGuid },");
            }
            
            var obj = command.context;
            switch(obj)
            {
                case Component c:
                    ExecuteWithComponent(c);
                    return;
                
                case ScriptableObject so:
                    ExecuteWithScriptableObject(so);
                    return;
                
                default: throw new Exception("Not supported");
            }
            
            
        }
    }
    
}
