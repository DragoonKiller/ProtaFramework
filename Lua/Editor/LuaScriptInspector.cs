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
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            root.AddChild(new ObjectField("脚本文件") {
                    objectType = typeof(TextAsset),
                    value = serializedObject.FindProperty("script").objectReferenceValue
                }
                .OnValueChange<ObjectField, UnityEngine.Object>(e => {
                    serializedObject.FindProperty("script").objectReferenceValue = e.newValue;
                    EditorUtility.SetDirty(serializedObject.targetObject);
                })
            );
            
            root.AddChild(luaInspector = new LuaElementInspector(0, "self", () => {
                var obj = serializedObject.targetObject as LuaScript;
                if(obj == null) return null;
                var table = obj.instance;
                return table;
            }));
            
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
        
        
    }
}