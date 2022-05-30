using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor.UIElements;

namespace Prota.Editor
{
    public class ProtaToolWindow : EditorWindow
    {
        [MenuItem("Window/Tool Window _F3")]
        public static EditorWindow Open()
        {
            var w = EditorWindow.GetWindow<ProtaToolWindow>();
            w.titleContent = new GUIContent("Tool window");
            return w;
        }
        
        VisualElement root => this.rootVisualElement;
        
        void OnEnable()
        {
            root.Clear();
            CreateRoot();
        }
        
        void OnDisable()
        {
            root.Clear();
        }
        
        
        // ========================================================================================
        // 数据
        // ========================================================================================
        
        class Category
        {
            public List<Command> commands = new List<Command>();
        }
        
        class Command
        {
            public MethodInfo func;
            public bool dynamic => !func.IsStatic;
            public List<ParamInfo> paramList = new List<ParamInfo>();
        }
        
        class ParamInfo
        {
            public bool isList => subList != null;
            public List<ParamInfo> subList;
            
            public bool isDict => subDict != null;
            public Dictionary<ParamInfo, ParamInfo> subDict;
            
            public Type type { get => typeKey; set => typeKey = value; }
            
            public Type typeKey;
            
            public Type typeValue;
            
            public object value;
            
            public string name;
            
            public static ParamInfo List(string name, Type type)
            {
                return new ParamInfo() { subList = new List<ParamInfo>(), type = type, name = name };
            }
            
            public static ParamInfo Object(string name, Type type)
            {
                return new ParamInfo() { type = type, name = name };
            }
            
            public static ParamInfo Dict(string name, Type typeKey, Type typeValue)
            {
                return new ParamInfo() { subDict = new Dictionary<ParamInfo, ParamInfo>(), type = typeKey, typeValue = typeValue, name = name };
            }
            
            public static ParamInfo Class(string name, Type type)
            {
                var p = new ParamInfo();
                return p;
            }
        }
        
        
        List<Category> cats;
        
        void SyncData()
        {
            if(cats != null) return;
            
            Dictionary<string, Category> name2Cats = new Dictionary<string, Category>();
            name2Cats.Add("defualt", new Category());
            
            foreach(var f in TypeCache.GetMethodsWithAttribute<ProtaToolAttribute>())
            {
                var c = f.GetCustomAttribute<ProtaToolCategoryAttribute>();
                if(c != null && !name2Cats.TryGetValue(c.catName, out var cat)) name2Cats.Add(c.catName, cat);
                cat = name2Cats[c?.catName ?? "default"];
                
                Command command = new Command() { func = f };
                cat.commands.Add(command);
                
                if(command.dynamic) command.paramList.Add(ParamInfo.Object("this", f.DeclaringType));
                
                foreach(var p in f.GetParameters())
                {
                    var pt = p.ParameterType;
                    bool isList = false;
                    bool isDict = false;
                    if(pt.IsConstructedGenericType)
                    {
                        var ptf = pt.GetGenericTypeDefinition();
                        if(ptf == typeof(List<>)) isList = true;
                        if(ptf == typeof(Dictionary<,>)) isDict = true;
                    }
                    
                    ParamInfo r;
                    if(isList)
                    {
                        r = ParamInfo.List(p.Name, pt.GetGenericArguments()[1]);
                    }
                    else if(isDict)
                    {
                        var pl = pt.GetGenericArguments();
                        r = ParamInfo.Dict(p.Name, pl[1], pl[2]);
                    }
                    else if(pt == typeof(byte)
                        || pt == typeof(sbyte)
                        || pt == typeof(short)
                        || pt == typeof(ushort)
                        || pt == typeof(uint)
                        || pt == typeof(long)
                        || pt == typeof(ulong)
                        || pt == typeof(float)
                        || pt == typeof(double)
                        || pt == typeof(string)
                        || typeof(UnityEngine.Object).IsAssignableFrom(pt))
                    {
                        r = ParamInfo.Object(p.Name, pt);
                    }
                    else
                    {
                        Debug.LogError($"Unaccepted function { f.Name } param { p.Name } type { pt }");
                    }
                    
                }
                
                
            }
        }
        
        // ========================================================================================
        // 视图
        // ========================================================================================
        
        
        ScrollView catList;
        ScrollView commandList;
        VisualElement paramList;
        Button processButton;
        
        void CreateRoot()
        {
            
            IntegerField g = null;
            byte o = 1;
            root.AddChild(g = new IntegerField() { value = (int)o }.OnValueChange<IntegerField, int>(x => {
                if((byte)x.newValue != x.newValue) { g.value = x.previousValue; return; }
                o = (byte)x.newValue;
                g.value = x.newValue;
            }));
            return;
            
            root.SetHorizontalLayout()
                .AddChild(catList = new ScrollView(ScrollViewMode.Vertical)
                    .SetMinWidth(120)
                    .SetGrow()
                )
                .AddChild(commandList = new ScrollView(ScrollViewMode.Vertical)
                    .SetMinWidth(120)
                    .SetGrow()
                )
                .AddChild(new ScrollView(ScrollViewMode.Vertical)
                    .SetMinWidth(240)
                    .SetGrow()
                    .AddChild(paramList = new VisualElement())
                    .AddChild(processButton = new Button())
                );
        }
        
        
        
        
        
        
    }
    
    
}