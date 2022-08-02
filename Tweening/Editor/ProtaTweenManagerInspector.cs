using UnityEngine;
using UnityEditor;
using Prota.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
using Prota.Tweening;
using System.Collections.Generic;

namespace Prota.Editor
{
    [CustomEditor(typeof(ProtaTweeningManager), false)]
    [ExecuteAlways]
    public class ProtaTweeningManagerInspector : UpdateInspector
    {
        VisualElement root;
        VisualElement running;
        Dictionary<UnityEngine.Object, VisualElement> runningList = new Dictionary<UnityEngine.Object, VisualElement>();
        
        ProtaTweeningManager mgr => target as ProtaTweeningManager;
        
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            
            root.Add(running = new VisualElement());
            
            return root;
        }
        
        protected override void Update()
        {
            runningList.SetSync(mgr.targetMap,
                (target, bindingList) => {
                    var x = new VisualElement();
                    return x;
                },
                (target, element, bindingList) => {
                    // TODO
                },
                (target, element) => {
                    // TODO
                }
            );
        }
    }

}
