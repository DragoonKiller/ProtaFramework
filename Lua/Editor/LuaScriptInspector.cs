using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using XLua;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Prota.Lua
{
    [CustomEditor(typeof(LuaScript), false)]
    public class LuaScriptInspector : UnityEditor.Editor
    {
        LuaElementInspector luaInspector;
        
        TextField pathText;
        
        Label tag;
        
        VisualElement fileList;
        
        SerializedProperty luaPathProperty => serializedObject.FindProperty("luaPath");
        
        LuaScript script => (target as LuaScript);
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            
            root.AddChild(new VisualElement().SetHorizontalLayout()
                .AddChild(tag = new Label("Lua:")
                )
                .AddChild(pathText = new TextField() { }
                    .SetGrow()
                    .OnValueChange<TextField, string>(e => {
                        Undo.RecordObject(target, "LuaScriptInspector:");
                        script.luaPath = e.newValue;
                        EditorUtility.SetDirty(target);
                        RefreshFilter();
                    })
                )
            );
            
            root.AddChild(fileList = new VisualElement() { });
            
            root.AddChild(luaInspector = new LuaElementInspector(0, "self", () => {
                var obj = serializedObject.targetObject as LuaScript;
                if(obj == null) return null;
                var table = obj.instance;
                return table;
            }));
            
            
            RefreshAll();
            
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
        
        void RefreshAll()
        {
            RefreshText();
            RefreshFilter();
        }
        
        void RefreshText()
        {
            pathText.value = script.luaPath;
        }
        
        static char[] delimiter = new char[]{ '/' };
        void RefreshFilter()
        {
            var nowStr = Path.Combine(LuaCore.luaSourcePath, pathText.value);
            nowStr = nowStr.Replace("\\", "/");
            FileInfo f = null;
            try
            {
                f = new FileInfo(nowStr + ".lua");
            }
            catch(Exception)
            {
                tag.SetTextColor(new Color(.8f, .3f, .3f, 1));
                return;
            }
            
            var success = false;
            if(f.Exists)
            {
                tag.SetTextColor(new Color(.8f, .8f, 1f, 1));
                success = true;
            }
            else tag.SetTextColor(new Color(.6f, .6f, .2f, 1));
            
            var pattern = pathText.value.ToLower();
            
            var i = 0;
            
            if(!success)
            {
                foreach(var fn in new DirectoryInfo(LuaCore.luaSourcePath).EnumerateFiles("*.lua", SearchOption.AllDirectories))
                {
                    var match = fn.FullName.Replace("\\", "/").ToLower();
                    if(!match.Contains(pattern)) continue;
                    
                    while(fileList.childCount <= i)
                    {
                        var newLabel = new Label() { };
                        fileList.AddChild(newLabel);
                        
                        var color = newLabel.resolvedStyle.backgroundColor;
                        
                        newLabel.RegisterCallback<MouseEnterEvent>(e => {
                            newLabel.SetColor(new Color(.1f, .1f, .1f, 1f));
                        });
                        
                        newLabel.RegisterCallback<MouseLeaveEvent>(e => {
                            newLabel.SetColor(color);
                        });
                        
                        newLabel.RegisterCallback<MouseDownEvent>( e => {
                            Undo.RecordObject(target, "LuaScriptInspector:");
                            script.luaPath = newLabel.text;
                            EditorUtility.SetDirty(target);
                            RefreshAll();
                        });
                    }
                    
                    var label = fileList[i] as Label;
                    label.SetVisible(true);
                    var path = Prota.Unity.Utils.GetRelativePath(LuaCore.luaSourcePath, fn.FullName);
                    path = path.Replace("\\", "/").Replace(".lua", "");
                    label.text = path;
                    label.SetVisible(true);
                    
                    i = i + 1;
                    if(i >= 10) break;
                }
            }
            
            while(fileList.childCount > i) fileList.RemoveAt(fileList.childCount - 1);
        }
        
    }
}