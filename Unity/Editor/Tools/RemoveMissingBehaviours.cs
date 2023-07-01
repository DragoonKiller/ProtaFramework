using System;
using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.IO;

using Prota.Unity;
using UnityEngine.SceneManagement;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        [MenuItem("ProtaFramework/Tools/Remove Missing Behaviours")]
        public static void RemoveMissingBehaviours()
        {
            var gs = Resources.LoadAll<GameObject>("/");
            foreach(var g in gs) RemoveForGameObject(g);

            for (int i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                foreach(var g in s.GetRootGameObjects())
                    RemoveForGameObject(g);
            }

            static void RemoveForGameObject(GameObject g)
            {
                g.transform.ForeachTransformRecursively(t =>
                {
                    var components = t.GetComponents<Component>();
                    
                    for (int i = 0; i < components.Length; i++)
                    {
                        if (components[i] == null)
                        {
                            Debug.Log($"Removing missing component from {t.name}");
                            int cnt = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
                            if(cnt > 0) Debug.Log($"Removed {cnt} missing components from {t.name}");
                            continue;
                        }
                        
                        if(!(components[i] is MonoBehaviour)) continue;
                        
                        if(SerializationUtility.ClearAllManagedReferencesWithMissingTypes(components[i]))
                        {
                            Debug.Log($"Removed missing references from {t.name}");
                        }
                    }
                    
                });
            }
        }
    }
}
