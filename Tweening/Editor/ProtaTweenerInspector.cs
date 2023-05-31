using UnityEngine;
using UnityEditor;
using Prota.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Prota.Tween;
using System.Collections.Generic;
using System;
using UnityEditor.Build.Content;
using System.Configuration;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaTweener), false)]
    [ExecuteAlways]
    public class ProtaTweenerInspector : UpdateInspector
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            Func<string, SerializedProperty> prop = serializedObject.FindProperty;
            
            root.AddChild(new VisualElement()
                .SetHorizontalLayout()
                .AddChild(new PropertyField(prop("progress"), "进度 progress")
                    .SetGrow()
                )
            );
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            root.AddChild(new VisualElement().AsToggle("autoPlay", "自动播放 auto play", prop("autoPlay")));
            
            root.AddChild(
                new VisualElement() { name = "autoPlayGroup" }
                    .AddChild(new VisualElement().AsToggle("stopWhenFinished", "播放完毕后停止 stop when finished", prop("stopWhenFinished")))
                    .AddChild(new PropertyField(prop("duration"), "播放时长 duration"))
                    .AddChild(new PropertyField(prop("playReversed"), "反向播放 play reversed"))
                    .AddChild(new VisualElement().AsToggle("loop", "循环 loop", prop("loop")))
                    .AddChild(new VisualElement().AsToggle("reverseOnLoop", "循环后反向 reverse on loop", prop("reverseOnLoop")))
                    .AddChild(new VisualElement().AsToggle("resetWhenEnabled", "激活时重置 reset when enabled", prop("resetWhenEnabled")))
                    .AddChild(new PropertyField(prop("resetPosition"), "重置设置时间") { name = "resetPosition" })
            );
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            root.AddChild(new VisualElement().AsToggle("move", "移动 move", prop("move")));            
            
            root.AddChild(
                new VisualElement() { name = "moveGroup" }
                    .AddChild(new PropertyField(prop("moveFrom"), "起点 from"))
                    .AddChild(new PropertyField(prop("moveTo"), "终点 to"))
                    .AddChild(new PropertyField(prop("moveEase"), "插值 ease"))
            );
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            root.AddChild(new VisualElement().AsToggle("rotate", "旋转 rotate", prop("rotate")));      
            
            root.AddChild(
                new VisualElement() { name = "rotateGroup" }
                    .AddChild(new PropertyField(prop("rotateFrom"), "起点 from"))
                    .AddChild(new PropertyField(prop("rotateTo"), "终点 to"))
                    .AddChild(new PropertyField(prop("rotateEase"), "插值 ease"))
            );
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            root.AddChild(new VisualElement().AsToggle("scale", "缩放 scale", prop("scale")));
            
            root.AddChild(
                new VisualElement() { name = "scaleGroup" }
                    .AddChild(new PropertyField(prop("scaleFrom"), "起点 from"))
                    .AddChild(new PropertyField(prop("scaleTo"), "终点 to"))
                    .AddChild(new PropertyField(prop("scaleEase"), "插值 ease"))
            );
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            root.AddChild(new VisualElement().AsToggle("color", "颜色 color", prop("color")));
            
            root.AddChild(
                new VisualElement() { name = "colorGroup" }
                    .AddChild(new PropertyField(prop("colorTarget"), "目标")
                        .SetNoInteraction()
                    )
                    .AddChild(new PropertyField(prop("colorFrom"), "起点 from"))
                    .AddChild(new PropertyField(prop("colorTo"), "终点 to"))
                    .AddChild(new PropertyField(prop("colorEase"), "插值 ease"))
            );
            
            root.AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            root.AddChild(new VisualElement().AsToggle("recordMode", "录制模式 record mode", prop("recordMode")));
            
            root.Q("autoPlayGroup").ShowOnCondition(
                root.Q<Toggle>("autoPlay"),
                (bool from, bool to) => to == true
            );
            
            root.Q("moveGroup").ShowOnCondition(
                root.Q<Toggle>("move"),
                (bool from, bool to) => to == true
            );
            
            root.Q("rotateGroup").ShowOnCondition(
                root.Q<Toggle>("rotate"),
                (bool from, bool to) => to == true
            );
            
            root.Q("scaleGroup").ShowOnCondition(
                root.Q<Toggle>("scale"),
                (bool from, bool to) => to == true
            );
            
            root.Q("colorGroup").ShowOnCondition(
                root.Q<Toggle>("color"),
                (bool from, bool to) => to == true
            );
            
            root.Q(":reverseOnLoop").ShowOnCondition(
                root.Q<Toggle>("loop"),
                (bool from, bool to) => to == true
            );
            
            root.Q("resetPosition").ShowOnCondition(
                root.Q<Toggle>("resetWhenEnabled"),
                (bool from, bool to) => to == true
            );
            
            return root;
        }
        
        
        protected override void Update()
        {
            
        }
    }
    
    
    

}
