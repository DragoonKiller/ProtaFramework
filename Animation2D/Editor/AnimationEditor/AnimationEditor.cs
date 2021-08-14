using UnityEngine;
using UnityEditor;

namespace Prota.Animation
{
    public sealed class AnimationEditorWindow : EditorWindow
    {
        ProtaAnimationState target;
        
        
        public void Set(ProtaAnimationState e) => target = e;
        
        void OnGUI()
        {
            EditorGUILayout.ObjectField(target, typeof(ProtaAnimationState), true);
        }
        
    }
}