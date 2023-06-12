using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

using Prota.Unity;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEditorInternal;

namespace Prota.Unity
{
    // 生命周期:
    // None: 不合法
    // Created: gameObject 已创建, 未初始化. 此时 activeSelf = false
    // Inited: 已初始化. 此时一定是关闭状态.
    // Opening: 开始打开窗口. 调用 DoOpen, OnOpenStart. DoOpen 决定了该窗口的打开活动.
    // Open: 窗口已打开, 调用 OnOpen.
    // Closing: 开始关闭窗口. 调用 DoClose, OnCloseStart. DoClose 决定了该窗口的关闭活动.
    // Close: 窗口已关闭, 调用 OnClose.
    public enum UIPanelState
    {
        None = 0,
        NotInited = 1,
        Opening,
        Open,
        Closing,
        Close,
    }
    
    
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel<T> : MonoBehaviour
        where T: UIPanel<T>
    {
        public static T instance { get; private set; }
        
        public UIPanelState state { get; private set; }
        
        public static bool isCreated => instance != null;
        public static bool isInited => isCreated && instance.state != UIPanelState.NotInited;
        public static bool isOpened => isCreated && instance.state == UIPanelState.Open;
        public static bool isClosed => isCreated && instance.state == UIPanelState.Close;
        public static bool isOpening => isCreated && instance.state == UIPanelState.Opening;
        public static bool isClosing => isCreated && instance.state == UIPanelState.Closing;
        
        protected void Awake()
        {
            this.SetActive(false);
            _ = Init();
        }
        
        async Task Init()
        {
            instance = (T)this;
            state = UIPanelState.NotInited;
            OnCreate();
            var task = DoInit();
            if(task != null) await task;
            OnInit();
            state = UIPanelState.Close;
            this.SetActive(true);
        }
        
        protected void OnDestroy()
        {
            if(instance == this) instance = null;
        }
        
        async void _Open()
        {
            if(state == UIPanelState.Opening)
            {
                Debug.LogError("UIPanel is opening, open command reject.");
                return;
            }
            
            if(state == UIPanelState.Open)
            {
                Debug.LogError("UIPanel is already opened, open command reject.");
                return;
            }
            
            if(state == UIPanelState.Closing)
            {
                Debug.LogError("UIPanel is closing, open command reject.");
                return;
            }
            
            if(state == UIPanelState.NotInited)
            {
                Debug.LogError("UIPanel is not inited, open command reject.");
                return;
            }
            
            state = UIPanelState.Opening;
            OnOpenStart();
            var task = this.DoOpen();
            if(task != null) await task;
            OnOpen();
            state = UIPanelState.Open;
        }
        
        async void _Close()
        {
            if(state == UIPanelState.Closing)
            {
                Debug.LogError("UIPanel is closing, close command reject.");
                return;
            }
            
            if(state == UIPanelState.Close)
            {
                Debug.LogError("UIPanel is already closed, close command reject.");
                return;
            }
            
            if(state == UIPanelState.Opening)
            {
                Debug.LogError("UIPanel is opening, close command reject.");
                return;
            }
            
            if(state == UIPanelState.NotInited)
            {
                Debug.LogError("UIPanel is not inited, close command reject.");
                return;
            }
            
            state = UIPanelState.Closing;
            OnCloseStart();
            var task = this.DoClose();
            if(task != null) await task;
            OnClose();
            state = UIPanelState.Close;
        }
        
        public static void Open()
        {
            instance._Open();
        }
        
        public static void Close()
        {
            instance._Close();
        }
        
        // Init 要将窗口初始化到 close 状态.
        protected virtual Task DoInit()
        {
            this.SetActive(false);
            var canvasGroup = this.GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            return null;
        }
        
        protected virtual Task DoOpen()
        {
            var canvasGroup = this.GetComponent<CanvasGroup>();
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1;
            return null;
        }
        
        protected virtual Task DoClose()
        {
            var canvasGroup = this.GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            return null;
        }
        
        protected abstract void OnCreate();
        protected abstract void OnInit();
        protected abstract void OnOpenStart();
        protected abstract void OnOpen();
        protected abstract void OnCloseStart();
        protected abstract void OnClose();
    }
}
