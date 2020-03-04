using System;
using Grpc.Core;

namespace RpcHelloWorld
{
    class Program
    {

        public class RpcHelloWorldClient
        {
            Comunicao.ComunicaoClient client;
            public RpcHelloWorldClient(Comunicao.ComunicaoClient c)
            {
                this.client = c;
            }

            public void SendCoordinates(int x1, int y1)
            {
                try
                {
                    Casa request = new Casa { X = x1, Y = y1 };
                    Mensagem m = client.SendCoordinates(request);
                    Console.WriteLine($"mensagem: {m.M}");
                }
                catch (RpcException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }


        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:50052", ChannelCredentials.Insecure);
            var client = new RpcHelloWorldClient(new Comunicao.ComunicaoClient(channel));

            client.SendCoordinates(300, 200);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
