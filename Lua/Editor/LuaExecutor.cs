using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using System.IO;

namespace Prota.Lua
{
    public class LuaExecutor : EditorWindow
    {
        [MenuItem("ProtaFramework/Lua/Executor %L")]
        public static void OpenWindow()
        {
            var window = EditorWindow.GetWindow<LuaExecutor>();
            window.titleContent = new GUIContent("Lua Executor");
            window.Show();
        }
        
        
        
        void OnEnable()
        {
            rootVisualElement.AddChild(new Button() { text = "重置虚拟机" }
                .OnClick(e => {
                    if(Application.isPlaying)
                    {
                        Debug.LogWarning("不能在游戏运行时关闭虚拟机.");
                        return;
                    }
                    LuaCore.instance.Reset();
                })
            );
            
            for(int i = 0; i < 10; i++)
            {
                var fname = string.Format("./{0}.lua", i.ToString());
                
                rootVisualElement
                    .AddChild(new Button() { text = "执行 " + fname }
                        .OnClick(e => {
                            if(!File.Exists(fname)) return;
                            if(LuaCore.instance == null) return;
                            LuaCore.instance.env.DoString(File.ReadAllText(fname));
                        })
                );
            }
            
            rootVisualElement.Add(new Button() { text = "测试" }
                .OnClick(e => {
                    var t = LuaCore.instance.env.NewTable();
                    t.SetInPath("val", 1);
                    t.SetInPath("gg", LuaCore.instance.env.NewTable());
                    Debug.Assert(t.GetInPath<object>("val").GetType() == typeof(long));
                    Debug.Assert(t.GetInPath<object>("gg").GetType() == typeof(XLua.LuaTable));
                })
            );
        }
        
        void OnDisable()
        {
            rootVisualElement.Clear();
        }
    }
}