using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prota.Unity;

namespace Prota.Editor
{
    public static partial class UnityMethodExtensions
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
        
        public static T SetSize<T>(this T x, float width, float height) where T: VisualElement
        {
            x.style.width = x.style.minWidth = x.style.maxWidth = width;
            x.style.height = x.style.minHeight = x.style.maxHeight = height;
            return x;
        }
        
        public static T SetMaxHeight<T>(this T x, float height) where T: VisualElement
        {
            x.style.maxHeight = height;
            return x;
        }
        
        public static T SetMinHeight<T>(this T x, float height) where T: VisualElement
        {
            x.style.minHeight = height;
            return x;
        }
        
        public static T SetMaxWidth<T>(this T x, float height) where T: VisualElement
        {
            x.style.maxWidth = height;
            return x;
        }
        
        public static T SetMinWidth<T>(this T x, float height) where T: VisualElement
        {
            x.style.minWidth = height;
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
            // x.focusable = false;
            // x.pickingMode = PickingMode.Ignore;
            x.SetEnabled(false);
            return x;
        }
        
        public static T SetInteractable<T>(this T x) where T: VisualElement
        {
            // x.focusable = false;
            // x.pickingMode = PickingMode.Ignore;
            x.SetEnabled(true);
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
            => x.AsHorizontalSeperator(height, new Color(.15f, .15f, .15f, 1)).SetMargin(0, 0, 1, 1);
        public static T AsHorizontalSeperator<T>(this T x, float height, Color color) where T: VisualElement
        {
            x.SetGrow().SetHeight(height).AutoWidth();
            x.style.backgroundColor = color;
            return x;
        }
        
        public static T AsVerticalSeperator<T>(this T x, float width) where T: VisualElement
            => x.AsVerticalSeperator(width, new Color(.15f, .15f, .15f, 1)).SetMargin(1, 1, 0, 0);
        public static T AsVerticalSeperator<T>(this T x, float width, Color color) where T: VisualElement
        {
            x.SetGrow().SetWidth(width).AutoHeight();
            x.style.backgroundColor = color;
            return x;
        }
        
        
        public static T SetVisible<T>(this T x, bool visible) where T: VisualElement
        {
            // if(x.visible != visible) x.visible = visible;
            x.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            // x.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
            return x;
        }
        
        public static ObjectField SetTargetType<T>(this ObjectField field)
        {
            field.objectType = typeof(T);
            return field;
        }
        
        public static T SetTargetType<T>(this T field, Type type) where T: ObjectField
        {
            field.objectType = type;
            return field;
        }
        
        public static T SetColor<T>(this T x, Color a) where T: VisualElement
        {
            x.style.backgroundColor = a;
            return x;
        }
        
        public static Label SetTextColor(this Label x, Color a)
        {
            x.style.color = a;
            return x;
        }
        
        public static Label SetTextBold(this Label x)
        {
            x.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            return x;
        }
        
        public static Label SetTextNormal(this Label x)
        {
            x.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Normal);
            return x;
        }
        
        public static Label SetTextCentered(this Label x)
        {
            x.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            return x;
        }
        
        public static TextField SetTextColor(this TextField x, Color a)
        {
            x.style.color = new StyleColor(a);
            return x;
        }
        
        public static TextField SetTextBold(this TextField x)
        {
            x.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            return x;
        }
        
