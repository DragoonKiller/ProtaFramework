using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;

namespace Prota.Unity
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel<T> : MonoBehaviour
        where T: UIPanel<T>
    {
        public static T instance { get; private set; }
        
        public bool startOpen = false;
        
        public static bool isOpening { get; private set; }
        
        protected void Awake()
        {
            instance = (T)this;
            OnCreate();
        }
        
        protected void Start()
        {
            if(startOpen) Open();
            else Close();
        }
        
        protected void OnDestroy()
        {
            if(instance == this) instance = null;
        }
        
        void _Open()
        {
            var canvasGroup = this.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            isOpening = true;
            this.OnOpen();
        }
        
        void _Close()
        {
            this.OnClose();
            isOpening = false;
            var canvasGroup = this.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
        }
        
        public static void Open()
        {
            instance._Open();
        }
        
        public static void Close()
        {
            instance._Close();
        }

        protected abstract void OnCreate();
        protected abstract void OnOpen();
        protected abstract void OnClose();
        
    }
}
