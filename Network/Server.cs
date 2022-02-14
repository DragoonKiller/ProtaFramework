using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace Prota.Net
{
    public class Server : MonoBehaviour
    {
        NetManager server;
        
        void Awake()
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            server = new NetManager(listener);
            server.Start(9050);

            listener.ConnectionRequestEvent += request =>
            {
                request.AcceptIfKey("ProtaClient");
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer.EndPoint);
                NetDataWriter writer = new NetDataWriter();
                writer.Put("Hello client!");
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            };
        }
        
        void Update()
        {
            server.PollEvents();
        }
        
        void OnDestroy()
        {
            server.Stop();
        }
    }
}