using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using XLua;
using System.Collections.Concurrent;
using System.IO;

using Prota.Lua;

namespace Prota.Editor
{
    [CustomEditor(typeof(LuaScript), false)]
    public class LuaScriptInspector : UnityEditor.Editor
    {
        LuaElementInspector luaInspector;
        
        TextField pathText;
        
        Label tag;
        
        Label loadedTag;
        
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
            
            root.AddChild(loadedTag = new Label() { });
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            root.AddChild(luaInspector = new LuaElementInspector(0, "self", () => {
                var table = script.instance;
                return table;
            }));
            
            root.AddChild(new VisualElement().AsVerticalSeperator(2));
            
            root.AddChild(new Button(() => {
                    script.Load();
                }) { text = "重新加载" }
            );
            
            root.AddChild(new VisualElement().AsVerticalSeperator(2));
            
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
            if(LuaCore.instance == null || !LuaCore.instance.envLoaded)
            {
                luaInspector?.SetVisible(false);
                return;
            }

            luaInspector.SetVisible(true);
            luaInspector?.Update();
        }
        
        void RefreshAll()
        {
            RefreshText();
            RefreshLoaded();
            RefreshTag();
            RefreshFilter();
        }
        
        void RefreshLoaded()
        {
            if(!string.IsNullOrEmpty(script.loaded?.path))
            {
                loadedTag.SetVisible(true);
                tag.SetVisible(false);
                loadedTag.text = "已加载: " + script.loaded.path;
            }
            else
            {
                tag.SetVisible(true);
                loadedTag.SetVisible(false);
            }
            
        }
        
        void RefreshText()
        {
            pathText.value = script.luaPath;
        }
        
        void RefreshTag()
        {
            if(!LuaCore.IsValidPath(script.luaPath))
            {
                tag.SetTextColor(new Color(.8f, .3f, .3f, 1));
                return;
            }
            
            var f = new FileInfo(LuaCore.KeyToSourcePath(script.luaPath));
            
            if(f.Exists)
            {
                tag.SetTextColor(new Color(.8f, .8f, 1f, 1));
            }
            else
            {
                tag.SetTextColor(new Color(.6f, .6f, .2f, 1));
            }
        }
        
        static char[] delimiter = new char[]{ '/' };
        void RefreshFilter()
        {
            if(string.IsNullOrEmpty(script.luaPath))
            {
                fileList.SetVisible(false);
                return;
            }
            
            fileList.SetVisible(true);
            
            var pattern = pathText.value.ToLower();
            
            var i = 0;
            
            if(!LuaCore.IsValidPath(script.luaPath))
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
                    var path = Prota.Editor.Utils.GetRelativePath(LuaCore.luaSourcePath, fn.FullName);
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