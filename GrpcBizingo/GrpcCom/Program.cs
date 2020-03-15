using System;
using System.Threading;
using Grpc.Core;
namespace GrpcCom
{
    class MainClass
    {
        public static void Server()
        {
            /*
            const int Port = 50052;

            Server server = new Server
            {
                Services = { BizingoRpc.BindService(new GrpcBizingoServer.GrpcBizingoServerImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("RouteGuide server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();*/
        }
        public static void Main(string[] args)
        {
            /*
            Thread t = new Thread(new ThreadStart(Server));
            t.Start();*/
        }
    }
}
