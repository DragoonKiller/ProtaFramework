using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using UnityEngine.UI;
using Prota;
using System.Collections.Generic;
using Codice.CM.Client.Differences;

namespace Prota.Editor
{
    public class SpriteGroupControl : EditorWindow
    {
        [MenuItem("ProtaFramework/Editor/Sprite Group Control", priority = 600)]
        static void OpenWindow()
        {
            var window = GetWindow<SpriteGroupControl>();
            window.titleContent = new GUIContent("Sprite Group Control");
            window.Show();
        }
        
        interface ISpriteProcessor
        {
            Color editorColor { get; }
            bool UseProcessor(GameObject g);
            void SetColor(GameObject g, Color color);
            void SetSprite(GameObject g, Sprite sprite);
            void DrawSelect(GameObject g);
        }
        
        class SpriteRendererProcessor : ISpriteProcessor
        {
            public Color editorColor => new Color(0.7f, 0.8f, 1f, 1f);
            
            public bool UseProcessor(GameObject g)
            {
                return g.GetComponent<SpriteRenderer>();
            }
            
            public void SetColor(GameObject g, Color color)
            {
                g.GetComponent<SpriteRenderer>().color = color;
            }

            public void SetSprite(GameObject g, Sprite sprite)
            {
                g.GetComponent<SpriteRenderer>().sprite = sprite;
            }

            public void DrawSelect(GameObject g)
            {
                EditorGUILayout.ObjectField("", g.GetComponent<SpriteRenderer>(), typeof(SpriteRenderer), true);
            }
        }
        
        class ProtaSpriteRendererProcessor : ISpriteProcessor
        {
            public Color editorColor => new Color(0.8f, 1f, 0.7f, 1f);
            
            public bool UseProcessor(GameObject g)
            {
                return g.GetComponent<ProtaSpriteRenderer>();
            }
            
            public void SetColor(GameObject g, Color color)
            {
                g.GetComponent<ProtaSpriteRenderer>().color = color;
            }

            public void SetSprite(GameObject g, Sprite sprite)
            {
                g.GetComponent<ProtaSpriteRenderer>().sprite = sprite;
            }

            public void DrawSelect(GameObject g)
            {
                EditorGUILayout.ObjectField("", g.GetComponent<ProtaSpriteRenderer>(), typeof(ProtaSpriteRenderer), true);
            }
        }
        
        class ImageProcessor : ISpriteProcessor
        {
            public Color editorColor => new Color(1f, 0.7f, 0.8f, 1f);
            
            public bool UseProcessor(GameObject g)
            {
                return g.GetComponent<Image>();
            }
            
            public void SetColor(GameObject g, Color color)
            {
                g.GetComponent<Image>().color = color;
            }

            public void SetSprite(GameObject g, Sprite sprite)
            {
                g.GetComponent<Image>().sprite = sprite;
            }

            public void DrawSelect(GameObject g)
            {
                EditorGUILayout.ObjectField("", g.GetComponent<Image>(), typeof(Image), true);
            }
        }
        
        SpriteRendererProcessor spriteRendererProcessor = new();
        ProtaSpriteRendererProcessor protaSpriteRendererProcessor = new();
        ImageProcessor imageProcessor = new();
        
        ISpriteProcessor[] processors;
        
