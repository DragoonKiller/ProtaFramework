using UnityEngine;
using UnityEditor;
using UnityEngine.Animations;
using System.Linq;

using Prota;
using Prota.Unity;
using UnityEngine.U2D.Animation;

namespace Prota.Animation
{
    public class AnimationToolWindow : EditorWindow
    {
        [MenuItem("ProtaFramework/Animation/Animation Tools")]
        public static void Open()
        {
            var w = GetWindow<AnimationToolWindow>();
            w.titleContent = new GUIContent("Animation Tools");
        }
        
        Texture selectedTexture;
        
        void OnGUI()
        {
            var selected = Selection.activeObject as Texture;
            if(selected != null)
            {
                var path = AssetDatabase.GetAssetPath(selected);
                if(path != AnimationTools2D.selectedTexturePath) AnimationTools2D.selectedTexturePath = path;
            }
            
            var selectedTexture = AssetDatabase.LoadAssetAtPath<Texture>(AnimationTools2D.selectedTexturePath);
            EditorGUILayout.ObjectField("Selected Texture", selectedTexture, typeof(Texture), false);
            
            var selectedGameObject = Selection.activeGameObject;
            if(selectedGameObject != null)
            {
                var id = selectedGameObject.GetInstanceID().ToString();
                if(id != AnimationTools2D.selectedGameObjectId) AnimationTools2D.selectedGameObjectId = id;
            }
            
            var targetRoot = EditorUtility.InstanceIDToObject(int.Parse(AnimationTools2D.selectedGameObjectId)) as GameObject;
            EditorGUILayout.ObjectField("Selected Root", targetRoot, typeof(GameObject), false);
            
            AnimationTools2D.prefixRemoval = EditorGUILayout.IntField("Prefix Removal", AnimationTools2D.prefixRemoval);
            AnimationTools2D.useUpperCase = EditorGUILayout.Toggle("Use Upper Case", AnimationTools2D.useUpperCase);
            
            if(GUILayout.Button("Build Sprite Objects")
                && targetRoot != null
                && selectedTexture != null
            )
            {
                Undo.RecordObject(targetRoot, "Build Sprite Objects");
                AnimationTools2D.BuildSpriteObjects();
            }
        }
        
        void Refresh()
        {
            Repaint();
        }
        
        void OnEnable()
        {
            EditorApplication.update += Refresh;
        }
        
        void OnDisable()
        {
            EditorApplication.update -= Refresh;
        }
        
    }
    
    public static class AnimationTools2D
    {
        public static string selectedTexturePath
        {
            get => EditorPrefs.GetString("prota::AnimationTools2D.selectedTexture", null);
            set => EditorPrefs.SetString("prota::AnimationTools2D.selectedTexture", value);
        }
        
        public static string selectedGameObjectId
        {
            get => EditorPrefs.GetString("prota::AnimationTools2D.selectedGameObject", null);
            set => EditorPrefs.SetString("prota::AnimationTools2D.selectedGameObject", value);
        }
        
        public static int prefixRemoval
        {
            get => EditorPrefs.GetInt("prota::AnimationTools2D.prefixRemoval", 0);
            set => EditorPrefs.SetInt("prota::AnimationTools2D.prefixRemoval", value);
        }
        
        public static bool useUpperCase
        {
            get => EditorPrefs.GetBool("prota::AnimationTools2D.useUpperCase", false);
            set => EditorPrefs.SetBool("prota::AnimationTools2D.useUpperCase", value);
        }
        
        
        // [MenuItem("GameObject/Prota Framework/Animation/Build Sprite Objects")]
        public static void BuildSpriteObjects()
        {
            var root = EditorUtility.InstanceIDToObject(int.Parse(selectedGameObjectId)) as GameObject;
            if(root == null) return;
            
            // get all sprites in target.
            var resources = AssetDatabase.LoadAllAssetsAtPath(selectedTexturePath);
            var sprites = resources.Select(r => r as Sprite).Where(x => x != null).ToList();
            if(sprites == null || sprites.Count == 0) return;
            
            var tr = root.transform.AsList<Transform>();
            
            foreach(var i in sprites)
            {
                var name = i.name;
                var nameComp = name.Split("_", System.StringSplitOptions.None);
                if(prefixRemoval > 0)
                {
                    nameComp = nameComp.Skip(prefixRemoval).ToArray();
                }
                
                if(useUpperCase)
                {
                    for(int j = 0; j < nameComp.Length; j++)
                    {
                        nameComp[j] = nameComp[j].Substring(0, 1).ToUpper() + nameComp[j].Substring(1);
                    }
                }
                
                name = string.Join("_", nameComp);
                var go = new GameObject(name);
                go.transform.SetParent(root.transform, false);
                go.transform.SetAsLastSibling();
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = i;
                var sk = go.AddComponent<SpriteSkin>();
            }
        }
        
    }
}
