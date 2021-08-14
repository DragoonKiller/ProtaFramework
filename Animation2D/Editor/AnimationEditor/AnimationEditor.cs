using UnityEngine;
using UnityEditor;

namespace Prota.Animation
{
    public sealed class AnimationEditorWindow : EditorWindow
    {
        ProtaAnimation target;
        
        
        public void Set(ProtaAnimation e) => target = e;
        
        void OnGUI()
        {
            EditorGUILayout.ObjectField(target, typeof(ProtaAnimation), true);
        }
        
    }
}