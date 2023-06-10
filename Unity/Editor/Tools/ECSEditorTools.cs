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
        static Dictionary<Type, Type> ecsComponentMap = null;
        
        [MenuItem("GameObject/Prota Framework/ECS/Create Components for builtins")]
        public static void CreateComponentsForBuiltins()
        {
            if(ecsComponentMap == null)
            {
                ecsComponentMap = new Dictionary<Type, Type>();
                ecsComponentMap.Add(typeof(Rigidbody2D), typeof(ECRigid2D));
                // ecsComponentMap.Add(typeof(Rigidbody), typeof(ECRigid2D));
                
                ecsComponentMap.Add(typeof(SpriteRenderer), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(Image), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(Text), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(TrailRenderer), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(LineRenderer), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(RawImage), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(CanvasGroup), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(MeshRenderer), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(SkinnedMeshRenderer), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(TextMeshPro), typeof(ECRenderer));
                ecsComponentMap.Add(typeof(TextMeshProUGUI), typeof(ECRenderer));
            }
            
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
