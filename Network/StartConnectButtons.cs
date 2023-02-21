using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Netcode.Transports.UTP;

namespace Prota.Network
{
    [RequireComponent(typeof(NetworkManager))]
    [RequireComponent(typeof(UnityTransport))]
    public class StartConnectButtons : MonoBehaviour
    {
        NetworkManager mgr;
        UnityTransport trs;
        
        public string address;
        public ushort port;
        
        // Unity 回调, 编辑器里加载默认值的时候调用.
        // 把已有的 UnityTransport 数据拿下来.
        void Reset()
        {
            mgr = this.GetComponent<NetworkManager>();
            trs = this.GetComponent<UnityTransport>();
            address = trs.ConnectionData.Address;
            port = trs.ConnectionData.Port;
        }
        
        // 同步数据.
        void OnValidate()
        {
            var trs = this.GetComponent<UnityTransport>();
            var c = trs.ConnectionData;
            c.Address = address;
            c.Port = port;
            trs.ConnectionData = c;
        }
        
        void Start()
        {
            Reset();
        }
        
        // IMGUI 画一些联机按钮....
        bool started = false;
        void OnGUI()
        {
            // if(mgr.didStart)
            if(started)
            {
                if(GUILayout.Button("stop connection"))
                {
                    mgr.Shutdown(true);
                    started = false;
                }
                
                return;
            }
            
            if(GUILayout.Button("start server"))
            {
                mgr.StartServer();
                started = true;
            }
            else if(GUILayout.Button("start client"))
            {
                mgr.StartClient();
                started = true;
            }
            else if(GUILayout.Button("start host"))
            {
                mgr.StartHost();
                started = true;
            }
        }
    }
}