using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using UnityEngine.UI;
using Prota;
using System.Collections.Generic;

namespace Prota.Editor
{
    using VerticalScope = EditorGUILayout.VerticalScope;
    using HorizontalScope = EditorGUILayout.HorizontalScope;
    
    public class SpriteGroupControl : EditorWindow
    {
        static GUIStyle _header;
        static GUIStyle header
        {
            get
            {
                if(_header == null)
                {
                    _header = new GUIStyle(EditorStyles.boldLabel);
                    _header.alignment = TextAnchor.MiddleCenter;
                }
                return _header;
            }
        }
        
        [MenuItem("ProtaFramework/Editor/Sprite Group Control", priority = 600)]
        static void OpenWindow()
        {
            var window = GetWindow<SpriteGroupControl>();
            window.titleContent = new GUIContent("Sprite Group Control");
            window.Show();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        GameObject[] targets = new GameObject[0];
        
        bool locked;
        
        Vector2 scrollPos;
        
        bool updatesEachFrame;
        
        void Update()
        {
            bool shouldRepaint = UpdateSelectSprites();
            shouldRepaint |= !locked && Selection.gameObjects != null && !Selection.gameObjects.SequenceEqual(targets);
            
            if(shouldRepaint) Repaint();
        }
        
        void OnGUI()
        {
            SaveLoad();
            UpdateSpriteRenderers();
            SetSpriteRenderersUI();
            SetSpritesUI();
            SetColorsUI();
            SetRandomSeed();
            DrawExecuteUI();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        [Serializable]
        struct SaveLoadData
        {
            public int randomSeed;
            public List<Color> colors;
            public List<string> targets;
            public List<string> lockedSprites;
            public float hueOffset;
            public float saturationOffset;
            public float brightnessOffset;
            public float contrastOffset;
        }
        
        void SaveLoad()
        {
            using(new HorizontalScope())
            {
                if(GUILayout.Button("Save")) Save();
                if(GUILayout.Button("Load")) Load();
            }
        }
        
        void Save()
        {
            var data = new SaveLoadData
            {
                randomSeed = randomSeed,
                colors = selectColors.ToList(),
                targets = targets.Select(x => x.FormatGameObjectPath()).ToList(),
                lockedSprites = lockedSprites.Select(x => x.FormatAssetPath()).ToList(),
                hueOffset = randomHue,
                saturationOffset = randomSaturation,
                brightnessOffset = randomBrightness,
                contrastOffset = randomContrast,
            };
            var json = JsonUtility.ToJson(data, true);
            
            var file = EditorUtility.SaveFilePanel("Save", "", "sprite_group_control.json", "json");
            if(string.IsNullOrEmpty(file)) return;
            
            System.IO.File.WriteAllText(file, json);
            AssetDatabase.Refresh();
        }
        
        void Load()
        {
            var file = EditorUtility.OpenFilePanel("Load", "", "json");
            if(string.IsNullOrEmpty(file)) return;
            
            selectSprites = new Sprite[0];
            selectColors.Clear();
            lockedSprites.Clear();
            
            var json = System.IO.File.ReadAllText(file);
            var data = JsonUtility.FromJson<SaveLoadData>(json);
            
            locked = true;
            randomSeed = data.randomSeed;
            targets = data.targets.Select(x => x.GetGameObjectFromPath()).Where(x => x).ToArray();
            selectColors.AddRange(data.colors);
            lockedSprites.AddRange(data.lockedSprites.Select(x => x.GetSpriteFromPath()).Where(x => x));
            randomHue = data.hueOffset;
            randomSaturation = data.saturationOffset;
            randomBrightness = data.brightnessOffset;
            randomContrast = data.contrastOffset;
            
            (!lockedSprites.Any(x => x == null)).Assert();
            (!targets.Any(x => x == null)).Assert();
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        bool regenerateRandomSeed;
        
        int randomSeed;
        
        System.Random random;
        
        void InitRandom()
        {
            if(regenerateRandomSeed) randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
            random = new System.Random(randomSeed);
        }
        
        float NextRandom(float from, float to) => (float)random.NextDouble() * (to - from) + from;
        
        float NextRandom01() => (float)random.NextDouble();
        
        void SetRandomSeed()
        {
            regenerateRandomSeed = EditorGUILayout.Toggle("Regenerate Random Seed", regenerateRandomSeed);
            randomSeed = EditorGUILayout.IntField("Random Seed", randomSeed);
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        Vector2 colorUIScroll;
        [ColorUsage(true, true)] List<Color> selectColors = new();
        float randomHue;
        float randomSaturation;
        float randomBrightness;
        float randomContrast;
        
        void SetColorsUI()
        {
            GUILayout.Label("==== Color ====", header);
            
            randomHue = EditorGUILayout.Slider("Hue", randomHue, 0f, 1f);
            randomSaturation = EditorGUILayout.Slider("Saturation", randomSaturation, 0f, 1f);
            randomBrightness = EditorGUILayout.Slider("Brightness", randomBrightness, 0f, 1f);
            randomContrast = EditorGUILayout.Slider("Contrast", randomContrast, 0f, 1f);
               
            int? removeIndex = null;
            var n = EditorGUILayout.IntField("Color Count", selectColors.Count);
            if(n != selectColors.Count) selectColors.Resize(n);
            
            using(var _s = new EditorGUILayout.ScrollViewScope(colorUIScroll, GUILayout.MaxHeight(300)))
            {
                colorUIScroll = _s.scrollPosition;
                
                for(int i = 0; i < selectColors.Count; i++)
                {
                    using var _ = new HorizontalScope();
                    if(GUILayout.Button("X", GUILayout.Width(20))) removeIndex = i;
                    selectColors[i] = EditorGUILayout.ColorField(GUIContent.none, selectColors[i], true, true, true);
                }
            }
            
            if(removeIndex.HasValue) selectColors.RemoveAt(removeIndex.Value);
        }
        
        void ExecuteSetColors()
        {
            if(selectColors.Count == 0) return;
            
            foreach(var g in targets)
            {
                var colorIndex = random.Next(selectColors.Count);
                var color = selectColors[colorIndex];
                
                var hsl = color.ToHSL();
                hsl.h += randomHue * NextRandom01().XMap(0, 1, -1, 1);
                hsl.s += randomSaturation * NextRandom01().XMap(0, 1, -1, 1);
                hsl.l += randomBrightness * NextRandom01().XMap(0, 1, -1, 1);
                color = hsl.ToColor(color.a);
                
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
            GUILayout.Label("==== Sprite ====", header);
            
            int? removeIndex = null;
            var n = EditorGUILayout.IntField("Sprite Count", lockedSprites.Count);
            if(n != lockedSprites.Count) lockedSprites.Resize(n);
            
            if(GUILayout.Button("Lock all sprites")) lockedSprites.AddRange(selectSprites);
            
            using(var _s = new EditorGUILayout.ScrollViewScope(spriteUIScroll, GUILayout.MaxHeight(300)))
            {
                spriteUIScroll = _s.scrollPosition;
                
                Sprite DrawEntry(Sprite sprite, out bool remove)
                {
                    remove = false;
                    
                    using var _ = new HorizontalScope();
                    
                    bool isLocked = lockedSprites.Contains(sprite);
                    
                    using(new EditorGUI.DisabledScope(isLocked))
                    {
                        if(GUILayout.Button("√", GUILayout.Width(20))) lockedSprites.Add(sprite);
                    }
                    
                    using(new EditorGUI.DisabledScope(!isLocked))
                    {
                        if(GUILayout.Button("X", GUILayout.Width(20))) remove = true;
                    }
                    
                    Sprite res;
                    using(new BackgroundColorScope(isLocked ? new Color(0.5f, 0.6f, 0.8f, 1f) : GUI.color))
                    {
                        var hint = GUILayout.Height(EditorGUIUtility.singleLineHeight);
                        res = (Sprite)EditorGUILayout.ObjectField("", sprite, typeof(Sprite), false, hint);
                    }
                    
                    return res;
                }
                
                for(int i = 0; i < lockedSprites.Count; i++)
                {
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
            if(lockedSprites.Count == 0) return;
            
            foreach(var g in targets)
            {
                var spriteIndex = random.Next(lockedSprites.Count);
                var sprite = lockedSprites[spriteIndex];
                
                var processor = GetProcessor(g);
                Undo.RecordObject(g, "Set Sprite");
                processor.SetSprite(g, sprite);
                // Debug.LogError($"Set sprite {sprite.name} to {g.name}");
            }
        }

        // ====================================================================================================
        // ====================================================================================================
        
        bool updateExecution;
        
        void DrawExecuteUI()
        {
            updateExecution = EditorGUILayout.Toggle("Update Execution", updateExecution);
            if(GUILayout.Button("Execute") || updateExecution) ExecuteAll();
        }
        
        void ExecuteAll()
        {
            InitRandom();
            ExecuteSetColors();
            ExecuteSetSprites();
            SceneView.lastActiveSceneView.Repaint();
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
        
        void SetSpriteRenderersUI()
        {
            locked = EditorGUILayout.Toggle("Lock", locked);
            
            using(new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.MinHeight(200)))
            {
                foreach(var g in targets)
                {
                    var backgroundColor = GUI.backgroundColor;
                    var cotenntColor = GetProcessor(g).editorColor;
                    
                    using(new HorizontalScope())
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
            }
        }
    
        bool ValidSprite(GameObject g)
        {
            return processors.Any(p => p.UseProcessor(g));
        }
        
        ISpriteProcessor GetProcessor(GameObject g)
        {
            return processors.First(p => p.UseProcessor(g));
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
                var rd = g.GetComponent<SpriteRenderer>();
                Undo.RecordObject(rd, "Set Color");
                rd.color = color;
            }

            public void SetSprite(GameObject g, Sprite sprite)
            {
                var rd = g.GetComponent<SpriteRenderer>();
                Undo.RecordObject(rd, "Set Sprite");
                rd.sprite = sprite;
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
                var rd = g.GetComponent<ProtaSpriteRenderer>();
                Undo.RecordObject(rd, "Set Color");
                rd.vertexColor = color;
            }

            public void SetSprite(GameObject g, Sprite sprite)
            {
                var rd = g.GetComponent<ProtaSpriteRenderer>();
                Undo.RecordObject(rd, "Set Sprite");
                rd.sprite = sprite;
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
                var rd = g.GetComponent<Image>();
                Undo.RecordObject(rd, "Set Color");
                rd.color = color;
            }

            public void SetSprite(GameObject g, Sprite sprite)
            {
                var rd = g.GetComponent<Image>();
                Undo.RecordObject(rd, "Set Sprite");
                rd.sprite = sprite;
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
            processors = new ISpriteProcessor[] {
                spriteRendererProcessor,
                protaSpriteRendererProcessor,
                imageProcessor
            };
        }
        
        
    }
    
}
