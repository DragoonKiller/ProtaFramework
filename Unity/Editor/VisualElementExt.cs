using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        public static VisualElement SetWidth(this VisualElement x, float width)
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = width;
            return x;
        }
        
        public static VisualElement SetHeight(this VisualElement x, float height)
        {
            x.style.height = x.style.minHeight = x.style.maxHeight = height;
            return x;
        }
        
        public static VisualElement SetWidth(this VisualElement x, Length width)
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = width;
            return x;
        }
        
        public static VisualElement SetHeight(this VisualElement x, Length height)
        {
            x.style.height = x.style.minHeight = x.style.maxHeight = height;
            return x;
        }
        
        
        public static VisualElement SetWidth(this VisualElement x, StyleLength width)
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = width;
            return x;
        }
        
        public static VisualElement SetHeight(this VisualElement x, StyleLength height)
        {
            x.style.height = x.style.minHeight = x.style.maxHeight = height;
            return x;
        }
        
        
        public static VisualElement AutoWidth(this VisualElement x)
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = new StyleLength() { keyword = StyleKeyword.Auto };
            return x;
        }
        
        public static VisualElement AutoHeight(this VisualElement x)
        {
            x.style.height = x.style.minHeight = x.style.maxHeight = new StyleLength() { keyword = StyleKeyword.Auto };
            return x;
        }
        
        public static VisualElement SetShrink(this VisualElement x)
        {
            x.style.flexShrink = 1;
            x.style.flexGrow = 0;
            return x;
        }
        
        public static VisualElement SetGrow(this VisualElement x)
        {
            x.style.flexShrink = 0;
            x.style.flexGrow = 1;
            return x;
        }
        
        public static VisualElement SetFixedSize(this VisualElement x)
        {
            x.style.flexShrink = 0;
            x.style.flexGrow = 0;
            return x;
        }
        
        public static VisualElement SetNoInteraction(this VisualElement x)
        {
            x.focusable = false;
            x.pickingMode = PickingMode.Ignore;
            return x;
        }
        
        public static VisualElement SetAbsolute(this VisualElement x)
        {
            x.style.position = Position.Absolute;
            x.style.left = 0;
            x.style.top = 0;
            return x;
        }
        
        public static VisualElement AsHorizontalSeperator(this VisualElement x, float height)
            => x.AsHorizontalSeperator(height, new Color(.1f, .1f, .1f, 1));
        public static VisualElement AsHorizontalSeperator(this VisualElement x, float height, Color color)
        {
            x.SetGrow().SetHeight(height).AutoWidth();
            x.style.backgroundColor = color;
            return x;
        }
        
        public static VisualElement AsVerticalSeperator(this VisualElement x, float width)
            => x.AsVerticalSeperator(width, new Color(.1f, .1f, .1f, 1));
        public static VisualElement AsVerticalSeperator(this VisualElement x, float width, Color color)
        {
            x.SetGrow().SetWidth(width).AutoHeight();
            x.style.backgroundColor = color;
            return x;
        }
        
        
        public static VisualElement SetVisible(this VisualElement x, bool visible)
        {
            x.style.visibility = visible ? new StyleEnum<Visibility>(){ keyword = StyleKeyword.Null } : Visibility.Hidden;
            return x;
        }
        
    }
}