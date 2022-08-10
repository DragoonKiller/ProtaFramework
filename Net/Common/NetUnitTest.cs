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
                await clientA.ConnectToServer(serverEndpoint);
                await clientA.EnterRoom(3);
                clientA.room.AssertNotNull();
                Console.WriteLine($"client A enter room: { clientA.room.roomId }");
            });
            
            Task.Run(async () => {
                await clientB.ConnectToServer(serverEndpoint);
                await new SystemTimer(2);
                await clientB.EnterRoom(3);
                clientB.room.AssertNotNull();
                Console.WriteLine($"client B enter room: { clientB.room.roomId }");
                Console.WriteLine($"client B roommates: { string.Join(",", clientB.room.players.Select(x => x.ToString())) }");
            });
            
            Task.Run(async () => {
                await new SystemTimer(6);
                Console.WriteLine("Client Destroy!");
                clientA.Dispose();
                clientB.Dispose();
                await new SystemTimer(2);
                server.Dispose();
                Console.WriteLine("Server Destroy!");
            });
            
            
        }
    }
    
}