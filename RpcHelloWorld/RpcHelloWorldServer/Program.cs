using System;
using Grpc.Core;

namespace RpcHelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            const int Port = 50052;
            var server = new Server
            {
                Services = { Comunicao.BindService(new RpcHelloWorldImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Hello world rpc ouvindo na porta " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
