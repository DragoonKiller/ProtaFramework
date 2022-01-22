using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using XLua;

namespace Prota.Lua
{
    [CustomEditor(typeof(LuaScriptList), false)]
    public class LuaScriptListInspector : UnityEditor.Editor
    {
        
        public override VisualElement CreateInspectorGUI()
        {
            var v = new VisualElement();
            
            v.AddChild(new Button() { text = "全部刷新" }
                .OnClick(e => {
                    var target = serializedObject.targetObject as LuaScriptList;
                    if(target == null) throw new Exception();
                    target.RefreshAll();
                })
            );
            
            v.AddChild(new PropertyField(serializedObject.FindProperty("entries")));
            
            return v;
        }
        
    }
}