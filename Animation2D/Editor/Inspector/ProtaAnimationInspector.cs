using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Prota.Animation
{
    /// <summary>
    /// Track 的基础类型.
    /// </summary>
    [CustomEditor(typeof(ProtaAnimation))]
    [CanEditMultipleObjects]
    public sealed class ProtaAnimationStateInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}