using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Prota
{
    public class StateMachineEditorWindow : EditorWindow
    {
        [MenuItem ("ProtaFramework/StateMachine/Editor %3")]
        public static void  ShowWindow () {
            var w = EditorWindow.GetWindow<StateMachineEditorWindow>();
            
            var assetPath = AssetDatabase.FindAssets("t:UnityEngine.UIElements.VisualTreeAsset")
                .Select(x => AssetDatabase.GUIDToAssetPath(x))
                .Where(x => x.EndsWith("ProtaFramework/StateMachine/Editor/StateMachineEditor.uxml"))
                .First();
            VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
            VisualElement ui = uiAsset.CloneTree();
            
            w.rootVisualElement.Add(ui);
        }
        
        VisualElement root => this.rootVisualElement;
        
        void OnGUI () { }
    }
}
