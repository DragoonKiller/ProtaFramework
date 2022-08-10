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
                    await clientA.EnterRoom(3);
                    clientA.room.AssertNotNull();
                    $"client A enter room: { clientA.room.roomId }".Log();
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
                    await clientB.EnterRoom(3);
                    clientB.room.AssertNotNull();
                    $"client B enter room: { clientB.room.roomId }".Log();
                    $"client B roommates: { string.Join(",", clientB.room.players.Select(x => x.ToString())) }".Log();
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