using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using XLua;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Prota.Lua
{
    using Entry = LuaDataBinding.Entry;
    
    
    [CustomEditor(typeof(LuaDataBinding), false)]
    public class LuaDataBindingInspector : UnityEditor.Editor
    {
        LuaDataBinding dataBinding => base.target as LuaDataBinding;
        
        public override VisualElement CreateInspectorGUI()
        {
            var res = new VisualElement();
            
            res.AddChild(new Button(this.RefreshSelf) { text = "刷新所有绑定" });
            
            res.AddChild(new Button(this.RefreshAll) { text = "刷新所有绑定及其子集" });
            
            var targetsProp = serializedObject.FindProperty("targets");
            res.AddChild(new PropertyField(targetsProp));
            
            return res;
        }
        
        void RefreshSelf()
        {
            Refresh(dataBinding.gameObject, dataBinding.targets);
        }
        
        void RefreshAll()
        {
            foreach(var c in dataBinding.gameObject.GetComponentsInChildren<LuaDataBinding>())
            {
                Refresh(c.gameObject, c.targets);
            }
        }
        
        void Refresh(GameObject g, List<Entry> results, bool isSelf = true)
        {
            if(!isSelf && g.GetComponent<LuaDataBinding>()) return; // 由下一级脚本刷新.
            
            if(isSelf)
            {
                Undo.RecordObject(g, "Generate Lua Data Binding");
                results.Clear();
                results.Add(new Entry() { name = "gameObject", target = g });
                results.Add(new Entry() { name = "transform", target = g.transform });
            }
            
            if(TryMatch(g, out var entry))
            {
                results.Add(entry);
            }
            
            for(int i = 0; i < g.transform.childCount; i++)
            {
                Refresh(g.transform.GetChild(i).gameObject, results, false);
            }
        }
        
        char[] delimiter = new char[]{ '.' };
        bool TryMatch(GameObject g, out Entry entry)
        {
            entry = new Entry();
            
            var r = g.name.Split(delimiter);
            if(r.Length != 2)
            {
                if(r.Length > 2) Debug.LogError("GameObject 命名有问题: " + g.name);
                return false;
            }
            
            var name = r[0];
            var type = r[1];
            
            var content = null as UnityEngine.Object;
            if(type == "Obj")
            {
                content = g;
            }
            else
            {
                content = g.GetComponent(type);
                if(content == null)
                {
                    Debug.LogError("在 GameObject " + g.name + " 中找不到组件: " + type);
                    return false;
                }
            }
            
            entry.name = name;
            entry.target = content;
            
            return true;
        }
    }
}