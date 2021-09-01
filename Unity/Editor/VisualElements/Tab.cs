using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using Prota.Unity;
using System;

namespace UnityEngine.UIElements
{
    public class SelectTabEvent : EventBase<SelectTabEvent>
    {
        public string name;
    }
    
    public sealed class Tab : VisualElement
    {
        const int height = 18;
        
        private List<Label> tabs = new List<Label>();
        
        public List<bool> hovering = new List<bool>();
        
        
        string _currnetSelect;
        public string currentSelect
        {
            get => _currnetSelect;
            set 
            {
                var origin = _currnetSelect;
                _currnetSelect = value;
                if(value != origin)
                {
                    UpdateState();
                    this.SendEvent(new SelectTabEvent() { name = value, target = this });
                }
            }
        }
        
        public Tab() : base()
        {
            this.SetGrow().SetHeight(height).SetHorizontalLayout();
        }
        
        public Label AddTab(string name, string selectionName, int width = 0)
        {
            var tab = tabs.Where(x => x.text == selectionName).FirstOrDefault();
            if(tab == null)
            {
                var i = tabs.Count;
                tab = new Label() { name = name, text = selectionName }.SetParent(this);
                tabs.Add(tab);
                hovering.Add(false);
                
                tab.SetGrow();
                tab.style.unityTextAlign = TextAnchor.MiddleCenter;
                
                if(width != 0) tab.SetWidth(width);
                tab.RegisterCallback<ClickEvent>(e => {
                    currentSelect = tab.name;
                    UpdateState();
                });
                tab.RegisterCallback<MouseEnterEvent>(e => {
                    hovering[i] = true;
                    UpdateState();
                });
                tab.RegisterCallback<MouseLeaveEvent>(e => {
                    hovering[i] = false;
                    UpdateState();
                });
            }
            if(currentSelect == null) currentSelect = name;
            UpdateState();
            return tab;
        }
        
        public void UpdateState()
        {
            for(int i = 0; i < tabs.Count; i++)
            {
                if(tabs[i].name == currentSelect)
                {
                    tabs[i].SetColor(new Color(.25f, .25f, .25f, 1));
                }
                else
                {
                    if(hovering[i])
                    {
                        tabs[i].SetColor(new Color(.2f, .2f, .2f, 1));
                    }
                    else
                    {
                        tabs[i].SetColor(new Color(.15f, .15f, .15f, 1));
                    }
                }
            }
            
        }
        
        
        
        public new class UxmlFactory : UxmlFactory<Tab> {}
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public UxmlTraits()
            {
                m_Name.defaultValue = "Tab";
            }
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ve.SetGrow().SetHeight(height).SetHorizontalLayout();
            }
        }
        
    }
}