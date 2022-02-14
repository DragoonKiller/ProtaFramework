using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Prota.Net
{
    public class Client : MonoBehaviour
    {
        NetManager client;
        
        void Awake()
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            client = new NetManager(listener);
            client.Start();
            client.Connect("localhost", 9050, "ProtaClient");
            
            listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
            {
                Debug.LogErrorFormat("We got: {0}", dataReader.GetString(100));
                dataReader.Recycle();
            };
        }
        
        void Update()
        {
            client.PollEvents();
        }
        
        void OnDestroy()
        {
            client.Stop();
        }
    }
}