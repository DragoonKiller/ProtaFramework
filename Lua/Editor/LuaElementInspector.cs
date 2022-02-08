using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Prota.Unity;
using XLua;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Prota.Lua
{
    public class LuaElementInspector : VisualElement
    {
        public Func<object> GetLuaElement { get; private set; }
        
        public object key { get; private set; }
        
        public int indent { get; private set; }
        
        const int indentSize = 12;
        
        Label content;
        
        Foldout foldout;
        
        VisualElement subContainer;
        
        public bool subVisible = false;
        
        SortedList<string, LuaElementInspector> elements = new SortedList<string, LuaElementInspector>();
        
        public override VisualElement contentContainer { get => subContainer; }
        
        string path
        {
            get
            {
                var list = new List<string>();
                var x = this;
                for(int i = 0; i < 1000; i++)
                {
                    list.Add(x.name);
                    x = x.parent as LuaElementInspector;
                    if(x == null) break;
                }
                return string.Concat(list.Select(x => "[" + x + "]"));
            }
        }
        
        public LuaElementInspector(int indent, object key, Func<object> GetLuaElement)
        {
            this.GetLuaElement = GetLuaElement;
            this.indent = indent;
            this.key = key;
            this.name = ShowValue(key);
            
            this.SetVerticalLayout();
            
            this.hierarchy.Add(new VisualElement()
                .SetHorizontalLayout()
                .AddChild(new VisualElement()
                    .SetWidth(indent * indentSize)
                )
                .AddChild(foldout = new Foldout() { value = false }
                    .SetWidth(10)
                    .OnValueChange<Foldout, bool>(e => {
                        if(e.newValue)
                        {
                            this.Unfold();
                        }
                        else
                        {
                            this.Fold();
                        }
                    })
                )
                .AddChild(content = new Label() { })
            );
            
            this.hierarchy.Add(subContainer = new VisualElement()
                .SetVerticalLayout()
            );
            
            Fold();
        }
        
        public void Add(LuaElementInspector child)
        {
            elements.Add(child.name, child);
            var index = elements.IndexOfKey(child.name);
            base.Insert(index, child);
        }
        
        public void Remove(LuaElementInspector child)
        {
            elements.Remove(child.name);
            base.Remove(child);
        }
        
        public new void Clear()
        {
            elements.Clear();
            base.Clear();
        }
        
        
        public bool FindSub(string name, out LuaElementInspector res)
        {
            res = null;
            if(elements.TryGetValue(name, out res)) return true;
            return false;
        }
        
        
        
        void Unfold()
        {
            this.subContainer.SetVisible(subVisible = true);
        }
        
        void Fold()
        {
            this.subContainer.SetVisible(subVisible = false);
        }
        
        // ============================================================================================================
        // 更新逻辑
        // ============================================================================================================
        
        
        object oldValueCache;
        object luaValueCache;
        
        public void Update()
        {
            luaValueCache = GetLuaElement();
            
            RefreshValue();
            if(subVisible && luaValueCache is LuaTable table)
            {
                RefreshTable(table);
            }
        }
        
        Dictionary<string, object> newKeys = new Dictionary<string, object>();
        
        void GrabNewTable(LuaTable table)
        {
            newKeys.Clear();
            table.ForEach<object, object>(GrabNewValue);
        }
        
        void GrabNewValue(object k, object v)
        {
            var showKey = ShowValue(k);
            newKeys.Add(showKey, k);
        }
        
        List<LuaElementInspector> toBeRemoved = new List<LuaElementInspector>();
        void RefreshTable(LuaTable table)
        {
            GrabNewTable(table);
            
            // 删除当前有 key 没 value 的.
            toBeRemoved.Clear();
            foreach(var e in elements)
            {
                if(!newKeys.ContainsKey(e.Key)) toBeRemoved.Add(e.Value);
            }
            foreach(var e in toBeRemoved) this.Remove(e);
            
            
            // 添加新的 key-value.
            foreach(var e in newKeys)
            {
                if(!elements.ContainsKey(e.Key)) this.EnsureSubExist(e.Key, e.Value);
            }
            
            // 递归刷新. 只有当前不 fold 才会刷.
            if(subVisible == true)
            {
                foreach(var el in elements) el.Value.Update();
            }
            
            return;
        }
        
        void RefreshValue()
        {
            var isLuaTable = luaValueCache is LuaTable;
            if(isLuaTable || luaValueCache != oldValueCache)
            {
                content.text = key + " : " + ShowValue(luaValueCache);
                oldValueCache = luaValueCache;
            }
            foldout.SetVisible(isLuaTable);
        }
        
        string ShowValue(object value)
        {
            switch(value)
            {
                case LuaTable t:
                return value.ToString();
                
                case string s:
                return "\"" + s + "\"";
                
                case long l:
                return l.ToString();
                
                case double d:
                return d.ToString(".0000000000");
                
                case null:
                return "null";
                
                default:
                return value.ToString();
            }
        }
        
        object GetSubElement(string subName)
        {
            var myLuaElement = luaValueCache;
            if(myLuaElement == null) return null;           // 这个路径没取到.
            if(myLuaElement is LuaTable table)
            {
                return table.Get<object, object>(elements[subName].key);
            }
            return null;
        }
        
        public VisualElement EnsureSubExist(string subName, object subKey)
        {
            if(elements.TryGetValue(subName, out var res)) return res;
            res = new LuaElementInspector(indent + 1, subKey, () => GetSubElement(subName));
            this.Add(res);
            return res;
        }
        
        
        void ClearCache()
        {
            oldValueCache = luaValueCache = null;
        }
        
    }
}