        SpriteGroupControl()
        {
            processors = new ISpriteProcessor[] { spriteRendererProcessor, protaSpriteRendererProcessor, imageProcessor };
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        GameObject[] targets = new GameObject[0];
        
        bool locked;
        
        Vector2 scrollPos;
        
        void Update()
        {
            bool shouldRepaint = UpdateSelectSprites();
            shouldRepaint |= !locked && Selection.gameObjects != null && !Selection.gameObjects.SequenceEqual(targets);
            
            if(shouldRepaint) Repaint();
        }
        
        void OnGUI()
        {
            UpdateSpriteRenderers();
            using var _ = new EditorGUILayout.HorizontalScope();
            
            using(new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.Width(300)))
            {
                locked = EditorGUILayout.Toggle("Lock", locked, GUILayout.ExpandWidth(true));
                
                foreach(var g in targets)
                {
                    var backgroundColor = GUI.backgroundColor;
                    var cotenntColor = GetProcessor(g).editorColor;
                    
                    using(new EditorGUILayout.HorizontalScope())
                    {
                        using(new EditorGUI.DisabledScope(locked))
                        using(new BackgroundColorScope(locked ? new Color(0.4f, 0.4f, 0.7f, 1f) : GUI.color))
                        using(new ContentColorScope(cotenntColor))
                        {
                            GetProcessor(g).DrawSelect(g);
                        }
                        if(GUILayout.Button("X", GUILayout.Width(20))) targets = targets.Where(t => t != g).ToArray();
                    }
                }
                
                SetRandomSeed();
            }
            
            using(new EditorGUILayout.VerticalScope(GUILayout.Width(400))) SetSpritesUI();
            using(new EditorGUILayout.VerticalScope(GUILayout.Width(400))) SetColorsUI();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        int randomSeed;
        
        System.Random random;
        
        void InitRandom() => random = new System.Random(randomSeed);
        
        float NextRandom(float from, float to) => (float)random.NextDouble() * (to - from) + from;
        
        float NextRandom01() => (float)random.NextDouble();
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        Vector2 colorUIScroll;
        List<Color> selectColors = new();
        bool randomColorValue;
        float randomHue;
        float randomSaturation;
        float randomBrightness;
        float randomContrast;
        
        StructStringCache<int> icache = new();
        
        void SetColorsUI()
        {
            int? removeIndex = null;
            var n = EditorGUILayout.IntField("Count", selectColors.Count);
            if(n != selectColors.Count) selectColors.Resize(n);
            
            using(var _s = new EditorGUILayout.ScrollViewScope(colorUIScroll, GUILayout.Height(300)))
            {
                colorUIScroll = _s.scrollPosition;
                
                for(int i = 0; i < selectColors.Count; i++)
                {
                    using var _ = new EditorGUILayout.HorizontalScope();
                    var name = icache[i];
                    selectColors[i] = EditorGUILayout.ColorField(name, selectColors[i]);
                    if(GUILayout.Button("X", GUILayout.Width(20))) removeIndex = i;
                }
            }
            
            if(removeIndex.HasValue) selectColors.RemoveAt(removeIndex.Value);
            
            randomColorValue = EditorGUILayout.Toggle("Random Color Value", randomColorValue);
            if(randomColorValue)
            {
                randomHue = EditorGUILayout.Slider("Hue", randomHue, -1f, 1f);
                randomSaturation = EditorGUILayout.Slider("Saturation", randomSaturation, -1f, 1f);
                randomBrightness = EditorGUILayout.Slider("Brightness", randomBrightness, -1f, 1f);
                randomContrast = EditorGUILayout.Slider("Contrast", randomContrast, -1f, 1f);
            }
            
            using(new EditorGUILayout.ScrollViewScope(Vector2.zero, GUILayout.ExpandHeight(true))) { }
            if(GUILayout.Button("Set Color")) ExecuteSetColors();
        }
        
        void ExecuteSetColors()
        {
            InitRandom();
            
            foreach(var g in targets)
            {
                var colorIndex = random.Next(selectColors.Count);
                var color = selectColors[colorIndex];
                
                if(randomColorValue)
                {
                    var hsl = color.ToHSL();
                    hsl.h += randomHue * NextRandom01();
                    hsl.s += randomSaturation * NextRandom01();
                    hsl.l += randomBrightness * NextRandom01();
                    color = hsl.ToColor(color.a);
                }
                
                var processor = GetProcessor(g);
                Undo.RecordObject(g, "Set Color");
                processor.SetColor(g, color);
            }
        }

        // ====================================================================================================
        // ====================================================================================================
        
        Vector2 spriteUIScroll;
        
        List<Sprite> lockedSprites = new();
        Sprite[] selectSprites = new Sprite[0];
        void SetSpritesUI()
        {
            int? removeIndex = null;
            var n = EditorGUILayout.IntField("Count", lockedSprites.Count);
            if(n != lockedSprites.Count) lockedSprites.Resize(n);
            
            using(var _s = new EditorGUILayout.ScrollViewScope(spriteUIScroll, GUILayout.Height(300)))
            {
                spriteUIScroll = _s.scrollPosition;
                
                Sprite DrawEntry(Sprite sprite, out bool remove)
                {
                    remove = false;
                    
                    using var _ = new EditorGUILayout.HorizontalScope();
                    
                    bool isLocked = lockedSprites.Contains(sprite);
                    
                    Sprite res;
                    using(new BackgroundColorScope(isLocked ? new Color(0.5f, 0.6f, 0.8f, 1f) : GUI.color))
                    {
                        var hint = GUILayout.Height(EditorGUIUtility.singleLineHeight);
                        res = (Sprite)EditorGUILayout.ObjectField("", sprite, typeof(Sprite), false, hint);
                    }
                    
                    using(new EditorGUI.DisabledScope(isLocked))
                    {
                        if(GUILayout.Button("√", GUILayout.Width(20))) lockedSprites.Add(sprite);
                    }
                    
                    using(new EditorGUI.DisabledScope(!isLocked))
                    {
                        if(GUILayout.Button("X", GUILayout.Width(20))) remove = true;
                    }
                    
                    return res;
                }
                
                for(int i = 0; i < lockedSprites.Count; i++)
                {
                    var name = icache[i];
                    lockedSprites[i] = DrawEntry(lockedSprites[i], out var remove);
                    if(remove) lockedSprites.RemoveAt(i--);
                }
                
                foreach(var sprite in selectSprites.Where(x => !lockedSprites.Contains(x)))
                {
                    DrawEntry(sprite, out _);
                }
            }
            
            if(removeIndex.HasValue)
            {
                lockedSprites.RemoveAt(removeIndex.Value);
            }
            
            if(GUILayout.Button("Lock all sprites")) lockedSprites.AddRange(selectSprites);
            
            using(new EditorGUILayout.ScrollViewScope(Vector2.zero, GUILayout.ExpandHeight(true))) { }
            if(GUILayout.Button("Set Sprite")) ExecuteSetSprites();
        }
        
        bool UpdateSelectSprites()
        {
            var selects = Selection.objects;
            var newSelect = selects
                .Where(o => o is Sprite s && !lockedSprites.Contains(s)).Cast<Sprite>()
                .Concat(selects.Where(o => o is Texture2D t).Cast<Texture2D>().SelectMany(GetSpritesFromTexture))
                .Where(x => x != null && !lockedSprites.Contains(x)).Distinct().ToArray();
            if(selectSprites.SequenceEqual(newSelect)) return false;
            selectSprites = newSelect;
            return true;
        }
        
        IEnumerable<Sprite> GetSpritesFromTexture(Texture2D t)
        {
            var path = AssetDatabase.GetAssetPath(t);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if(importer == null) return new Sprite[0];
            return AssetDatabase.LoadAllAssetsAtPath(path).Where(o => o is Sprite).Cast<Sprite>();
        }

        void ExecuteSetSprites()
        {
            InitRandom();
            
            foreach(var g in targets)
            {
                var spriteIndex = random.Next(lockedSprites.Count);
                var sprite = lockedSprites[spriteIndex];
                
                var processor = GetProcessor(g);
                Undo.RecordObject(g, "Set Sprite");
                processor.SetSprite(g, sprite);
                Debug.LogError($"Set sprite {sprite.name} to {g.name}");
            }
        }

        void SetRandomSeed()
        {
            using(new EditorGUILayout.ScrollViewScope(Vector2.zero, GUILayout.ExpandHeight(true))) { }
            randomSeed = EditorGUILayout.IntField("Random seed", randomSeed);
        }


        // ====================================================================================================
        // ====================================================================================================
        
        void UpdateSpriteRenderers()
        {
            if(locked) return;
            
            if(Selection.gameObjects == null)
            {
                targets = new GameObject[0];
                return;
            }
            
            targets = Selection.gameObjects.Where(ValidSprite).ToArray();
        }
    
        bool ValidSprite(GameObject g)
        {
            return processors.Any(p => p.UseProcessor(g));
        }
        
        ISpriteProcessor GetProcessor(GameObject g)
        {
            return processors.First(p => p.UseProcessor(g));
        }
        
        
    }
    
}
