using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Text;
using System.IO;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        [MenuItem("Assets/ProtaFramework/Tools/创建 Prota Sprite", priority = 600)]
        public static void CreateProtaSprites()
        {
            Sprite[] ToSprites(Texture2D texture)
            {
                var all = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(texture));
                return all.OfType<Sprite>().ToArray();
            }
            
            var selections = Selection.objects;
            using var _ = TempList.Get<Sprite>(out var sprites);
            sprites.AddRange(selections.OfType<Sprite>());
            sprites.AddRange(selections.OfType<Texture2D>().SelectMany(ToSprites));
            
            foreach(var sprite in sprites)
            {
                var g = new GameObject(sprite.name);
                var sr = g.AddComponent<ProtaSpriteRenderer>();
                sr.sprite = sprite;
                sr.rectTransform.sizeDelta = Vector2.one;
            }
        }
        
        
        
    }
    
}
