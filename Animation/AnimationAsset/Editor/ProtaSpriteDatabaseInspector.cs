using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Prota.Animation
{
    /// <summary>
    /// Track 的基础类型.
    /// </summary>
    [CustomEditor(typeof(ProtaSpriteDatabase))]
    public sealed class ProtaSpriteDatabaseInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if(GUILayout.Button("刷新")) Animation2DEditor.UpdateSpriteDatabase();
            base.OnInspectorGUI();
        }
    }
}