using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Prota.Unity;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Prota.Animation
{
    [CustomPropertyDrawer(typeof(ProtaAnimationTrack.Sprite))]
    public class ProtaSpriteTrackPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return base.CreatePropertyGUI(property);
        }
    }
}