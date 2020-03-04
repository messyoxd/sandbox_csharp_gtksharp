using System;
using Grpc.Core;

namespace GrpcBizingoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            const int Port = 50052;

            Server server = new Server
            {
                Services = { BizingoRpc.BindService(new GrpcBizingoServerImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("RouteGuide server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
