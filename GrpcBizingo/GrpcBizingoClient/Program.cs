using System;
using Grpc.Core;

namespace GrpcBizingoClient
{
    class Program
    {

        public class GrpcBizingoClient
        {
            readonly BizingoRpc.BizingoRpcClient client;

            public GrpcBizingoClient(BizingoRpc.BizingoRpcClient c)
            {
                this.client = c;
            }
            public void MandarDialog()
            {
                try
                {
                    Mensagem c = new Mensagem { M = "Teste dialog" };
                    Mensagem m = client.MandarDialog(c);
                    if (m.M.Length > 0)
                    {
                        Console.WriteLine(m.M);
                    }
                    else
                    {
                        Console.WriteLine("Nao funcionou");
                    }
                }
                catch (RpcException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            public void FecharConexao()
            {
                try
                {
                    Flag c = new Flag { G = true };
                    Mensagem m = client.FecharConexao(c);
                    if (m.M.Length > 0)
                    {
                        Console.WriteLine(m.M);
                    }
                    else
                    {
                        Console.WriteLine("Nao funcionou");
                    }
                }
                catch (RpcException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            public void ResetRequest()
            {
                try
                {
                    Flag c = new Flag { G = true };
                    Mensagem m = client.ResetRequest(c);
                    if (m.M.Length > 0)
                    {
                        Console.WriteLine(m.M);
                    }
                    else
                    {
                        Console.WriteLine("Nao funcionou");
                    }
                }
                catch (RpcException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            public void GameOver()
            {
                try
                {
                    Flag c = new Flag { G = true };
                    Mensagem m = client.GameOver(c);
                    if (m.M.Length > 0)
                    {
                        Console.WriteLine(m.M);
                    }
                    else
                    {
                        Console.WriteLine("Nao funcionou");
                    }
                }
                catch (RpcException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            public void SendMessage(string s)
            {
                try
                {
                    Mensagem c = new Mensagem { M = s };
                    Mensagem m = client.SendMessage(c);
                    if (m.M.Length > 0)
                    {
                        Console.WriteLine(m.M);
                    }
                    else
                    {
                        Console.WriteLine("Nao funcionou");
                    }
                }
                catch (RpcException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            public void SendCoord(int x, int y)
            {
                try
                {
                    Casa c = new Casa { X = x, Y = y };
                    Mensagem m = client.SendCoord(c);
                    if (m.M.Length > 0)
                    {
                        Console.WriteLine(m.M);
                    }
                    else
                    {
                        Console.WriteLine("Nao funcionou");
                    }
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
            var channel = new Channel("127.0.0.1:50052", ChannelCredentials.Insecure);
            var client = new GrpcBizingoClient(new BizingoRpc.BizingoRpcClient(channel));

            client.SendCoord(200, 300);
            client.SendMessage("Teste");
            client.GameOver();
            client.ResetRequest();
            client.FecharConexao();
            client.MandarDialog();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
