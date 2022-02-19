using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    public static partial class MethodExtensions
    {
        public static T SetWidth<T>(this T x, float width) where T: VisualElement
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = width;
            return x;
        }
        
        public static T SetHeight<T>(this T x, float height) where T: VisualElement
        {
            x.style.height = x.style.minHeight = x.style.maxHeight = height;
            return x;
        }
        
        public static T SetWidth<T>(this T x, Length width) where T: VisualElement
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = width;
            return x;
        }
        
        public static T SetHeight<T>(this T x, Length height) where T: VisualElement
        {
            x.style.height = x.style.minHeight = x.style.maxHeight = height;
            return x;
        }
        
        
        public static T SetWidth<T>(this T x, StyleLength width) where T: VisualElement
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = width;
            return x;
        }
        
        public static T SetHeight<T>(this T x, StyleLength height) where T: VisualElement
        {
            x.style.height = x.style.minHeight = x.style.maxHeight = height;
            return x;
        }
        
        
        public static T AutoWidth<T>(this T x) where T: VisualElement
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = new StyleLength() { keyword = StyleKeyword.Auto };
            return x;
        }
        
        public static T AutoHeight<T>(this T x) where T: VisualElement
        {
            x.style.height = x.style.minHeight = x.style.maxHeight = new StyleLength() { keyword = StyleKeyword.Auto };
            return x;
        }
        
        public static T SetShrink<T>(this T x) where T: VisualElement
        {
            x.style.flexShrink = 1;
            x.style.flexGrow = 0;
            return x;
        }
        
        public static T SetGrow<T>(this T x) where T: VisualElement
        {
            x.style.flexShrink = 0;
            x.style.flexGrow = 1;
            return x;
        }
        
        public static T SetFixedSize<T>(this T x) where T: VisualElement
        {
            x.style.flexShrink = 0;
            x.style.flexGrow = 0;
            return x;
        }
        
        public static T SetNoInteraction<T>(this T x) where T: VisualElement
        {
            x.focusable = false;
            x.pickingMode = PickingMode.Ignore;
            return x;
        }
        
        public static T SetAbsolute<T>(this T x) where T: VisualElement
        {
            x.style.position = Position.Absolute;
            x.style.left = 0;
            x.style.top = 0;
            return x;
        }
        
        public static T AsHorizontalSeperator<T>(this T x, float height) where T: VisualElement
            => x.AsHorizontalSeperator(height, new Color(.1f, .1f, .1f, 1));
        public static T AsHorizontalSeperator<T>(this T x, float height, Color color) where T: VisualElement
        {
            x.SetGrow().SetHeight(height).AutoWidth();
            x.style.backgroundColor = color;
            return x;
        }
        
        public static T AsVerticalSeperator<T>(this T x, float width) where T: VisualElement
            => x.AsVerticalSeperator(width, new Color(.1f, .1f, .1f, 1));
        public static T AsVerticalSeperator<T>(this T x, float width, Color color) where T: VisualElement
        {
            x.SetGrow().SetWidth(width).AutoHeight();
            x.style.backgroundColor = color;
            return x;
        }
        
        
        public static T SetVisible<T>(this T x, bool visible) where T: VisualElement
        {
            if(x.visible != visible) x.visible = visible;
            x.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            x.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
            return x;
        }
        
        public static ObjectField SetTargetType<T>(this ObjectField field)
        {
            field.objectType = typeof(T);
            return field;
        }
        
        public static T TargetType<T>(this T field, Type type) where T: ObjectField
        {
            field.objectType = type;
            return field;
        }
        
        public static T SetColor<T>(this T x, Color a) where T: VisualElement
        {
            x.style.backgroundColor = a;
            return x;
        }
        
        public static T SetTextColor<T>(this T x, Color a) where T: Label
        {
            x.style.color = a;
            return x;
        }
        
        public static Label SetTextCentered(this Label x)
        {
            x.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            return x;
        }
        
        public static T HoverLeaveColor<T>(this T x, Color a, Color b) where T: VisualElement
        {
            x.RegisterCallback<MouseEnterEvent>(e => x.style.backgroundColor = a);
            x.RegisterCallback<MouseLeaveEvent>(e => x.style.backgroundColor = b);
            x.SetColor(b);
            return x;
        }
        
        public static T HoverLeaveColor<T>(this T x, Color hover) where T: VisualElement
        {
            var originalColor = x.resolvedStyle.backgroundColor;
            x.RegisterCallback<MouseEnterEvent>(e => x.style.backgroundColor = hover);
            x.RegisterCallback<MouseLeaveEvent>(e => x.style.backgroundColor = originalColor);
            return x;
        }
        
        public static ScrollView VerticalScroll(this ScrollView x)
        {
            x.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            x.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            return x;
        }
        public static ScrollView HorizontalScroll(this ScrollView x)
        {
            x.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            x.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            return x;
        }
        public static ScrollView NoScroll(this ScrollView x)
        {
            x.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            x.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            return x;
        }
        public static ScrollView AllScroll(this ScrollView x)
        {
            x.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            x.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            return x;
        }
        
        public static T SetParent<T>(this T x, VisualElement y) where T: VisualElement
        {
            if(x.parent == y) return x;
            if(x.parent != null) x.parent.Remove(x);
            y.Add(x);
            return x;
        }
        
        public static T AddChild<T>(this T x, VisualElement y) where T: VisualElement
        {
            x.Add(y);
            return x;
        }
        
        public static T SetHorizontalLayout<T>(this T x, bool reversed = false) where T: VisualElement
        {
            x.style.flexDirection = reversed ? FlexDirection.RowReverse : FlexDirection.Row;
            return x;
        }
        public static T SetVerticalLayout<T>(this T x, bool reversed = false) where T: VisualElement
        {
            x.style.flexDirection = reversed ? FlexDirection.ColumnReverse : FlexDirection.Column;
            return x;
        }
        
        public static T SetPadding<T>(this T x, float l, float r, float b, float t) where T : VisualElement
        {
            x.style.paddingLeft = l;
            x.style.paddingRight = r;
            x.style.paddingBottom = b;
            x.style.paddingTop = t;
            return x;
        }
        
        public static T OnClick<T>(this T x, EventCallback<ClickEvent> f) where T: VisualElement
        {
            x.RegisterCallback<ClickEvent>(f);
            return x;
        }
        
        public static T OnValueChange<T, G>(this T x, EventCallback<ChangeEvent<G>> f) where T: VisualElement, INotifyValueChanged<G>
        {
            x.RegisterValueChangedCallback<G>(f);
            return x;
        }
        
        public static ListView Setup<T>(this ListView x, List<T> list, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
        {
            x.itemsSource = list;
            x.selectionType = SelectionType.Single;
            x.makeItem = makeItem;
            x.bindItem = bindItem;
            x.style.flexGrow = 1.0f;
            return x;
        }
    }
}