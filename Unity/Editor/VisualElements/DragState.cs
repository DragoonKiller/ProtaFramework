using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

namespace UnityEngine.UIElements
{
    public sealed class DragState
    {
        public Vector2 from { get; private set; }
        
        public Vector2 now { get; private set; }
        
        public Vector2 delta => now - from;
        
        public bool canStartDrag { get; private set; }
        
        public bool dragging { get; private set; }
        
        public Action onDragBegin;
        
        public Action onDragEnd;
        
        public Action onDrag;
        
        public Func<bool> canDrag;
        
        public void Register(VisualElement x)
        {
            x.RegisterCallback<MouseEnterEvent>(e => {
                canStartDrag = true;
            });
            x.RegisterCallback<MouseLeaveEvent>(e => {
                canStartDrag = false;
                TryEndDrag(e);
            });
            x.RegisterCallback<MouseMoveEvent>(e => {
                TryUpdateDrag(e);
            });
            x.RegisterCallback<MouseDownEvent>(e => {
                TryStartDrag(e);
            });
            x.RegisterCallback<MouseUpEvent>(e => {
                TryEndDrag(e);
            });
        }
        
        void TryStartDrag(IMouseEvent e)
        {
            if(dragging) return;
            if(!canStartDrag) return;
            now = from = e.localMousePosition;
            if(canDrag != null && !canDrag()) return;
            dragging = true;
            onDragBegin?.Invoke();
        }
        
        void TryUpdateDrag(IMouseEvent e)
        {
            if(!dragging) return;
            now = e.localMousePosition;
            onDrag?.Invoke();
        }
        
        void TryEndDrag(IMouseEvent e)
        {
            if(!dragging) return;
            now = e.localMousePosition;
            dragging = false;
            onDragEnd?.Invoke();
        }
    }
}