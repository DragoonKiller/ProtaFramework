using System;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Prota.Unity;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Prota.Editor
{
    public static partial class ProtaEditorCommands
    {
        static Dictionary<Type, Type> ecsComponentMap = new Dictionary<Type, Type>();
        
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod]
        static void Init()
        {
            ecsComponentMap.TryAdd(typeof(SpriteRenderer), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(Image), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(Text), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(TrailRenderer), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(LineRenderer), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(RawImage), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(CanvasGroup), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(MeshRenderer), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(SkinnedMeshRenderer), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(TextMeshPro), typeof(ECRenderer));
            ecsComponentMap.TryAdd(typeof(TextMeshProUGUI), typeof(ECRenderer));
            
        }
        
        [MenuItem("GameObject/Prota Framework/ECS/Create Components for builtins")]
        public static void CreateComponentsForBuiltins()
        {
            var gs = Selection.gameObjects;
            foreach(var g in gs)
            {
                foreach(var c in ecsComponentMap)
                {
                    foreach(var cc in g.GetComponentsInChildren(c.Key))
                    {
                        if(cc.TryGetComponent(c.Value, out var _)) continue;
                        if(!cc.GetComponentInParent<ERoot>()) continue;
                        cc.gameObject.AddComponent(c.Value);
                    }
                }
            }
        }
    }
}
