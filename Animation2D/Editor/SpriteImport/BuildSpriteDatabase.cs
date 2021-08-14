using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Prota.Unity;
using UnityEngine.Timeline;

namespace Prota.Animation
{
    
    // 要求格式: 文件名的最后由 "_xxxx" 结尾, xxxx 是数字.
    public static partial class Animation2DEditor
    {
        public static ProtaSpriteDatabase database => Resources.Load<ProtaSpriteDatabase>(ProtaSpriteDatabase.resPath);
        
        
        [MenuItem("Assets/ProtaFramework/Sprite/新建Sprite数据库", priority = 1200)] 
        static void BuildSpriteDatabase()
        {
            var db = ScriptableObject.CreateInstance<ProtaSpriteDatabase>();
            AssetDatabase.CreateAsset(db, Path.Combine(AssetDatabase.GetAssetPath(Selection.activeInstanceID), "SpriteDatabase.asset"));
            UpdateSpriteDatabase();
        }
        
        
        // [MenuItem("Assets/ProtaFramework/动画/刷新Sprite数据库", priority = 1101)] 
        public static void UpdateSpriteDatabase()
        {
            var collections = AssetDatabase.FindAssets("t:ProtaSpriteCollection");
            var db = database;
            db.Clear();
            foreach(var c in collections)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(c);
                Debug.Log("找到Sprite动画: " + assetPath);
                var cc = AssetDatabase.LoadAssetAtPath<ProtaSpriteCollection>(assetPath);
                db.Add(cc.name, cc);
            }
            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
        }
        
        static void CollectAllSpriteCollections()
        {
            
        }
        
        
        static void AddCollection(ProtaSpriteCollection collection)
        {
            
        }
        
        
    }
}