using System;
using System.Threading;
using Grpc.Core;
namespace GrpcCom
{
    class MainClass
    {
        public static void Client()
        {
            var channel = new Channel("127.0.0.1:50052", ChannelCredentials.Insecure);
            var client = new GrpcBizingoClient.GrpcBizingoClientImpl(new BizingoRpc.BizingoRpcClient(channel));
            /*
            client.SendCoord(200, 300);
            client.SendMessage("Teste");
            client.GameOver();
            client.ResetRequest();
            client.FecharConexao();
            client.MandarDialog();
            */
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        public static void Main(string[] args)
        {
            /*
            Console.WriteLine("Hello World!");
            const int Port = 50052;

            Server server = new Server
            {
                Services = { BizingoRpc.BindService(new GrpcBizingoServer.GrpcBizingoServerImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Thread receiver = new Thread(new ThreadStart(Client));
            receiver.Start();

            Console.WriteLine("RouteGuide server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
            */           
        }
    }
}
