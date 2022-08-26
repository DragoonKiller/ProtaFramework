using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using XLua;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Prota.Lua;

namespace Prota.Editor
{
    [CustomEditor(typeof(LuaCore), false)]
    [ExecuteAlways]
    public class LuaCoreInspector : UpdateInspector
    {
        LuaCore core => target as LuaCore;
        
        VisualElement root;
        VisualElement playingRoot;
        VisualElement editorRoot;
        IntegerField memoryIndicator;
        VisualElement loadedModuleRoot;
        
        Dictionary<string, VisualElement> loadedModuleList = new Dictionary<string, VisualElement>();
        
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            root.AddChild(playingRoot = new VisualElement())
                .AddChild(editorRoot = new VisualElement());
            
            playingRoot.AddChild(
                memoryIndicator = new IntegerField("内存: ") { isReadOnly = true }
            )
            .AddChild(new VisualElement().AsHorizontalSeperator(2))
            .AddChild(new ScrollView()
                .SetGrow()
                .SetVerticalLayout()
                .SetMaxHeight(600)
                .AddChild(loadedModuleRoot = new VisualElement())
            )
            .AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            editorRoot.AddChild(new Label("虚拟机未开启"));
            
            return root;
        }
        
        protected override void Update()
        {
            if(root == null) return;
            
            playingRoot.SetVisible(core.envLoaded);
            editorRoot.SetVisible(!core.envLoaded);
            
            if(core.envLoaded)
            {
                memoryIndicator.value = core.memory;
                
                loadedModuleList.SetSync(() => core.loaded,
                    core.loaded.TryGetValue,
                    (k, v) => {
                        var res = new VisualElement()
                            .SetHorizontalLayout()
                            .AddChild(new Label() { text = " r " }
                                .SetWidth(12)
                                .HoverLeaveColor(new Color(.3f, .3f, .3f, 1f), new Color(.1f, .1f, .1f, .1f))
                                .OnClick(e => core.loaded[k].Reload(true))
                            )
                            .AddChild(new Label(k) { name = "label" });
                            
                        res.name = k;
                        loadedModuleRoot.AddChild(res);
                        return res;
                    },
                    (k, g, t) => {
                        t.SetVisible(true);
                        t.Q<Label>("label").text = k;
                    },
                    (k, g) => {
                        loadedModuleRoot.Remove(loadedModuleRoot.Q(k));
                    }
                );
                
            }
        }
        
    }
}
