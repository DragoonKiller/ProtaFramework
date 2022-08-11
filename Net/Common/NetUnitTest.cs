using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Prota.Net
{
    public static class UnitTest
    {
        [ProtaProtocol(int.MinValue, method = LiteNetLib.DeliveryMethod.ReliableUnordered)]
        public struct NtfUnitTest
        {
            public NetId srcId;
            public string info;
        }
        
        public static void Test()
        {
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.3"), 39793);
            
            var server = new Server(serverEndpoint.Port, 1000);
            
            var clientA = new Client(false);
            var clientB = new Client(false);
            
            
            Task.Run(async () => {
                try
                {
                    await clientA.ConnectToServer(serverEndpoint);
                    $"client A [[{ clientA.id }]] connect to server success!".Log();
                    clientA.AddCallback<NtfUnitTest>((header, data) => {
                        $"Client A tells: { data.info }".Log();
                    });
                    await clientA.EnterRoom(3);
                    clientA.room.AssertNotNull();
                    $"client A enter room: { clientA.room.roomId }".Log();
                    await new SystemTimer(3);
                    $"client A roommates: { clientA.room.players.ToListString(x => x.ToString()) }".Log();
                    clientA.Send(NetId.none, new NtfUnitTest(){ srcId = clientA.id, info = "info sent from client a" });
                    
                    var list = new List<Task>();
                    for(int i = 0; i < 100; i++)
                    {
                        int g = i;
                        list.Add(Task.Run(() => {
                            $"client A send{ g }".Log();
                            clientA.Send(NetId.none, new NtfUnitTest(){ srcId = clientA.id, info = $"number { g } sent from client a" });
                        }));
                    }
                    
                    await Task.WhenAll(list);
                }
                catch(Exception e)
                {
                    e.ToString().LogError();
                }
            });
            
            Task.Run(async () => {
                try
                {
                    await clientB.ConnectToServer(serverEndpoint);
                    $"client B [[{ clientB.id }]] connect to server success!".Log();
                    await new SystemTimer(1);
                    $"client B start room!".Log();
                    clientB.AddCallback<NtfUnitTest>((header, data) => {
                        $"Client B tells: { data.info }".Log();
                    });
                    await clientB.EnterRoom(3);
                    clientB.room.AssertNotNull();
                    $"client B enter room: { clientB.room.roomId }".Log();
                    $"client B roommates: { clientB.room.players.ToListString(x => x.ToString()) }".Log();
                    clientB.Send(NetId.none, new NtfUnitTest(){ srcId = clientA.id, info = "info sent from client b" });
                }
                catch(Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            });
            
            Task.Run(async () => {
                try
                {
                    while(true)
                    {
                        clientA.PollEvents();
                        clientB.PollEvents();
                        await Task.Delay(1);
                    }
                }
                catch(Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
            });
            
            Task.Run(async () => {
                await new SystemTimer(6);
                "Client Destroy!".Log();
                clientA.Dispose();
                clientB.Dispose();
                await new SystemTimer(2);
                server.Dispose();
                "Server Destroy!".Log();
            });
            
            
        }
    }
    
}