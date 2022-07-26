using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using Prota;

using Prota.Editor;

namespace UnityEngine.UIElements
{
    
    public class VisualSerialize : VisualElement
    {
        object key;
        VisualElement keyElement;
        VisualElement keySub;
        
        object value;
        VisualElement valueElement;
        VisualElement valueSub;
        
        
        // 上层用来告诉这个组件, 该更新了. 自己更新自己时也会调用 onUpdate 来完成一些刷新工作.
        public event Action refresh;
        
        
        public VisualSerialize(object key, object value)
        {
            this.key = key;
            this.keyElement = CreateElementFromObject(GetKey, SetKey, out keySub);
            if(value != null)
            {
                this.value = value;
                this.valueElement = CreateElementFromObject(GetValue, SetValue, out valueSub);
            }
            refresh();
        }
        
        public VisualSerialize(object key)
        {
            this.SetHorizontalLayout();
            this.key = key;
            this.keyElement = CreateElementFromObject(GetKey, SetKey, out keySub);
            refresh();
        }
        
        public object GetKey() => key;
        public void SetKey(object x)
        {
            if(key == x) return;
            key = x;
            refresh?.Invoke();
        }
        public object GetValue() => value;
        public void SetValue(object x)
        {
            if(value == x) return;
            value = x;
            refresh?.Invoke();
        }
        
        protected VisualElement CreateElementFromObject(Func<object> Get, Action<object> Set, out VisualElement subElement)
        {
            subElement = null;
            var type = Get().GetType();
            if(type == typeof(byte))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(sbyte))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(short))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(ushort))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(int))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(uint))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(long))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(ulong))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(float))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(double))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    if(x.newValue.NullOrEmpty()) { g.value = "0"; return; }
                    if(!byte.TryParse(x.newValue, out var val)) { g.value = x.previousValue; return; }
                    Set(val);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(type == typeof(string))
            {
                TextField g = null;
                g = new TextField() { value = Get().ToString() }.OnValueChange<TextField, string>(x => {
                    Set(x.newValue);
                });
                refresh = () => g.value = Get().ToString();
                return g;
            }
            else if(typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                ObjectField h = null;
                h = new ObjectField() { value = (UnityEngine.Object)Get() }.OnValueChange<ObjectField, UnityEngine.Object>(x => {
                    Set(x.newValue);
                });
                refresh = () => h.value = (UnityEngine.Object)Get();
                return h;
            }
            else if(type == typeof(Vector2))
            {
                var q = new Vector2Field() { value = (Vector2)Get() }.OnValueChange<Vector2Field, Vector2>(x => {
                    Set(x.newValue);
                });
                refresh = () => q.value = (Vector2)Get();
                return q;
            }
            else if(type == typeof(Vector3))
            {
                var q = new Vector3Field() { value = (Vector3)Get() }.OnValueChange<Vector3Field, Vector3>(x => {
                    Set(x.newValue);
                });
                refresh = () => q.value = (Vector3)Get();
                return q;
            }
            else if(type == typeof(Vector4))
            {
                var q = new Vector4Field() { value = (Vector4)Get() }.OnValueChange<Vector4Field, Vector4>(x => {
                    Set(x.newValue);
                });
                refresh = () => q.value = (Vector4)Get();
                return q;
            }
            else if(type == typeof(Quaternion))
            {
                var q = new Vector3Field() { value = ((Quaternion)Get()).eulerAngles }.OnValueChange<Vector3Field, Vector3>(x => {
                    Set(Quaternion.Euler(x.newValue));
                });
                refresh = () => q.value = (Vector4)Get();
                return q;
            }
            else if(type == typeof(Vector2Int))
            {
                var q = new Vector2IntField() { value = (Vector2Int)Get() }.OnValueChange<Vector2IntField, Vector2Int>(x => {
                    Set(x.newValue);
                });
                refresh = () => q.value = (Vector2Int)Get();
                return q;
            }
            else if(type == typeof(Vector3Int))
            {
                var q = new Vector3IntField() { value = (Vector3Int)Get() }.OnValueChange<Vector3IntField, Vector3Int>(x => {
                    Set(x.newValue);
                });
                refresh = () => q.value = (Vector3Int)Get();
                return q;
            }
            else if(type == typeof(Color))
            {
                var q = new ColorField() { value = (Color)Get() }.OnValueChange<ColorField, Color>(x => {
                    Set(x.newValue);
                });
                refresh = () => q.value = (Color)Get();
                return q;
            }
            else if(type == typeof(Color32))
            {
                var q = new ColorField() { value = (Color32)Get() }.OnValueChange<ColorField, Color>(x => {
                    Set((Color32)x.newValue);
                });
                refresh = () => q.value = (Color)Get();
                return q;
            }
            else if(type.IsEnum)
            {
                var q = new EnumField() { value = (Enum)Get() }.OnValueChange<EnumField, Enum>(x => {
                    Set((Enum)x.newValue);
                });
                refresh = () => q.value = (Enum)Get();
                return q;
            }
            // else if(new ProtaReflection(type).IsGenericOf(typeof(IList<>))) // T[], List<T>, ...
            // {
            //     var sub = new VisualElement();
            //     var q = new Button();
            //     var data = new List<VisualSerialize>();
            //     var list = new IListAdapter(Get());
            //     refresh = () => {
            //         q.text = $"list : { list.Count }";
            //         data.SetLength(list.Count, i => {
            //             var element = list[i];
            //             var v = new VisualSerialize(i, element);
            //             sub.AddChild(v);
            //             return v;
            //         }, (i, v) => {
            //             if((int)v.GetKey() != i || v.GetValue() != list[i]) v.refresh?.Invoke();
            //             v.SetVisible(true);
            //         }, (i, v) => {
            //             v.SetVisible(false);
            //         });
            //     };
            //     subElement = sub;
            //     return q;
            // }
            // else if(new ProtaReflection(type).IsGenericOf(typeof(IDictionary<,>)))  // Dictionary<K, V>, SortedList<K, V>, SortedDictionary<K, V>, ...
            // {
            //     var sub = new VisualElement();
            //     var q = new Button();
            //     var data = new Dictionary<object, VisualSerialize>();
            //     var dict = new Prota
            //     refresh = () => {
            //         q.text = $"dict : { dict.Count }";
            //         data.SetSync(new DictionaryWrapper() { d = dict }, (k, v) => {
            //             var element = new VisualSerialize(k, v);
            //             sub.AddChild(element);
            //             return element;
            //         }, (k, e, v) => {
            //             e.SetKey(k);
            //             e.SetValue(v);
            //             e.SetVisible(true);
            //         }, (k, e) => {
            //             e.SetVisible(false);
            //         });
            //     };
            //     return q;
            // }
            // else if(Get() is ICollection collection)
            // {
            //     var sub = new VisualElement();
            //     var q = new Button();
            //     var data = new Dictionary<object, VisualSerialize>();
            //     refresh = () => {
            //         q.text = $"set : { collection.Count }";
            //         data.SetSync(new ICollectionWrapper() { d = collection }, (k, v) => {
            //             var element = new VisualSerialize(k, v);
            //             sub.AddChild(element);
            //             return element;
            //         }, (k, e, v) => {
            //             e.SetKey(k);
            //             e.SetValue(v);
            //             e.SetVisible(true);
            //         }, (k, e) => {
            //             e.SetVisible(false);
            //         });
            //     };
            //     return q;
            // }
            else
            {
                var q = new Button();
                refresh = () => q.text = $"struct : { Get() }";
                return q;
            }
            
            throw new Exception($"Invalid object { Get() }");
        }

        
    }
}
