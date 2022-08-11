using UnityEngine;
using UnityEditor;
using Prota.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

namespace Prota.Editor
{
    [CustomEditor(typeof(Prota.Procedural.ProceduralTexture), false)]
    [ExecuteAlways]
    public class LuaCoreInspector : UpdateInspector
    {
        
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            var target = this.target as Prota.Procedural.ProceduralTexture;
            var sTarget = this.serializedObject;
            root.AddChild(new PropertyField(sTarget.FindProperty("type")))
                .AddChild(new VisualElement().AsHorizontalSeperator(2))
                .AddChild(new PropertyField(sTarget.FindProperty("recordType")))
                .AddChild(new PropertyField(sTarget.FindProperty("texture")))
                .AddChild(new PropertyField(sTarget.FindProperty("sprite")))
                .AddChild(
                    new Button(() => {
                        var texture = sTarget.FindProperty("texture").objectReferenceValue as Texture2D;
                        if(texture == null) return;
                        var path = EditorUtility.SaveFilePanel("Save to", "./", "name.png", ".png");
                        File.WriteAllBytes(path, texture.EncodeToPNG());
                    }) { text = "Save" }
                );
            
            return root;
        }

        protected override void Update()
        {
            
        }
    }
}
