using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

using Prota.Unity;

using Prota.Net;

namespace Prota.Editor
{
    [CustomEditor(typeof(Client))]
    public class NetClientInspector : UnityEditor.Editor
    {
        LongField myId;
        IntegerField local;
        TextField server;
        TextField myEndpoint;
        IntegerField pingToServer;
        TextField isHost;
        VisualElement clientInfoList;
        LongField hostId;
        
        Client client => target as Client;
        
        bool inited = false;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            
            root.AddChild(new VisualElement()
                .AddChild(myId = new LongField() { label = "My id", isReadOnly = true })
                .AddChild(local = new IntegerField() { label = "my local port", isReadOnly = true })
                .AddChild(myEndpoint = new TextField() { label = "my endpoint", isReadOnly = true })
                .AddChild(server = new TextField() { label = "server endpoint", isReadOnly = true })
                .AddChild(pingToServer = new IntegerField() { label = "ping to server", isReadOnly = true })
                .AddChild(isHost = new TextField() { label = "is host", isReadOnly = true })
                .AddChild(hostId = new LongField() { label = "Host id", isReadOnly = true })
            )
            .AddChild(new VisualElement().AsHorizontalSeperator(2))
            .AddChild(clientInfoList = new VisualElement())
            .AddChild(new VisualElement().AsHorizontalSeperator(2));
            
            inited = true;
            
            return root;
        }
        
        void OnEnable()
        {
            EditorApplication.update += EditorUpdate;
        }
        
        void OnDisable()
        {
            inited = false;
            EditorApplication.update -= EditorUpdate;
        }
        
        void OnDestroy()
        {
            inited = false;
            OnDisable();
        }
        
        void EditorUpdate()
        {
            if(client == null) return;
            if(!inited) return;
            myId.value = client.myId.id;
            local.value = client.client?.LocalPort ?? -1;
            server.value = client.peerToServer?.EndPoint?.ToString() ?? "";
            myEndpoint.value = client.myEndpoint?.ToString() ?? "";
            pingToServer.value = client.peerToServer?.Ping ?? -1;
            hostId.value = client.hostId;
            
            for(int i = 0; i < client.serverConnections.Count; i++)
            {
                if(clientInfoList.childCount <= i)
                {
                    clientInfoList.AddChild(new VisualElement()
                        .AddChild(new LongField() { name = "id", label = "Net id", isReadOnly = true })
                        .AddChild(new TextField() { name = "ip", label = "\tIP Address", isReadOnly = true })
                        .AddChild(new IntegerField() { name = "port", label = "\tPort", isReadOnly = true })
                        .AddChild(new TextField() { name = "isHost", label = "\tIs host", isReadOnly = true })
                    );
                }
                var element = clientInfoList[i];
                element.Q<LongField>("id").value = client.serverConnections[i].id;
                element.Q<TextField>("ip").value = client.serverConnections[i].ip;
                element.Q<IntegerField>("port").value = client.serverConnections[i].port;
                element.Q<TextField>("isHost").value = client.serverConnections[i].isHost.ToString();
                      
            }
            
            
            
            for(int i = clientInfoList.childCount - 1; i >= client.serverConnections.Count; i--)
            {
                clientInfoList.RemoveAt(i);
            }
            
        }
    }
    
}