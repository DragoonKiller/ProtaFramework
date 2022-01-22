using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using XLua;

namespace Prota.Lua
{
    [CustomEditor(typeof(LuaScript), false)]
    public class LuaScriptInspector : UnityEditor.Editor
    {
        LuaElementInspector luaInspector;
        
        ObjectField scriptField;
        
        TextField pathText;
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            root.AddChild(scriptField = new ObjectField("文件") {
                    objectType = typeof(TextAsset),
                    value = serializedObject.FindProperty("script").objectReferenceValue
                }
                .OnValueChange<ObjectField, UnityEngine.Object>(e => {
                    serializedObject.FindProperty("script").objectReferenceValue = e.newValue;
                    RefreshPathLabel();
                    EditorUtility.SetDirty(serializedObject.targetObject);
                })
            );
            
            root.AddChild(pathText = new TextField("路径").SetNoInteraction());
            
            root.AddChild(luaInspector = new LuaElementInspector(0, "self", () => {
                var obj = serializedObject.targetObject as LuaScript;
                if(obj == null) return null;
                var table = obj.instance;
                return table;
            }));
            
            RefreshPathLabel();
            
            return root;
        }
        
        void OnEnable()
        {
            EditorApplication.update += Update;
        }
        
        void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        
        void Update()
        {
            luaInspector?.Update();
        }
        
        void RefreshPathLabel()
        {
            var v = scriptField.value as TextAsset;
            if(v == null)
            {
                pathText.value = "没有指定文件.";
            }
            else
            {
                var path = AssetDatabase.GetAssetPath(v);
                pathText.value = path;
            }
        }
        
    }
}