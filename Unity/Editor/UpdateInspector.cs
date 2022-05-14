using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Prota.Unity
{
    public abstract class UpdateInspector : UnityEditor.Editor
    {
        public abstract override VisualElement CreateInspectorGUI();
        
        protected virtual void OnEnable()
        {
            EditorApplication.update += Update;
        }
        
        protected virtual void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        
        protected abstract void Update();
        
    }
}