        public static TextField SetTextNormal(this TextField x)
        {
            x.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Normal);
            return x;
        }
        
        public static TextField SetTextCentered(this TextField x)
        {
            x.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            return x;
        }
        
        
        public static T SetCentered<T>(this T x) where T: VisualElement
        {
            x.style.alignSelf = Align.Center;
            return x;
        }
        
        public static T SetFontSize<T>(this T x, int size) where T: VisualElement
        {
            x.style.fontSize = size;
            return x;
        }
        
        public static T OnHoverLeave<T>(this T x, Action<IMouseEvent, bool> f) where T: VisualElement
        {
            x.RegisterCallback<MouseEnterEvent>(e => f(e, true));
            x.RegisterCallback<MouseLeaveEvent>(e => f(e, false));
            return x;
        }
        
        public static T HoverLeaveColor<T>(this T x, Color hover, Color leave) where T: VisualElement
        {
            x.RegisterCallback<MouseEnterEvent>(e => x.style.backgroundColor = hover);
            x.RegisterCallback<MouseLeaveEvent>(e => x.style.backgroundColor = leave);
            x.SetColor(leave);
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
            x.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            return x;
        }
        public static ScrollView HorizontalScroll(this ScrollView x)
        {
            x.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            x.horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
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
        
        public static T SetMargin<T>(this T x, float l, float r, float b, float t) where T : VisualElement
        {
            x.style.marginLeft = l;
            x.style.marginRight = r;
            x.style.marginBottom = b;
            x.style.marginTop = t;
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
        
        public static VisualElement ShowOnCondition<T, G>(this VisualElement x, T listenTarget, Func<G, G, bool> f) where T: VisualElement, INotifyValueChanged<G>
        {
            listenTarget.RegisterValueChangedCallback<G>(e => {
                x.SetVisible(f(e.previousValue, e.newValue));
            });
            return x.SetVisible(f(default, listenTarget.value));
        }
        
        public static T SyncData<G, T>(this T l, int n, Func<int, G> onCreate, Action<int, G> onUpdate, Action<int, G> onDisable)
            where T: VisualElement
            where G:VisualElement
        {
            for(int i = 0; i < n; i++)
            {
                if(i >= l.childCount)
                {
                    l.AddChild(onCreate(i));
                }
                
                onUpdate(i, l[i] as G);
            }
            
            for(int i = n; i < l.childCount; i++) onDisable(i, l[i] as G);
            return l;
        }
        
        public static T SyncData<G, T, V>(this T l, IEnumerable<V> data, Func<V, G> onCreate, Action<V, G> onUpdate)
            where T: VisualElement
            where G: VisualElement
        {
            int i = 0;
            foreach(var e in data)
            {
                if(i >= l.childCount)
                {
                    l.AddChild(onCreate(e));
                }
                onUpdate(e, l[i] as G);
                i++;
            }
            
            for(; i < l.childCount; i++) l[i].SetVisible(false);
            return l;
        }
        
        public static VisualElement AsToggle(this VisualElement e, string name, string hint, SerializedProperty prop)
        {
            return new VisualElement() { name = ":" + name }
                .SetHorizontalLayout()
                .AddChild(new Toggle() { name = name }.WithBinding(prop).SetMargin(0, 4, 0, 0))
                .AddChild(new Label(hint) { name = "hint" });
        }
        
        public static VisualElement AsToggle(this VisualElement e, string name, string hint, out Toggle toggle)
        {
            e.name = ":" + name;
            var s = e
                .SetHorizontalLayout()
                .AddChild(toggle = new Toggle() { name = name }.SetMargin(0, 4, 0, 0))
                .AddChild(new Label(hint) { name = "hint" });
            return s;
        }
        
        public static VisualElement AsToggle(this VisualElement e, string hint, SerializedProperty prop)
        {
            return new VisualElement()
                .SetHorizontalLayout()
                .AddChild(new Toggle().WithBinding(prop).SetMargin(0, 4, 0, 0))
                .AddChild(new Label(hint) { name = "hint" });
        }
        
        public static VisualElement AsToggle(this VisualElement e, string hint, out Toggle toggle)
        {
            var s = e
                .SetHorizontalLayout()
                .AddChild(toggle = new Toggle().SetMargin(0, 4, 0, 0))
                .AddChild(new Label(hint) { name = "hint" });
            return s;
        }
        
        public static T WithBinding<T>(this T bindable, SerializedProperty prop) where T: VisualElement, IBindable
        {
            bindable.BindProperty(prop);
            return bindable;
        }
        
        public static T ReactOnChange<T>(this T self, Action<T> a, params VisualElement[] g) where T: VisualElement
        {
            foreach(var x in g)
            {
                switch(x)
                {
                    case INotifyValueChanged<bool> b:
                        b.RegisterValueChangedCallback<bool>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<int> b:
                        b.RegisterValueChangedCallback<int>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<float> b:
                        b.RegisterValueChangedCallback<float>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<string> b:
                        b.RegisterValueChangedCallback<string>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<Enum> b:
                        b.RegisterValueChangedCallback<Enum>(e => a(self));
                        break;
                        
                    case INotifyValueChanged<Vector2> b:
                        b.RegisterValueChangedCallback<Vector2>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<Vector3> b:
                        b.RegisterValueChangedCallback<Vector3>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<Vector4> b:
                        b.RegisterValueChangedCallback<Vector4>(e => a(self));
                        break;
                        
                    case INotifyValueChanged<Color> b:
                        b.RegisterValueChangedCallback<Color>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<Gradient> b:
                        b.RegisterValueChangedCallback<Gradient>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<AnimationCurve> b:
                        b.RegisterValueChangedCallback<AnimationCurve>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<UnityEngine.Object> b:
                        b.RegisterValueChangedCallback<UnityEngine.Object>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<Quaternion> b:
                        b.RegisterValueChangedCallback<Quaternion>(e => a(self));
                        break;
                    
                    case INotifyValueChanged<Rect> b:
                        b.RegisterValueChangedCallback<Rect>(e => a(self));
                        break;
                    
                    default: throw new Exception("type not handled.");
                }
            }
            
            // initial value.
            a(self);
            
            return self;
        }
    }
}
