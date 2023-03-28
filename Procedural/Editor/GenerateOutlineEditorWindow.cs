using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota;
using Prota.Unity;
using System.Threading.Tasks;
using Unity.Collections;
using System.IO;

using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Prota.Editor
{
    public class GenerateOutlineTexture : EditorWindow
    {
        
        [MenuItem("ProtaFramework/Tools/Generate Outline Texture Window", priority = 2)]
        public static void ShowWindow()
        {
            var window = GetWindow<GenerateOutlineTexture>();
            window.titleContent = new GUIContent("Generate Outline Texture");
            window.Show();
        }
        
        List<Texture2D> textures = new List<Texture2D>();
            
        void OnEnable()
        {
            rootVisualElement.AddChild(CreateGUI());
            EditorApplication.update += Update;
        }
        
        protected virtual void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        
        void Update()
        {
            textures = GenerateTextureArgs.GetCurrentSelectedTextures();
            rootVisualElement.Q<ListView>("list").itemsSource = textures;
        }
        
        VisualElement CreateGUI()
        {
            var root = new VisualElement();
            
            root.AddChild(new VisualElement()
                .AddChild(new FloatField("描边宽度") { name = "outlineThickness" })
                .AddChild(new FloatField("描边半透明层宽度") { name = "trThickness" })
                .AddChild(new FloatField("定义边界的透明度") { name = "alpha" })
                .AddChild(new ColorField("描边颜色") { name = "outlineColor" })
                .AddChild(new Button(() => {
                    textures.GenerateOutlineTexture();
                    }) { text = "生成!" }
                )
            ).AddChild(new ListView(textures, 20, () => {
                var element = new ObjectField();
                element.objectType = typeof(Texture2D);
                return element;
            }, (a, i) => {
                (a as ObjectField).value = textures[i];
            }) { name = "list" });
            
            var outlineThickness = root.Q<FloatField>("outlineThickness");
            outlineThickness.value = GenerateTextureArgs.outlineWidth;
            outlineThickness.OnValueChange((ChangeEvent<float> e) => {
                GenerateTextureArgs.outlineWidth = e.newValue;
            });
            
            var transparentThickness = root.Q<FloatField>("trThickness");
            transparentThickness.value = GenerateTextureArgs.outlineAlphaWidth;
            transparentThickness.OnValueChange((ChangeEvent<float> e) => {
                GenerateTextureArgs.outlineAlphaWidth = e.newValue;
            });
            
            var alpha = root.Q<FloatField>("alpha");
            alpha.value = GenerateTextureArgs.alphaThreshold;
            alpha.OnValueChange((ChangeEvent<float> e) => {
                GenerateTextureArgs.alphaThreshold = e.newValue;
            });
            
            var outlineColor = root.Q<ColorField>("outlineColor");
            outlineColor.value = GenerateTextureArgs.outlineColor;
            outlineColor.OnValueChange((ChangeEvent<Color> e) => {
                GenerateTextureArgs.outlineColor = e.newValue;
            });
            
            return root;
        }
        
    }
}
